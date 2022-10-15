using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class CheckViolation : ChannelWorkflowStep
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

            var violationInfo = await ServicesHelper.GetIndividualViolationByPermitNo(PermitNo.Get());

            if (violationInfo != null && short.Parse(violationInfo.OverStayDays) > 0)
                throw ChannelErrorCodes.individualUserProfileIsViolated.ToWebFault($"Extend On Arrival Visa fail because this permit has a violation, PermitNo : " + PermitNo.Get());

            return StepState = WorkflowStepState.Done;
        }
    }
}