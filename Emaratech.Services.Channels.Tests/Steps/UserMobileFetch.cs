using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Tests;

namespace Emaratech.Services.Channels.Tests.Steps
{
    class UserMobileFetch : RegressionTestStepBase
    {
        public UserMobileFetch() : base("UserMobileFetch.json") { }

        public override RegressionTestStepResponse Execute()
        {
            return Post(base.testData.GetData(), "emaratech.services.channels/ChannelService.svc/json/users/mobile");
        }
    }
}
