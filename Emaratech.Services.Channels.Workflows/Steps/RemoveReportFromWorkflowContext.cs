using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class RemoveReportFromWorkflowContext : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RemoveReportFromWorkflowContext));

        public ReferenceParameter<JObject> UnifiedApplication { get; set; }
        public ReferenceParameter<string> ReportData { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug("Going to remove report from workflow context");
            await base.Execute();

            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            UnifiedApplication.Value["ReportDetails"]["ReportData"] = null;
            ReportData.Value = null;
            Log.Debug("Report successfully removed from workflow context");

            StepState = WorkflowStepState.Done;
            return StepState;
        }
    }
}
