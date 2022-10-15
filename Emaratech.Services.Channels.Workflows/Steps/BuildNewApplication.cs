using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class BuildNewApplication : BuildApplication
    {
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            await BuildUnifiedApplicationDocument(null);
            return StepState = WorkflowStepState.Done;
        }
    }
}
