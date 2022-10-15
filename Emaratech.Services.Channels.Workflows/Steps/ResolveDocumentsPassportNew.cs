using Emaratech.Services.Document.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveDocumentsPassportNew : ChannelWorkflowStep
    {
        public ReferenceParameter<IEnumerable<RestDocument>> Documents { get; set; }
        
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            var documents = await ServicesHelper.GetPassportNewDocuments();
            var result = new List<RestDocument>();
            foreach (var document in documents)
            {
                var required = document["fileMandatory"].ToString() == "1";
                var docTypeId = document["fileTypeId"].ToString();
                if (Documents.Value == null)
                {
                    result.Add(new RestDocument
                    {
                        Required = required,
                        DocumentTypeId = docTypeId,
                        ResourceKey = ReusePassportFormData.GetPassportDocumentResourceKey(Convert.ToInt32(document["fileTypeId"]))
                    });
                }
                else
                {
                    foreach (var doc in Documents.Value.Where(x => x.DocumentTypeId == docTypeId))
                    {
                        doc.Required = required;
                    } 
                }
            }

            if (Documents.Value == null)
            {
                Documents.Value = result;
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}