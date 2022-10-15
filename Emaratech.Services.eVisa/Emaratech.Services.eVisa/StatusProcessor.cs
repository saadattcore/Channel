using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Application.Model;
using log4net;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    abstract class StatusProcessor
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(StatusProcessor));

        private static void MarkApplicationAsProcessed(string applicationId, string appStatus)
        {
            string configPaidStatusList = ConfigurationSystem.AppSettings["FeesPaidStatusId"];
            string propertyToUpdate = "Flag1";

            if (configPaidStatusList.Contains(appStatus))
            {
                propertyToUpdate = "Flag4";
            }

            ServicesHelper.GetApplicationApi.UpdateApplicationField(applicationId,
                new RestApplicationSearchKeyValues()
                {
                    PropertyName = propertyToUpdate,
                    Entity = "ApplicationDetails",
                    Value = "1"
                });
        }

        protected abstract IList<RestApplicationSearchRow> GetApplications();
        protected abstract string GetSubject(RestApplicationSearchRow application);
        protected abstract string GetTemplateContent(RestApplicationSearchRow application);
        protected virtual string GetAttachment(RestApplicationSearchRow application) => null;

        public void Process()
        {
            IList<RestApplicationSearchRow> applications;
            try
            {
                applications = GetApplications();
                Log.Info($"Applications Found = {applications?.Count}");

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                applications = new List<RestApplicationSearchRow>();
            }

            foreach (var application in applications)
            {
                try
                {

                    string applicationId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
                    string sponsorEmail = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "sponsoremail")?.Value;
                    string applicationStatus = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "statusid")?.Value;
                    //string flag4 = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "flag4")?.Value;

                    //if (ConfigurationSystem.AppSettings["FeesPaidStatusId"].Contains(applicationStatus) && flag4 == "1")
                    //{
                    //    Log.Debug($"Application number {applicationId} with sponsor email {sponsorEmail} having status ={applicationStatus} and flag4 = {flag4} email already sent skipping this app");
                    //    continue;
                    //}

                    Log.Debug($"Application number {applicationId} with sponsor email {sponsorEmail} is processing");

                    string subject = GetSubject(application).Replace("{0}", applicationId);

                    string content = GetTemplateContent(application);

                    Log.Debug("Before fetching attachment");
                    string attachment = GetAttachment(application);

                    bool? isEmailSent = ServicesHelper.PushEmail(attachment, content, subject, sponsorEmail, sponsorEmail, applicationId);

                    if (isEmailSent.HasValue && isEmailSent.Value)
                    {
                        MarkApplicationAsProcessed(applicationId, applicationStatus);
                        Log.Debug($"Email Sent successfully with application id :  ====  {applicationId} and Sponsor email {sponsorEmail}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

            }
        }
    }
}
