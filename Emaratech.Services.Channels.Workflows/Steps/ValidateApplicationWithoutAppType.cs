using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Emaratech.Services.Channels.Workflows.Errors;
using log4net;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id = 31DF2347371C4E3CB2D9C3FF6C96BB72
    public class ValidateApplicationWithoutAppType : ValidateData
    {        
        protected override async Task CompleteApplication(JObject unifiedAppDoc)
        {
            var applicationNode = unifiedAppDoc["ApplicationDetails"];

            var applicationId = await ServicesHelper.GetApplicationId();
            applicationNode["ApplicationId"] = applicationId;
        }
    }
}