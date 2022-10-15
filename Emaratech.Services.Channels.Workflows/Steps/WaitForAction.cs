using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class WaitForAction : ChannelWorkflowStep
    {
        public InputParameter<string> Action { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(Action);
            if (ParametersRequiringInput.Count > 0 || string.IsNullOrEmpty(Action.Get()))
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}