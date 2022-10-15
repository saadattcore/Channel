using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetDisclaimerView : ChannelWorkflowStep
    {
        public OutputParameter View { get; set; }
        public OutputParameter ResourceKey { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
            ResourceKey = new OutputParameter(nameof(ResourceKey));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            View.Set(ViewEnum.Disclaimer);
            return StepState = WorkflowStepState.Done;
        }
    }
}