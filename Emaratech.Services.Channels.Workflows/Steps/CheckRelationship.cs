using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class CheckRelationship : ChannelWorkflowStep
    {
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<SponsorRelationship> Relationship { get; set; }
        public InputParameter<string> ApplicationData { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<Gender?> GenderId { get; set; }
        public InputParameter<string> VisaType { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(SystemId);
            CheckRequiredInput(SponsorNo);
            CheckRequiredInput(Relationship);
            CheckRequiredInput(VisaType);
            CheckRequiredInput(ApplicationData);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }


            var gender = GenderId.Get();
            if (!gender.HasValue || 
                (gender.Value == Gender.Male && Relationship.Get() == SponsorRelationship.Husband) ||
                (gender.Value == Gender.Female && Relationship.Get() == SponsorRelationship.Wife))
            {
                throw ChannelWorkflowErrorCodes.InvalidSponsoredGender.ToWebFault("The gender of the sponsored is invalid");
            }

            var formData = JObject.Parse(ApplicationData.Get());
            var maritalStatus = await DataHelper.GetFieldValue((JObject)formData["Application"], SystemId.Get(), "MaritalStatus");

            // If daughter relation and already married throw error
            if (maritalStatus == "2" && Relationship.Get() == SponsorRelationship.Daughter &&
                VisaTypeId.Residence == (VisaTypeId)Enum.Parse(typeof(VisaTypeId), VisaType.Get()))
            {
                throw ChannelWorkflowErrorCodes.InvalidDaughterMaritalStatus.ToWebFault("A married daughter cannot be sponsored");
            }
            return StepState = WorkflowStepState.Done;
        }

    }
}