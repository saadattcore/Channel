using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests
{
    interface IRegressionTestStep
    {
        IRegressionTestStep PreviousStep { get; set; }
        void Init(ITestData testData);
        RegressionTestStepResponse Execute();
        string SuccessResponse { get; }
    }
}
