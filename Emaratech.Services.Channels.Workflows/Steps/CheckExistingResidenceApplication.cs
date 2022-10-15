using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using System.Xml;
using Emaratech.Services.Channels.Contracts.Errors;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id=F4B3DA7F7CC842CFAF867F3694FC59ED
    public class CheckExistingResidenceApplication : ChannelWorkflowStep
    {
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> ResidenceNo { get; set; }
        public InputParameter<string> PermitNo { get; set; }
        public InputParameter<string> ApplicationId { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(ServiceId);
            if (!PermitNo.IsFilled() && !ResidenceNo.IsFilled())
            {
                ParametersRequiringInput.Add(PermitNo.Name);
            }

            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            string visaNumber;

            if (!string.IsNullOrEmpty(PermitNo?.Get()))
                visaNumber = PermitNo?.Get();
            else if (!string.IsNullOrEmpty(ResidenceNo?.Get()))
                visaNumber = ResidenceNo?.Get();
            else
                throw ChannelErrorCodes.BadRequest.ToWebFault("Permit and residence no are not found");

            //var visaNumber = !string.IsNullOrEmpty(PermitNo?.Get()) ? PermitNo.Get() : ResidenceNo.Get();
            if (!ApplicationId.IsFilled() && await ServicesHelper.IsApplicationExistForStatuses(ServiceId.Get(), visaNumber))
            {
                throw ChannelWorkflowErrorCodes.ApplicationAlreadyExists.ToWebFault($"An application has already been submitted");
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}