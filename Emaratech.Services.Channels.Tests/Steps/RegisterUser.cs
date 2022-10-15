using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests.Steps
{
    class RegisterUser : RegressionTestStepBase
    {
        public override RegressionTestStepResponse Execute()
        {
            var key = Guid.NewGuid().ToString();

            var emiratesId = "784199370209688";
            var username = "Regression_User_" + key;
            var passowrd = "P@ss1234";
            var dateOfBirth = "2002-05-11T07:09:52.996Z";
            var email = username+"@regression.unified.emaratech.ae";

            return Post(
                $"{{\"emiratesId\": \"{emiratesId}\",\"username\": \"{username}\", \"password\": \"{passowrd}\", \"dateOfBirth\": \"{dateOfBirth}\", \"email\": \"{email}\", \"otpToken\": {PreviousStep.SuccessResponse}, \"registerNewMobileNumberToken\": \"\" }}"
                , "Emaratech.Services.Channels/ChannelService.svc/json/users");
        }
    }
}
