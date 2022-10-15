using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolvePreForm : ResolveForm
    {
        public OutputParameter Type { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Type = new OutputParameter(nameof(Type));
            matrixColumnNumber = 2;
        }
    }
}