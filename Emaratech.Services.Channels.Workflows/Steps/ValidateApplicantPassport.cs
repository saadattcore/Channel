using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateApplicantPassport : ChannelWorkflowStep
    {
        public InputParameter<DateTime> PassportExpiryDate { get; set; }

        public InputParameter<string> ResidenceNo { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(PassportExpiryDate);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            if ((PassportExpiryDate.Get() - DateTime.Now).Days < 180)
                throw ChannelWorkflowErrorCodes.ResidentCancelFailed.ToWebFault($"Applicant passport is expiry on {PassportExpiryDate.Get()} for residence number {ResidenceNo?.Get()}");

            StepState = WorkflowStepState.Done;

            return StepState;
        }
    }
}