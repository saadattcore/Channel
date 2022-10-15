using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests.Steps
{
    class MobileVerificationCheck : RegressionTestStepBase
    {
        public override RegressionTestStepResponse Execute()
        {
            JObject jObj = JObject.Parse(PreviousStep.PreviousStep.SuccessResponse);
            var mobile = jObj.Descendants()
                                .OfType<JProperty>()
                                .Where(p => p.Name == "mobileNumbers")
                                .First()
                                .Value
                                .First();

            var otpCode = base.Get($"Emaratech.Services.Sms/SMSService.svc/json/verify/test/{mobile}/0E56E4FE722847BCBD4F2569E2C87E14").Response;

            return Post($"{{\"otpCode\": {otpCode},\"otpToken\": {PreviousStep.SuccessResponse}}}", "emaratech.services.channels/ChannelService.svc/json/users/mobile/verification/check");
        }
    }
}
