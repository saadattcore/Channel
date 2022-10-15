using Emaratech.Services.Document.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveDocumentsPassportRenew : ChannelWorkflowStep
    {
        public ReferenceParameter<IEnumerable<RestDocument>> Documents { get; set; }
        
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            Documents.Value = await GetDocuments(Documents.Value);
            return StepState = WorkflowStepState.Done;
        }

        public static async Task<List<RestDocument>> GetDocuments(IEnumerable<RestDocument> existingDocuments)
        {
            var documents = await ServicesHelper.GetPassportRenewDocuments();
            var result = new List<RestDocument>();

            foreach (var document in documents)
            {
                var required = document["fileMandatory"].ToString() == "1";
                var docTypeId = document["fileTypeId"].ToString();
                if (existingDocuments == null || !existingDocuments.Any(x => x.DocumentTypeId == docTypeId))
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
                    foreach (var doc in existingDocuments.Where(x => x.DocumentTypeId == docTypeId))
                    {
                        doc.Required = required;
                        result.Add(doc);
                    }
                }
            }
            return result;
        }
    }
}