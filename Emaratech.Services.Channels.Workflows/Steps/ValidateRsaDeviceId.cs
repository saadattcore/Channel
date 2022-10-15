using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateRsaDeviceId : ChannelWorkflowStep
    {
        public InputParameter<string> UserId { get; set; }
        public OutputParameter RSADeviceId { get; set; }
        public override void Initialize()
        {
            base.Initialize();
            RSADeviceId = new OutputParameter(nameof(RSADeviceId));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(UserId);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }


            var userName = ApiFactory.Default.GetUserApi().GetUser(UserId.Get()).Username;

            //var userName = ClaimUtil.GetAuthenticatedUserName();
            var deviceId = await ServicesHelper.GetRSADeviceId(userName);
            RSADeviceId.Set(deviceId);

            return StepState = WorkflowStepState.Done;
        }
    }
}