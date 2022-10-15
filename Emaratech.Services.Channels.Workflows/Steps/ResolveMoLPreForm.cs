using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveMoLPreForm : ResolvePreForm
    {
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            Type.Set(ViewTypeEnum.MoL);
            return StepState;
        }
    }
}