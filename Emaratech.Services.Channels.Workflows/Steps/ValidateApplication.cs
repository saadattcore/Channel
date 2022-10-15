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
    //Step Id= 937A456A0BAB46DFB444993805BF441D
    public class ValidateApplication : ValidateData
    {
        public InputParameter<string> VisaType { get; set; }
        public InputParameter<string> AppType { get; set; }
        public InputParameter<string> AppSubType { get; set; }
        //public InputParameter<string> ApplicationId { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidateApplication));

        protected  override Task CompleteApplication(JObject unifiedAppDoc)
        {

            var applicationNode = unifiedAppDoc["ApplicationDetails"];
            applicationNode["ApplicationType"] = AppType.Get();
            applicationNode["ApplicationSubType"] = AppSubType.Get();

            if (string.IsNullOrEmpty(applicationNode["VisaTypeId"]?.ToString()))
            {
                applicationNode["VisaTypeId"] = VisaType.Get();
            }

            Log.Debug($"App Type, sub type and details are set ");
            var departmentId = applicationNode["DepartmentId"]?.Value<string>();

            Log.Debug($"Value of department id is {departmentId}");

            return Task.FromResult(0);
        }
    }
}