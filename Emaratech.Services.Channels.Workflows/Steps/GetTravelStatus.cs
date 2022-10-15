using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Xml;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public abstract class GetTravelStatus : ChannelWorkflowStep
    {
        protected JToken applicantNode;

        public ReferenceParameter<JObject> UnifiedApplication { get; set; }
        protected static readonly ILog Log = LogManager.GetLogger(typeof(GetTravelStatus));

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            return StepState = WorkflowStepState.Done;
        }

        protected void SetIsInsideCountryNode(string travelType)
        {
            TravelType travelStatus = TravelType.Exit;
            if (!string.IsNullOrEmpty(travelType))
            {
                Log.Debug($"Travel type is {travelType}");
                travelStatus = (TravelType)Convert.ToInt32(travelType);
            }
            UnifiedApplication.Value["ApplicantDetails"]["IsInsideUAE"] = WorkflowConstants.TravelTypeIsInsideMapping[travelStatus];
            UnifiedApplication.Value["ApplicantDetails"]["InsideOutside"] = (int) travelStatus;
        }
    }
}