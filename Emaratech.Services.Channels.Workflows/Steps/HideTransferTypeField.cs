using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Workflows.Engine;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class HideTransferTypeField:ChannelWorkflowStep
    {
        public ReferenceParameter<RestRenderGraph> FormConfiguration { get; set; }

        public override Task<WorkflowStepState> Execute()
        {
            DataHelper.RemoveField(FormConfiguration.Value.VerticalItems, "ApplicationDetails", "ServiceId");
            return base.Execute();
        }
    }
}
