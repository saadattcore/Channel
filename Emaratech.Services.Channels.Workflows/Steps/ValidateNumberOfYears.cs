using System;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using log4net;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id = 95E5612F11DE4C80B05E264E1C61A837
    public class ValidateNumberOfYears : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidateNumberOfYears));
        public InputParameter<string> SponsorNo { get; set; }

        public InputParameter<string> YearsOfResidence { get; set; }

        public InputParameter<string> UserType { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug("Going to validate number of years");

            await base.Execute();

            if (UserType.Get() == Constants.UserTypeLookup.ResidentUserType)
            {
                CheckRequiredInput(SponsorNo);
                CheckRequiredInput(YearsOfResidence);

                if (ParametersRequiringInput.Count > 0)
                {
                    return StepState = WorkflowStepState.InputRequired;
                }

                var residenceInfo = await ServicesHelper.GetIndividualResidenceInfoBySponsorNo(SponsorNo.Get());

                int sponsorNumberOfYears = Convert.ToInt32(residenceInfo.ResidenceExpiryDate?.Year - residenceInfo.ResidenceIssueDate?.Year);
                Log.Debug($"Sponsor Number of years are {sponsorNumberOfYears}");

                if (Convert.ToInt32(YearsOfResidence.Get()) > sponsorNumberOfYears)
                    throw ChannelWorkflowErrorCodes.InvalidNumberOfYears.ToWebFault($"Number of years {YearsOfResidence.Get()} are greater than sponsor number of years {sponsorNumberOfYears}");
            }

            return StepState = WorkflowStepState.Done;
        }
    }
}