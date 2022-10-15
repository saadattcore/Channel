using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateSponsorFileNumber : ChannelWorkflowStep
    {
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> UserType { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(SponsorNo);
            CheckRequiredInput(UserType);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            if (UserType.Get() == Constants.UserTypeLookup.CitizenUserType)
            {
                var info = await ServicesHelper.GetIndividualDetailedInfoBySponsorNo(SponsorNo.Get());
                if (info.IndividualSponsorInfo.SponsorshipInfo == null || 
                    info.IndividualSponsorInfo.SponsorshipInfo.IsFileActive != 1)
                {
                    throw ChannelWorkflowErrorCodes.InvalidSponsorFileNumber.ToWebFault($"The sponsor file number is invalid");
                }
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}