using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateApplicantSponsored : ChannelWorkflowStep
    {
        public InputParameter<string> ResidenceNo { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(ResidenceNo);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var residenceInfo = await ServicesHelper.GetIndividualResidenceInfo(ResidenceNo.Get());

            if (residenceInfo.NumberOfDependents > 1)
                throw ChannelWorkflowErrorCodes.ResidentCancelFailedRelatedSponsored.ToWebFault($"Residence cancel fail because residence number {ResidenceNo.Get()} has {residenceInfo.NumberOfDependents} dependents under his name");

            return StepState = WorkflowStepState.Done;
        }
    }
}