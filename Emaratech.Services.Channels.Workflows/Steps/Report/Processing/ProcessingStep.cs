using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public abstract class ProcessingStep
    {
        protected ProcessingStep(ProcessingStep argNextStep)
        {
            NextStep = argNextStep;
        }

        protected ProcessingStep NextStep { get; private set;}
        protected abstract Task Execute();

        // public abstract bool HasNext { get; set;}

        public async Task Process()
        {
            await Execute();
            if (NextStep != null)
            {
                await NextStep.Process();
            }
        }
    }
}