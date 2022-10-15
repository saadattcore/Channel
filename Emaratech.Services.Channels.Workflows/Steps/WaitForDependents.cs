using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class WaitForDependents : ChannelWorkflowStep
    {
        public InputParameter<string> ResidenceNo { get; set; }

        public InputParameter<string> PermitNo { get; set; }
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(ResidenceNo);
            if (ParametersRequiringInput.Count > 0)
            {
                CheckRequiredInput(PermitNo);

                if (ParametersRequiringInput.Count>1)
                    return StepState = WorkflowStepState.InputRequired;
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}
