using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class WaitForPaymentStatus : ChannelWorkflowStep
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MarkPaymentStatus));
        public InputParameter<string> Request { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            LOG.Debug($"Waiting for payment status with request");

            await base.Execute();
            CheckRequiredInput(Request);
            if (ParametersRequiringInput.Count > 0)
            {
                LOG.Debug("Wait for Payment status input required");
                return StepState = WorkflowStepState.InputRequired;
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}