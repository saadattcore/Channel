using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Document.Model;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id = 032846035FDE420895EC185145C617B6
    public class ResolveDocuments : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ResolveFees));
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> SponsorType { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> SponsorSponsorType { get; set; }        

        public InputParameter<string> EstablishmentType { get; set; }
        public InputParameter<string> EstablishmentCode { get; set; } 
        public InputParameter<string> ApplicationId { get; set; }
        public InputParameter<string> JobId { get; set; }

        public OutputParameter Documents { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Documents = new OutputParameter(nameof(Documents));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug("Going to execute resolve documents step");
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(ServiceId);
            CheckRequiredInput(JobId);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var documentMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "DocumentMatrix");
            if (string.IsNullOrEmpty(documentMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidDocumentConfiguration.ToWebFault($"DocumentMatrix not configured in system");
            }

            var sponsorEstCode = EstablishmentCode.Get();
            var docsParams = new List<DocumentParameter>()
            {
                new DocumentParameter("Service", ServiceId.Get()),
                new DocumentParameter(nameof(UserType), UserType?.Get()),
                new DocumentParameter(nameof(SponsorType), SponsorSponsorType?.Get()),
                new DocumentParameter(nameof(EstablishmentType), EstablishmentType?.Get()),
                new DocumentParameter("EstablishmentCode", sponsorEstCode),
                new DocumentParameter("ProfessionId", JobId?.Get())
            };

            var parameters = await DataHelper.GetMatrixParameters(UnifiedApplication?.Get(), documentMappingMatrix, SystemId?.Get());
            foreach (var kvp in parameters)
            {
                // If there is any property with the same key, ignore this parameter
                // Input param should be hardcoded and output param is the mapping matrix output, not a search criteria
                if (GetType().GetProperties().All(p => p.Name != kvp.Key) && !docsParams.Exists(d => d.Name == kvp.Key))
                {
                    docsParams.Add(new DocumentParameter(kvp.Key, kvp.Value));
                }
            }
            

            var documents = await ServicesHelper.SearchDocuments(documentMappingMatrix, docsParams);

            if (!string.IsNullOrEmpty(ApplicationId?.Get()) && ApplicationId.IsFilled())
            {
                var applicationDocuments = await ServicesHelper.GetApplicationDocuments(ApplicationId.Get());

                foreach (var document in documents)
                    document.DocumentId = applicationDocuments?.FirstOrDefault(p => p.DocumentType == document.DocumentTypeId)?.DocumenteId;
            }

            Documents.Set(documents);
            StepState = WorkflowStepState.Done;

            return StepState;
        }
    }
}