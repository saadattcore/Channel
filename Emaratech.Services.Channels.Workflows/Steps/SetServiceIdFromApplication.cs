using System.Linq;
using Emaratech.Services.Workflows.Engine;
using Emaratech.Services.MappingMatrix.Model;
using Emaratech.Services.Channels.Workflows.Models;
using System.Collections.Generic;
using Emaratech.Services.Channels.Workflows.Errors;
using System.Threading.Tasks;
using Emaratech.Utilities;
using log4net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //STEP ID = C7857FD99EB74EE5A6CD489A76270C43
    public class SetServiceIdFromApplication : ChannelWorkflowStep
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(SetServiceIdFromApplication));
        public InputParameter<JObject> UnifiedApplication { get; set; }

        public OutputParameter ServiceId { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            ServiceId = new OutputParameter(nameof(ServiceId));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            string serviceId = UnifiedApplication.Get()["ApplicationDetails"]["ServiceId"]?.Value<string>();

            LOG.Debug($"Service ID Found from Unified Application object is {serviceId}");

            ServiceId.Set(serviceId);

            return StepState = WorkflowStepState.Done;
        }
    }
}
