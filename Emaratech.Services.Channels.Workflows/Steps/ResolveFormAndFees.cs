using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine.Interfaces;
using Emaratech.Services.Channels.Workflows.Models;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveFormAndFees : SequenceStep
    {
        public OutputParameter View { get; set; }

        public ResolveFormAndFees(IList<IWorkflowStep> childSteps)
            : base(childSteps)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            var state = await base.Execute();
            View.Set(ViewEnum.FormAndPay);
            return state;
        }
    }
}