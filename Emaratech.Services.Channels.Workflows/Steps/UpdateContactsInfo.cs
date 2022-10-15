using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class UpdateContactsInfo : ChannelWorkflowStep
    {
        public InputParameter<string> PermitNo { get; set; }
        public OutputParameter MobileNumber { get; set; }
        public OutputParameter Email { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            MobileNumber = new OutputParameter(nameof(MobileNumber));
            Email = new OutputParameter(nameof(Email));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            RestIndividualUserFormInfo visionData = null;
            visionData = await ServicesHelper.GetIndividualProfileByPermitNo(PermitNo.Get());
            if (visionData == null)
            {
                throw ChannelErrorCodes.ProfileNotFound.ToWebFault($"Profile not found for PermitNo {PermitNo.Get()}");
            }

            var email = string.Empty;
            var mobile = string.Empty;

            foreach (var contact in visionData?.IndividualUserInfo?.IndividualContactDetails)
            {
                if (contact.ContactTypeId == ContactType.Email.GetHashCode())
                {
                    email = contact.CONTACTDETAIL;
                }
                else if (contact.ContactTypeId == ContactType.Mobile.GetHashCode())
                {
                    mobile = contact.CONTACTDETAIL;
                }
            }

            Email.Set(email);
            MobileNumber.Set(mobile);

            return StepState = WorkflowStepState.Done;
        }
    }
}