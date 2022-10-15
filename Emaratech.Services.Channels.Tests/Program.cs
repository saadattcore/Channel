using Emaratech.Services.Channels.Tests;
using Emaratech.Services.Channels.Tests.Steps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var registration = new RegistrationTest();
            registration.Run(new List<IRegressionTestStep> {
                new UserMobileFetch(),
                new MobileVerificationInit (),
                new MobileVerificationCheck(),
                new RegisterUser() });
        }
    }
}