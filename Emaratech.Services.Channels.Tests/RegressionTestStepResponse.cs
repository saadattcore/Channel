using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests
{
    class RegressionTestStepResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}
