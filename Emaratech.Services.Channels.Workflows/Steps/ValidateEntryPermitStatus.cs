using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateEntryPermitStatus : ChannelWorkflowStep
    {
        public InputParameter<string> PermitNo { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(PermitNo);

            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var permitInfo = await ServicesHelper.GetIndividualProfileByPermitNo(PermitNo.Get());

            if (permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitStatusId != PermitStatusType.Used.GetHashCode())
                throw ChannelWorkflowErrorCodes.InvalidApplicationData.ToWebFault($"Entry Permit fail because entry permit status is " + permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitStatusId + " (Not Used) ");

            return StepState = WorkflowStepState.Done;
        }
    }
}