using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class FilterRelationships : ChannelWorkflowStep
    {
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> Platform { get; set; }

        public InputParameter<string> ServiceId { get; set; }

        public InputParameter<RestRenderGraph> FormConfiguration { get; set; }

        public OutputParameter UpdatedFormConfiguration { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            UpdatedFormConfiguration = new OutputParameter(nameof(UpdatedFormConfiguration));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(FormConfiguration);
            CheckRequiredInput(UserType);
            CheckRequiredInput(Platform);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            DataHelper.FilterLookup("SponsorRelationId", GetAllowedRelationships(), FormConfiguration.Get());

            UpdatedFormConfiguration.Set(FormConfiguration.Get());
            return StepState = WorkflowStepState.Done;
        }

        private IEnumerable<string> GetAllowedRelationships()
        {
            var relations = new List<SponsorRelationship>
            {
                SponsorRelationship.Husband,
                SponsorRelationship.Wife,
                SponsorRelationship.Son,
                SponsorRelationship.Daughter,
                SponsorRelationship.Father,
                SponsorRelationship.Mother,
                SponsorRelationship.FatherInLaw,
                SponsorRelationship.MotherInLaw,
                SponsorRelationship.Brother,
                SponsorRelationship.Sister
            };

            if (UserType.Get() == Constants.UserTypeLookup.CitizenUserType)
            {
                relations.Add(SponsorRelationship.SisterInLaw);
                relations.Add(SponsorRelationship.BrotherInLaw);
            }

            //Remove sponsor relations from web
            if (Platform.Get() == Constants.Platforms.WebPlatform && !string.IsNullOrEmpty(ServiceId.Get()))
            {
                if (ServiceId.Get() == Constants.Services.EntryPermitNewResidenceService)
                {
                    relations.Remove(SponsorRelationship.SisterInLaw);
                    relations.Remove(SponsorRelationship.BrotherInLaw);

                    if (UserType.Get() == Constants.UserTypeLookup.ResidentUserType)
                    {
                        relations.Remove(SponsorRelationship.Father);
                        relations.Remove(SponsorRelationship.Mother);
                        relations.Remove(SponsorRelationship.FatherInLaw);
                        relations.Remove(SponsorRelationship.MotherInLaw);
                        relations.Remove(SponsorRelationship.Brother);
                        relations.Remove(SponsorRelationship.Sister);
                    }
                }
            }

            return relations.Select(s => Convert.ToInt32(s).ToString());
        }
    }
}