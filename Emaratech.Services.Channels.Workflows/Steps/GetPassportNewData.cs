using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class GetPassportNewData : GetPassportFormData
    {
        protected override void SetSpecificFields(JArray passportApplication, int index)
        {
            foreach (var entry in passportApplication)
            {
                entry["procTypeId"] = JObject.FromObject(new
                {
                    ptypeId = 2
                });
            }

            var firstEntry = passportApplication[index];
            if (firstEntry != null && applicantDetails != null && applicantDetails["EmiratesIdNo"] != null)
            {
                firstEntry["nidNumber"] = applicantDetails["EmiratesIdNo"];
                firstEntry["udbNumber"] = applicantDetails["UnifiedNo"];
            }
        }
    };
}