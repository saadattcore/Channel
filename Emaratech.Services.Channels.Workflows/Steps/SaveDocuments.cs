using Emaratech.Services.Application.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SaveDocuments : ChannelWorkflowStep
    {
        public InputParameter<string> ApplicationId { get; set; }
        public InputParameter<IEnumerable<string>> DocumentIds { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(ApplicationId);
            CheckRequiredInput(DocumentIds);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            // Create list of documents
            var documentIds = DocumentIds.Get().ToArray();
            var results = await ServicesHelper.GetDocumentInfo(documentIds);
            var documents = results.Select((x) => new
                RestApplicationDocument
                (null, x.DocumentId, x.DocumentTypeId
                )).ToList();

            if (documents.Any())
            {
                var app = new RestApplication
                {
                    ApplicationDocument = documents
                };
                var appId = await ServicesHelper.UpdateApplicationDocuments(ApplicationId.Get(), documents);
                await ServicesHelper.UpdateApplicationCompleteStatus(ApplicationId.Get());
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}