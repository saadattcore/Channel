using System;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SendReportEmail : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReportGenerator));

        public InputParameter<string> ReportEmailAddress { get; set; }
        public InputParameter<string> ApplicationId { get; set; }
        public InputParameter<string> UserId { get; set; }
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<ApplicationStatus> AppStatus { get; set; }



        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            Log.Debug($"App Status Received as {AppStatus.Get()} for application Id {ApplicationId.Get()}");

            if (AppStatus.Get() != ApplicationStatus.Paid)
                return WorkflowStepState.Done;

			return StepState = WorkflowStepState.Done;
        }
    }
}
