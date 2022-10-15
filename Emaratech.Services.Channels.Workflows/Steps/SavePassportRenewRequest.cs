using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SavePassportRenewRequest : SavePassportRequest
    {
        protected override string PassportEntryListKeyName
        {
            get { return "passportEntryList"; }
        }

        public override async Task<JToken> CallSaveApi(JObject request)
        {
            return await ServicesHelper.SavePassportRenewRequest(request);
        }
    };
}