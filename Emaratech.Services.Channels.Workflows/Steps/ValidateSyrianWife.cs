using System;
using System.Collections.Generic;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Common.Entities;
using Emaratech.Services.Vision.Model;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id=BFB5DF38C0BF4A4FBB0C60CC5811FF25
    public class ValidateSyrianWife : ChannelWorkflowStep
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(ValidateData));
        public InputParameter<SponsorRelationship> Relationship { get; set; }
        public InputParameter<string> VisaType { get; set; }
        public InputParameter<string> CurrentNationality { get; set; }
        public InputParameter<string> UserType { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(Relationship);
            CheckRequiredInput(VisaType);
            CheckRequiredInput(CurrentNationality);
            CheckRequiredInput(UserType);

            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            if (Relationship.Get() == SponsorRelationship.Wife &&
                UserType.Get() == Constants.UserTypeLookup.ResidentUserType &&
                VisaTypeId.Residence == (VisaTypeId)Enum.Parse(typeof(VisaTypeId), VisaType.Get())
                && CurrentNationality.Get() == Convert.ToString((int)Nationality.Syria))
            {
                Log.Debug("Nationality of wife is syria.");
                throw ChannelWorkflowErrorCodes.WifeNationalitySyria.ToWebFault("Wife nationality is syria which is not allowed to create residence");
            }

            return WorkflowStepState.Done;
        }
    }
}