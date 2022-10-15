using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class WaitForDocuments : ChannelWorkflowStep
    {
        public InputParameter<IEnumerable<string>> DocumentIds { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(DocumentIds);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}
