using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Linq;
using Emaratech.Services.Channels.Workflows.Models;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public abstract class SavePassportRequest : ChannelWorkflowStep
    {
        protected string mode = string.Empty;
        protected abstract string PassportEntryListKeyName { get; }

        public InputParameter<JArray> PassportApplication { get; set; }
        public InputParameter<string> Action { get; set; }
        public ReferenceParameter<string> ApplicationId { get; set; }

        public OutputParameter View { get; set; }
        public OutputParameter FeeDetails { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
            FeeDetails = new OutputParameter(nameof(FeeDetails));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(PassportApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            foreach (var entry in PassportApplication.Get())
            {
                JProperty id = entry.Children<JProperty>().FirstOrDefault(p => p.Name == "unifiedId");
                if (id != null)
                {
                    id.Remove();
                }
            }
            
            var request = new JObject();
            if (Action.IsFilled() && Action.Get()?.ToLower() == "draft")
            {
                mode = "DRAFT";
            }
            else if(PassportApplication.IsFilled() && PassportApplication.Get()?.First["applciationRefNumber"] != null)
            {
                mode = "UPDATE";
            }
            else
            {
                mode = "PROCEED";
            }
            request["txnMode"] = mode;
            request[PassportEntryListKeyName] = JArray.FromObject(PassportApplication.Get());

            var response = await CallSaveApi(request);
            if (response is JArray)
            {
                ApplicationId.Value = response.First["applciationRefNumber"]?.ToString();
                View.Set(ViewEnum.EndWorkflow);
                StepState = WorkflowStepState.InputRequired;
            }
            else
            {
                var feesResponse = JObject.FromObject(response);
                FeeDetails.Set(feesResponse);
                StepState = WorkflowStepState.Done;
            }
            return StepState;
        }

        public abstract Task<JToken> CallSaveApi(JObject request);
    };
}