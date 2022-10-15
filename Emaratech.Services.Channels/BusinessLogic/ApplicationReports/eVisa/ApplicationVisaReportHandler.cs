namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    using Application.Model;
    using Contracts.Rest.Models.Enums;
    using Helpers;
    using log4net;
    using Lookups.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Web;
    using WcfCommons.Faults.Models;
    using Services.Reports;
    using Contracts.Errors;
    using System.Threading.Tasks;
    using Vision.Model;

    public class ApplicationVisaReportHandler : VisaReportHandler
    {
        /// <summary>
        /// Get report list for entry permit application id list
        /// </summary>
        /// <param name="applicationIds">List of entry permit application id</param>
        /// <returns>Ger report list<see cref="ApplicationReports"/></returns>
        public override async Task<IList<ReportRecordInfo>> GetReportEntries(IList<string> ids)
        {
            try
            {
                var entryPermitReprots = new List<ReportRecordInfo>();
                var establishmentCode = ServicesHelper.GetUserEstablishmentsCode();
                var applications = await ApplicationHelper.FilterAndGetEntryPermitApplications(
                                                                    ids,
                                                                    establishmentCode);
                if (!applications.Any())
                {
                    throw ChannelErrorCodes.BadRequest.ToWebFault("Not valid applications.");
                }

                foreach (var application in applications)
                {
                    var reprot = GetReportEntry(application);

                    if (reprot != null)
                    {
                        entryPermitReprots.Add(reprot);
                    }
                }

                return entryPermitReprots;
            }
            catch (FaultException fault)
            {
                Log.Error(fault);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ErrorCodes.BadRequest.ToServiceFault(ex.Message);
            }
        }

        private ReportRecordInfo GetReportEntry(RestApplicationSearchRow application)
        {
            var report = new ReportRecordInfo()
            {
                Id = application.RestApplicationSearchKeyValues?
                                           .FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value,
                UserId = application.RestApplicationSearchKeyValues?
                                    .FirstOrDefault(p => p.PropertyName.ToLower() == "userid")?.Value,
                ServiceId = application.RestApplicationSearchKeyValues?
                                    .FirstOrDefault(p => p.PropertyName.ToLower() == "serviceid")?.Value,
                FileType = application.RestApplicationSearchKeyValues?
                                    .FirstOrDefault(p => p.PropertyName.ToLower() == "file_type")?.Value,
                HasInstructions = true,
                IsRequiredDateParameter = true,
                IsRequiredPhotoParameter = true,
                ReportPath = ReportConstants.EntryPermitReport
            };

            string visaNumber = application.RestApplicationSearchKeyValues?
                                    .FirstOrDefault(p => p.PropertyName.ToLower() == "visa_number")?.Value;

            RestIndividualUserFormInfo permitInfo = null;

            try
            {
                permitInfo = ServicesHelper.GetIndividualProfileByPermitNo(visaNumber).Result;
            }
            catch (Exception ex)
            {
                throw ChannelErrorCodes.PermitNumberNotFound.ToWebFault(
                    $"Profile not found for application number {report.Id} with permit number {visaNumber}");
            }

            report.ReportData = GetEntryPermitReportData(report.Id, permitInfo);

            report.UserPhoto = GetApplicantPhotoDocument(report.Id);

            return report;
        }
    }
}