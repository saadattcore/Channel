using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Workflows.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ReusePassportNewFormData : ReusePassportFormData
    {
        protected override void SetSpecificFields(JObject formData, JArray passportApplication, Dictionary<string, List<FormField>> formConfiguration)
        {
            var applicant = passportApplication.First;
            var applicantDetails = formData["ApplicantDetails"];
            applicantDetails["EmiratesIdNo"] = applicant["nidNumber"];
            applicantDetails["UnifiedNo"] = applicant["udbNumber"];
        }

        protected async override Task<JArray> GetApplicationRequest(RestSearchInfo searchInfo)
        {
            return await ServicesHelper.FetchPassportNewRequest(searchInfo.EmiratesId, searchInfo.UnifiedNumber);
        }
    }
}