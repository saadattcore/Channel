using System;
using System.Collections.Generic;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Vision.Model;
using System.Collections;
using System.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id=7202DA5D666E47A1B5ACB7C02657ACFB
    public class ValidateSponsorWifesCount : ChannelWorkflowStep
    {
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<SponsorRelationship> Relationship { get; set; }
        public InputParameter<string> VisaType { get; set; }
        public InputParameter<string> PPSID { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(SponsorNo);
            CheckRequiredInput(UserType);
            CheckRequiredInput(Relationship);
            CheckRequiredInput(VisaType);
            bool isValidated = true;

            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            if (Relationship.Get() == SponsorRelationship.Wife &&
                VisaTypeId.Residence == (VisaTypeId)Enum.Parse(typeof(VisaTypeId), VisaType.Get()))
            {
                var numberOfWifes = await ServicesHelper.GetNumberOfWifes(SponsorNo.Get());

                //If entry permit and wife count is greater than 0 then don't allow
                if (string.IsNullOrEmpty(PPSID.Get()) && numberOfWifes?.Count > 0)
                    isValidated = false;
                else if (numberOfWifes?.Count > 0) //If PPSID is also there and wife count is greater than 0 - Renew Residence
                {
                    var oldestWife = numberOfWifes.Where(p => p.VisaType == Emaratech.Services.Channels.Contracts.Rest.Models.Enums.VisaType.Residence.GetAttribute<VisaTypeAttr>().Type).OrderBy(o => o.OrigIssueDate).FirstOrDefault();
                    if (oldestWife != null && oldestWife.PpsId != PPSID.Get()) //If a wife found on residence visa with minimum residence issue date and we are not renewing for that wife then don't allow
                        isValidated = false;
                }
            }

            if (!isValidated)
                throw ChannelWorkflowErrorCodes.InvalidNumberOfWifes.ToWebFault($"A resident can only sponsor one wife at a time");

            return StepState = WorkflowStepState.Done;
        }
    }
}