using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class WaitForEstablishment : ChannelWorkflowStep
    {
        public InputParameter<string> EstablishmentCode { get; set; }
        
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(EstablishmentCode);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}
