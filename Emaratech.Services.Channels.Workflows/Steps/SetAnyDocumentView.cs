using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetAnyDocumentView : ChannelWorkflowStep
    {
        public OutputParameter View { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            View.Set(ViewEnum.AnyDocument);
            return StepState = WorkflowStepState.Done;
        }
    }
}