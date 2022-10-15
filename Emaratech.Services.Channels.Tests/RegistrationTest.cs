using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests
{
    class RegistrationTest : IRegressionTest
    {
        public bool Run(IList<IRegressionTestStep> steps)
        {
            for(int i=0;i<steps.Count;i++)
            {
                var step = steps[i];

                if (i > 0)
                    steps[i].PreviousStep = steps[i - 1];

                var response = step.Execute();

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new RegressionException(step, "Registration Failed");
                else
                {
                    Console.WriteLine($"Step  : {step} : is successful!\nRequest:{response.Request}\nResponse: {response.Response}");
                }
                Console.WriteLine("----------------------------------------------------------");
            }
            return true;
        }
    }
}
