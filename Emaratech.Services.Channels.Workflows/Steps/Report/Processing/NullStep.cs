using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class NullStep : ProcessingStep
    {
        public NullStep() : base(null)
        {
            
        }

        protected override Task Execute()
        {
            return Task.Run(() => { });
        }
    }
}