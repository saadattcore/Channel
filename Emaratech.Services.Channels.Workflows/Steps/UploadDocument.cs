using Emaratech.Services.Workflows.Engine;
using System.IO;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class UploadDocument : ChannelWorkflowStep
    {
        public InputParameter<Stream> Data { get; set; }

        public OutputParameter DocumentId { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            DocumentId = new OutputParameter(nameof(DocumentId));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            
            CheckRequiredInput(Data);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            //var documentId = ServicesHelper.UploadDocument(Data.Get());
            //DocumentId.Set(documentId);

            return StepState = WorkflowStepState.Done;
        }
    }
}