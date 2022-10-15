using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class GetPassportRenewDependantsButton : ChannelWorkflowStep
    {
        public InputParameter<JArray> PassportApplication { get; set; }

        public OutputParameter AdditionalButtons { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            AdditionalButtons = new OutputParameter(nameof(AdditionalButtons));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            var buttons = GetButtons(PassportApplication.Get());
            if (buttons != null)
            {
                AdditionalButtons.Set(buttons);
            }
            return StepState = WorkflowStepState.Done;
        }

        public static IEnumerable<RestButton> GetButtons(JArray passportEntries)
        {
            if (passportEntries == null || passportEntries.Count < 7)
            {
                return new List<RestButton>
                {
                    new RestButton { Action = "new", ResourceKey = "NewApplication" }
                };
            }
            return null;
        }
    }
}