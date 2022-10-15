using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests.Steps
{
    class MobileVerificationInit : RegressionTestStepBase
    {
        public override RegressionTestStepResponse Execute()
        {
            JObject jObj = JObject.Parse(PreviousStep.SuccessResponse);
            var mobile = jObj.Descendants()
                .OfType<JProperty>().First(p => p.Name == "mobileNumbers")
                                .Value
                                .First();


            return Post($"{{\"mobileNumber\" : \"{mobile}\"}}", "Emaratech.Services.Channels/ChannelService.svc/json/users/mobile/verification/init");
        }
    }
}
