using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class GetPassportRenewData2 : GetPassportRenewData { }
    public class GetPassportRenewData3 : GetPassportRenewData { }
    public class GetPassportRenewData : GetPassportFormData
    {
        protected override void SetSpecificFields(JArray passportApplication, int index)
        {
            SetPassportRenewSpecificData(passportApplication, 0, applicantDetails);
        }

        public static void SetPassportRenewSpecificData(JArray passportEntries, JToken dependantToEdit, JToken applicantDetails)
        {
            foreach (var entry in passportEntries)
            {
                entry["procTypeId"] = JObject.FromObject(new
                {
                    ptypeId = 1
                });
            }
            
            if (dependantToEdit != null && applicantDetails != null && applicantDetails["EmiratesIdNo"] != null)
            {
                dependantToEdit["nidNumber"] = applicantDetails["EmiratesIdNo"];
                dependantToEdit["udbNumber"] = applicantDetails["UnifiedNo"];
            }
        }
    };
}