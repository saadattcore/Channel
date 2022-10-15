using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetDocumentView : ChannelWorkflowStep
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(SetDocumentView));
        public OutputParameter View { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            LOG.Debug("Setting documents view");
            await base.Execute();
            View.Set(ViewEnum.Documents);
            return StepState = WorkflowStepState.Done;
        }
    }
}
