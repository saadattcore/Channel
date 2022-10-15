﻿using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Threading.Tasks;
using log4net;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateRenewResidenceSponsorInfo : ChannelWorkflowStep
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(ValidateData));
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

            Log.Debug($"Going to validate sponsor info for renew residence for sponsor no {SponsorNo.Get()}");

            var sponsorInfo = await ServicesHelper.GetIndividualDetailedInfoBySponsorNo(SponsorNo.Get());

            //Check for valid file no
            if (UserType.Get() == Constants.UserTypeLookup.CitizenUserType)
            {
                if (sponsorInfo.IndividualUserInfo.IndividualSponsorshipInformation == null ||
                    sponsorInfo.IndividualUserInfo.IndividualSponsorshipInformation.IsFileActive != 1)
                {
                    throw ChannelWorkflowErrorCodes.InvalidSponsorFileNumber.ToWebFault($"The sponsor file number is invalid");
                }
            }

            if (UserType.Get() == Constants.UserTypeLookup.ResidentUserType)
            {
                //Check for valid residency of sponsor
                if (sponsorInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceStatusId == (int) ResidenceStatus.Cancelled || sponsorInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceStatusId == (int) ResidenceStatus.PermanentClosed)
                    throw ChannelWorkflowErrorCodes.ResidencyNotValid.ToWebFault($"Workflow step failed because sponsor residence status is {sponsorInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceStatusId} for sponsor number {SponsorNo.Get()}");

                if ((sponsorInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceExpiryDate - DateTime.Now).Value.TotalDays < 90)
                    throw ChannelWorkflowErrorCodes.SponsorResidenceExpiry3Months.ToWebFault($"Workflow step failed because sponsor residence expiry less than 3 months, Expired on {Convert.ToString(sponsorInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceExpiryDate)} for sponsor number {SponsorNo.Get()}");
            }
            Log.Debug($"Sponsor Info validated for sponsor no {SponsorNo.Get()}");

            return StepState = WorkflowStepState.Done;
        }
    }
}