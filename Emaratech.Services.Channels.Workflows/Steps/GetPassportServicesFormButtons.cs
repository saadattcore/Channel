using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class GetPassportServicesFormButtons : ChannelWorkflowStep
    {
        public InputParameter<JArray> PassportApplication { get; set; }

        public OutputParameter AdditionalButtons { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            AdditionalButtons = new OutputParameter(nameof(AdditionalButtons));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            if (!PassportApplication.IsFilled() || 
                PassportApplication.Get().First?["applciationRefNumber"] == null)
            {
                AdditionalButtons.Set(new List<RestButton>
                {
                    new RestButton { Action = "draft", ResourceKey = "SaveDraft" }
                });
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}