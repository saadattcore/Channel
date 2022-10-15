using Emaratech.Services.Channels.Workflows.Steps.Report;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ReadEmailAddressForReport : ChannelWorkflowStep
    {
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public OutputParameter ReportEmailAddress { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            ReportEmailAddress = new OutputParameter(nameof(ReportEmailAddress));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            var emailAddress = UnifiedApplication.Get()["SponsorDetails"]["SponsorEmail"];
            ReportEmailAddress.Set(emailAddress);
            return StepState = WorkflowStepState.Done;
        }
    }
}