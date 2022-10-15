using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Lookups.Model;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.BusinessLogic.ApplicationReports;
using Emaratech.Services.Application.Model;
using System;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService
    {
        public async Task<IEnumerable<RestReportsHistory>> GetReportsHistory()
        {
            var userId = ClaimUtil.GetAuthenticatedUserId();

            var services = (await serviceApi.GetSystemServicesAsync(new Emaratech.Services.Services.Model.Systems { SystemIds = new List<string> { systemSettings.SystemId } }, "1", short.MaxValue.ToString())).Data;

            var allServices = services.Select(x => new RestService
            {
                Id = x.ServiceId,
                ResourceKey = x.ResourceKey,
                CategoryId = x.CategoryInfo?.Parent?.Id,
                CategoryResourceKey = x.CategoryInfo?.Parent?.ResourceKey,
                SubCategoryId = x.CategoryInfo?.Id,
                SubCategoryResourceKey = x.CategoryInfo?.ResourceKey
            }).ToList();


            var report = await ApiFactory.Default.GetApplicationApi().GetReportsListAsync(userId);

            foreach (var obj in report.LstSnsReportInfo)
            {

                obj.ResourceKey = allServices.First(p => p.Id == obj.ServiceId).ResourceKey;
            }

            return mapper.Map<IList<RestReportsHistory>>(report.LstSnsReportInfo).AsEnumerable();
        }

        public async Task<IEnumerable<RestReportsAllowed>> GetAllowedReports(string systemId)
        {
            var serviceList = await GetServices();
            var reportServiceList = serviceList.Where(p => p.CategoryId == ConfigurationManager.AppSettings["ReportCategoryId"]).ToList();
            List<RestReportsAllowed> lstReportsAllowed = reportServiceList.Select(service => new RestReportsAllowed { ServiceId = service.Id, ServiceResourceKey = service.ResourceKey }).ToList();
            return lstReportsAllowed.AsEnumerable();
        }

        public async Task<Stream> GetReport(string applicationId)
        {
            var currentContext = WebOperationContext.Current;
            // Capturing context since context will be null after await statement. 
            // This is weird since <httpRuntime> and <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true" /> should take care of this but they don't.

            var userId = ClaimUtil.GetAuthenticatedUserId();
            await ApplicationHelper.ValidateApplication(userId, applicationId);
            return GetReportImpl.Create(currentContext).AsPdfOrExcel(applicationId, userId);
        }

        public async Task<Stream> GetReportWithToken(string token)
        {
            var currentContext = WebOperationContext.Current;

            var values = await TokenUtils.VerifyTokenInContext(SystemId, token);
            var applicationId = values.Single(x => x.Name == "ApplicationId").Value;
            var userId = values.Single(x => x.Name == "UserId").Value;
            await ApplicationHelper.ValidateApplication(userId, applicationId);

            return GetReportImpl.Create(currentContext).AsPdfOrExcel(applicationId, userId);
        }

        public async Task SendReport(string applicationId, string email)
        {

            var userId = ClaimUtil.GetAuthenticatedUserId();
            await ApplicationHelper.ValidateApplication(userId, applicationId);
            var sendReportStatus = await new SendReportImpl().CreateAndSend(email, applicationId, userId, SystemId);
        }

        public async Task<RestReportingResponse> Reporting(RestReportingRequest req)
        {
            if (ClaimUtil.GetAuthenticatedUserType() == Emaratech.Services.Channels.Models.Enums.Constants.UserTypeLookup.SuperAdminUserType)
            {
                var mappedRequest = mapper.Map<ReportingRequest>(req);
                var response = await ApiFactory.Default.GetLookupApi().ReportingAsync(mappedRequest);
                return mapper.Map<RestReportingResponse>(response);
            }
            else
                throw ChannelErrorCodes.Unauthorized.ToWebFault("User type is not authorized to use this feature");
        }

        public async Task<int?> ReportingUpdate(RestReportingRequest req)
        {
            if (ClaimUtil.GetAuthenticatedUserType() == Emaratech.Services.Channels.Models.Enums.Constants.UserTypeLookup.SuperAdminUserType)
            {
                var mappedRequest = mapper.Map<ReportingRequest>(req);
                return await ApiFactory.Default.GetLookupApi().ReportingUpdateAsync(mappedRequest);
            }
            else
                throw ChannelErrorCodes.Unauthorized.ToWebFault("User type is not authorized to use this feature");
        }

        /// <inhertdoc/>
        public async Task<RestReportResponse> GenerateReport(RestReportsRequest reportRequest)
        {
            var reportType = ValidateRequestAndGetReportType(reportRequest);

            var ids = reportRequest.Ids.Distinct().ToList();

            var reportHandler = ReportHandlerFactory.Create(reportType);

            var reportEntries = await reportHandler.GetReportEntries(ids);

            var reportContent = string.Empty;
            if (reportEntries.Any())
            {
                reportContent = reportHandler.GenerateReport(reportEntries);
            }

            return BuildPrintResponse(ids, reportEntries, reportContent);
        }

        private ReportType ValidateRequestAndGetReportType(RestReportsRequest reportRequest)
        {
            if (!ClaimUtil.IsLoggedIn())
            {
                throw ChannelErrorCodes.Unauthorized.ToWebFault(string.Empty);
            }

            if (reportRequest == null ||
                reportRequest.Ids == null)
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault("Invalid request parameter");
            }

            var reportType = GetReportType(reportRequest.ReportType);
            if (reportType == ReportType.None)
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault("Invalid report type parameter");
            }

            int maxPrintNumber = 0;
            var applicationsCount = reportRequest.Ids.Distinct().Count();

            try
            {
                maxPrintNumber = systemSettings.GetProperty<int>("MaxNumberOfApplicationToPrint");
            }
            catch
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault(
                    "Fail to get maximum print number form system setting");
            }

            if (applicationsCount < 1 && applicationsCount > maxPrintNumber)
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault(
                    $"Invalid count, the count should be between 1 and {maxPrintNumber}");
            }

            var isEstablishmentUser = ClaimUtil.GetAuthenticatedUserType() == Constants.UserTypeLookup.EstablishmentUserType;

            var hasPrintAccess = false;
            var printPermissions = ConfigurationManager.AppSettings.Get(
                Constants.ConfigurationKeys.PrintPermissions).Split(',').ToList();
            foreach (var permission in printPermissions)
            {
                if (authorizationManager.CheckAccess(null, permission))
                {
                    hasPrintAccess = true;
                    break;
                }
            }

            if (!isEstablishmentUser || !hasPrintAccess)
            {
                throw ChannelErrorCodes.Unauthorized.ToWebFault("User is not authorized to use this feature");
            }

            return reportType;
        }

        private ReportType GetReportType(string value)
        {
            ReportType type;

            var isValidType = Enum.TryParse(value, out type);

            return isValidType ? type : ReportType.None;
        }

        private RestReportResponse BuildPrintResponse(
            List<string> allIds,
            IList<ReportRecordInfo> reportEntries,
            string reportContent)
        {
            var validIds = (from entry in reportEntries
                                       select entry.Id).ToList();

            var notValidIds = allIds.Except(validIds).ToList();

            return new RestReportResponse()
            {
                ReportContentAsBase64String = reportContent,
                FileName = "Report.pdf",
                FileType = ReportConstants.Pdf,
                ValidApplications = validIds,
                NotValidApplications = notValidIds,
                Description = GetPrintEntryPemitDescription(validIds, notValidIds)
            };
        }

        private string GetPrintEntryPemitDescription(
                List<string> validApplicationIds,
                List<string> notValidApplicationIds)
        {
            var validMessage = string.Empty;
            var invalidMessage = string.Empty;

            if (validApplicationIds.Any())
            {
                validMessage = $"The report generated for ids { string.Join(", ", validApplicationIds)}. ";
            }

            if (notValidApplicationIds.Any())
            {
                invalidMessage = $" But the ids { string.Join(", ", notValidApplicationIds)} are invalid.";
            }

            Log.Debug(validMessage + invalidMessage);
            return validMessage + invalidMessage;
        }
    }
}