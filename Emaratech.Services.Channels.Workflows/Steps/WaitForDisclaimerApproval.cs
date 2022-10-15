using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class WaitForDisclaimerApproval : ChannelWorkflowStep
    {
        public InputParameter<bool?> DisclaimerApproved { get; set; }
        public InputParameter<RestSearchInfo> SearchInfo { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            if (!SearchInfo.IsFilled())
            {
                CheckRequiredInput(DisclaimerApproved);
                if (ParametersRequiringInput.Count > 0 || DisclaimerApproved?.Get() == false)
                {
                    return StepState = WorkflowStepState.InputRequired;
                }
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}