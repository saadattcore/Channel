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
    public class ValidateResidenceStatus : ChannelWorkflowStep
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

            if (residenceInfo.ResidenceStatusId != (int) ResidenceStatus.Issued && residenceInfo.ResidenceStatusId != (int) ResidenceStatus.Renewed)
                throw ChannelWorkflowErrorCodes.ResidentCancelFailed.ToWebFault($"Residence cancel fail because residence status is " + residenceInfo.ResidenceStatusId);

            if (residenceInfo.ResidenceTypeId != (int) ResidenceType.Residence && residenceInfo.ResidenceTypeId != (int) ResidenceType.HouseMaid)
                throw ChannelWorkflowErrorCodes.ResidentCancelFailed.ToWebFault($"Residence cancel fail because residence type id is " + residenceInfo.ResidenceTypeId);

            return StepState = WorkflowStepState.Done;
        }
    }
}