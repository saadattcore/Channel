using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class CheckExistingEntryPermitApplication : ChannelWorkflowStep
    {
        //Step Id = 40A417647D534B3AA69B89ECF499143A
        public InputParameter<string> ServiceId { get; set; }
        public ReferenceParameter<string> ApplicationData { get; set; }
        public InputParameter<string> ApplicationId { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(ServiceId);
            CheckRequiredInput(ApplicationData);

            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            var formData = JObject.Parse(ApplicationData.Value);
            var passportNumber = formData["Application"]["ApplicantDetails"]["PassportNo"].Value<string>();
            var nationalityId = formData["Application"]["ApplicantDetails"]["CurrentNationalityId"].Value<string>();
            if (!ApplicationId.IsFilled() && await ServicesHelper.IsApplicationExistForStatuses(GetAllowedServices(), passportNumber, nationalityId))
            {
                throw ChannelWorkflowErrorCodes.ApplicationAlreadyExists.ToWebFault($"An application has already been submitted");
            }
            return StepState = WorkflowStepState.Done;
        }

        private List<string> GetAllowedServices()
        {
            return new List<string>
            {
                Constants.Services.EntryPermitNewResidenceService,
                Constants.Services.EntryPermitNewLeisureShortSingleService,
                Constants.Services.EntryPermitNewLeisureShortMultiService,
                Constants.Services.EntryPermitNewLeisureLongSingleService,
                Constants.Services.EntryPermitNewLeisureLongMultiService,
                Constants.Services.EntryPermitCancelPrivateService
            };
        }
    }
}