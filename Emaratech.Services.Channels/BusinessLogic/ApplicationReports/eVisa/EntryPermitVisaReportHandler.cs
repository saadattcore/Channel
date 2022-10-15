using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.WcfCommons.Faults.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    public class EntryPermitVisaReportHandler : VisaReportHandler
    {
        public override async Task<IList<ReportRecordInfo>> GetReportEntries(IList<string> ids)
        {
            try
            {
                var entryPermitReprots = new List<ReportRecordInfo>();
                var establishmentCode = ServicesHelper.GetUserEstablishmentsCode();
                var validPermits = await GetValidEnrtyPermits(ids);
                var applications = await ApplicationHelper.GetApplicationsForEntryPermits(
                                          validPermits.Select(
                                              p => p.IndividualUserInfo.IndividualPermitInfo.PermitNo)
                                              .ToList(),
                                          establishmentCode);
                if (!applications.Any())
                {
                    throw ChannelErrorCodes.BadRequest.ToWebFault(
                        "Not valid entry permits.");
                }

                foreach (var permit in validPermits)
                {
                    var application = GetApplication(
                        applications, 
                        permit.IndividualUserInfo.IndividualPermitInfo.PermitNo);

                    if(application == null)
                    {
                        continue;
                    }

                    var reprot = GetReportEntry(application, permit);

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

        private ReportRecordInfo GetReportEntry(
            RestApplicationSearchRow application, 
            Vision.Model.RestIndividualUserFormInfo permitInfo)
        {
            var report = new ReportRecordInfo()
            {
                Id = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitNo,
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

            var appId = application.RestApplicationSearchKeyValues?
                           .FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;

            report.ReportData = GetEntryPermitReportData(appId, permitInfo);

            report.UserPhoto = GetApplicantPhotoDocument(appId);

            return report;
        }

        private async Task<List<RestIndividualUserFormInfo>> GetValidEnrtyPermits(
            IList<string> ids)
        {
            var permits = new List<RestIndividualUserFormInfo>();
            var entrypermitStatuses = ConfigurationRepository.GetEntryPermitPrintStatuses();

            try
            {
                permits = await ServicesHelper.GetIndividualProfileListByPermitNoForReport(ids.ToList());
            }
            catch
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault("Not valid entry permits.");
            }


            var validPermits = (from permit in permits
                                let permitStatus = permit?.IndividualUserInfo?
                                                          .IndividualPermitInfo?
                                                          .PermitStatusId
                                where entrypermitStatuses.Contains(permitStatus.Value)
                                select permit).ToList();


            if (!validPermits.Any())
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault("Not valid entry permits.");
            }

            return validPermits;
        }

        private RestApplicationSearchRow GetApplication(
            IList<RestApplicationSearchRow> applications, 
            string visaNumber)
        {
            return (from application in applications
                    where application.RestApplicationSearchKeyValues?.
                                      FirstOrDefault(p => p.PropertyName.ToLower() == "visa_number")
                                      .Value == visaNumber
                    select application).FirstOrDefault();
        }
    }
}