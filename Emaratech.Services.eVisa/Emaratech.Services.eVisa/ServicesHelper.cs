using Emaratech.Services.Lookups.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Lookups.Api;
using System.Configuration;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.Application.Api;
using Emaratech.Services.Document.Api;
using Emaratech.Services.Email.Api;
using Emaratech.Services.Email.Model;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
   public static class ServicesHelper
    {

        public static async Task<IEnumerable<RestLookupDetail>> GetLookupItems(string lookupId)
        {
            return (await new LookupApi(ConfigurationSystem.AppSettings["LookupsApi"]).GetLookupDetailsAsync(lookupId, string.Empty, string.Empty)).Data;
        }

        public static VisionIndividualApi GetVisionApi
        {
            get
            {
                return new Vision.Api.VisionIndividualApi(ConfigurationSystem.AppSettings["VisionApi"]);
            }
        }

        public static VisionEstablishmentApi GetVisionEstablishmentApi
        {
            get
            {
                return new Vision.Api.VisionEstablishmentApi(ConfigurationSystem.AppSettings["VisionApi"]);
            }
        }

        public static ApplicationSearchApi GetApplicationSearchApi
        {
            get
            {
                return new ApplicationSearchApi(ConfigurationSystem.AppSettings["ApplicationApi"]);
            }
        }
        public static ApplicationApi GetApplicationApi
        {
            get
            {
                return new ApplicationApi(ConfigurationSystem.AppSettings["ApplicationApi"]);
            }
        }

        public static DocumentApi GetDocumentApi
        {
            get
            {
                return new DocumentApi(ConfigurationSystem.AppSettings["DocumentApi"]);
            }
        }

        public static EmailApi GetEmailApi
        {
            get
            {
                return new EmailApi(ConfigurationSystem.AppSettings["EmailApi"]);
            }
        }

      

        public static bool? PushEmail(string argAttachment, string argContent, string argSubject, string argEmail, string argName , string applicationId = "")
        {
            var email = new RestAddress() { Email = argEmail, Name = argName };

            var attachment = new RestAttachment() { Name = applicationId, File = argAttachment };

            var emailToSend = new RestEmailProperties()
            {
                AccountId = "771640C526BA4804916329811C7129B5",
                Content = argContent,
                ContentType = "text/html; charset=utf-8",
                Subject = argSubject,
                Attachment = string.IsNullOrEmpty(argAttachment) ? null : new List<RestAttachment>() { attachment },
                To = new List<RestAddress>() { email }
            };
            return GetEmailApi.SendEmail(emailToSend);
        }
    }
}
