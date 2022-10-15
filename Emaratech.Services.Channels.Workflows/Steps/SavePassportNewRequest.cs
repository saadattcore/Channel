using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Workflows.Errors;
using System;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SavePassportNewRequest : SavePassportRequest
    {
        protected override string PassportEntryListKeyName
        {
            get { return "passportEntryListNewBorn"; }
        }

        public override async Task<JToken> CallSaveApi(JObject request)
        {
            var application = PassportApplication.Get()?.First;
            var emiratesId = application["nidNumber"]?.ToString();
            var unifiedNo = application["udbNumber"]?.ToString();
            JArray existing = null;

            if (mode != "UPDATE")
            {
                try
                {
                    existing = await ServicesHelper.FetchPassportNewRequest(emiratesId, unifiedNo);
                }
                catch (Exception)
                {
                    throw ChannelWorkflowErrorCodes.PassportRequestAlreadyExists.ToWebFault($"A request already exists for this emirates ID and unified number");
                }

                if (existing != null && existing.First != null)
                {
                    throw ChannelWorkflowErrorCodes.PassportRequestAlreadyExists.ToWebFault($"A request already exists for this emirates ID and unified number");
                }
            }
            return await ServicesHelper.SavePassportNewRequest(request);
        }
    };
}