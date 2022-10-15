using Emaratech.Services.Channels.Reports.Models.Sns;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Services.Reports
{
    public static class RootUtils
    {
        public static Root Deserialize(string report)
        {
            return JsonConvert.DeserializeObject<Root>(report);
        }
    }
}