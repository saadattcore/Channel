using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Steps.SendEmailNs;
using Emaratech.Services.Workflows.Engine;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SendEmail : ChannelWorkflowStep
    {
        public InputParameter<EmailHeader> EmailHeader { get; set; }
        public InputParameter<string> EmailContent { get; set; }


        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            var emailDataValue = EmailHeader.Get();
            
            foreach (var destination in EmailHeader.Get().DestinationList)
            {
                //var res = ServicesHelper.PushEmail(EmailContent.Get(), emailDataValue.Subject, destination.Address, destination.Name).Result;
            }

            return StepState = WorkflowStepState.Done;
        }
    }
}