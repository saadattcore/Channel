using System.Threading.Tasks;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SaveApplicationBase : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SaveApplicationBase));

        public InputParameter<JObject> UnifiedApplication { get; set; }

        public ReferenceParameter<string> ApplicationId { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            //TODO: Check if application is already saved, then update it, rather than 
            //creating a new one
            await base.Execute();

            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var app = CreateServiceApplication();

            var appId = await ServicesHelper.SaveApplication(app);
            await ServicesHelper.UpdateApplicationIncompleteDocumentStatus(appId);
            ApplicationId.Value = appId;
            Log.Debug($"Application successfully saved with Id {appId}");
            StepState = WorkflowStepState.Done;
            return StepState;
        }

        protected virtual RestApplication CreateServiceApplication()
        {
            var app = new RestApplication
            {
                ApplicationDetail = UnifiedApplication.Get().ToUnifiedXml().OuterXml
            };
            app.ApplicationId = ApplicationId?.Value;
            return app;
        }
    }
}