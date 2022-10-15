using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using System.Xml;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class CheckPermitApplicationExistence : ChannelWorkflowStep
    {
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> PermitNo { get; set; }


        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(ServiceId);
            CheckRequiredInput(PermitNo);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            if (await ServicesHelper.IsApplicationExist(ServiceId.Get(), PermitNo.Get()))
            {
                throw ChannelWorkflowErrorCodes.ApplicationAlreadyExists.ToWebFault($"An application has already been submitted");
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}