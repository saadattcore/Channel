using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Contracts.Errors;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateRsaAuthentication : ChannelWorkflowStep
    {
        public InputParameter<string> RSADeviceId { get; set; }
        public InputParameter<string> Action { get; set; }
        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(RSADeviceId);
            CheckRequiredInput(Action);
            if (RSADeviceId != null && !string.IsNullOrEmpty(RSADeviceId.Get()))
            {
                if (ParametersRequiringInput.Count > 0)
                {
                    return StepState = WorkflowStepState.InputRequired;
                }

                var rsaTokenObject = JObject.Parse(Action.Get());
                if (string.IsNullOrEmpty(RSADeviceId.Get()))
                    return StepState = WorkflowStepState.Done;
                else
                {
                    //var payload = JObject.Parse(rsaTokenObject["payload"].ToString());
                    var status = await EdnrdHelper.Authenticate(new RSAAuthenticationData
                    {
                        ActionType = rsaTokenObject["actionType"].ToString(),
                        DeviceId = RSADeviceId.Get(),
                        OldToken = rsaTokenObject["oldToken"].ToString(),
                        Pin = rsaTokenObject["pin"].ToString(),
                        Token = rsaTokenObject["token"].ToString()
                    });

                    if (status != null && status.StatusId != "0")//ACCESS OK
                    {
                        if(status.StatusId == "1")
                            throw ChannelWorkflowErrorCodes.RsaAccessDenied.ToWebFault("Rsa Status : " + status.StatusId);
                        else if (status.StatusId == "2")
                            throw ChannelWorkflowErrorCodes.RsaNextCodeRequired.ToWebFault("Rsa Status : " + status.StatusId);
                        else if (status.StatusId == "4")
                            throw ChannelWorkflowErrorCodes.RsaNextCodeBad.ToWebFault("Rsa Status : " + status.StatusId);
                        else if(status.StatusId == "5")
                            throw ChannelWorkflowErrorCodes.RsaNewPinRequired.ToWebFault("Rsa Status : " + status.StatusId);
                        else if(status.StatusId == "6")
                            throw ChannelWorkflowErrorCodes.RsaPinAccepted.ToWebFault("Rsa Status : " + status.StatusId);
                        else if(status.StatusId == "7")
                            throw ChannelWorkflowErrorCodes.RsaPinRejected.ToWebFault("Rsa Status : " + status.StatusId);
                        else
                            throw ChannelErrorCodes.BadRequest.ToWebFault("Rsa Status : " + status.StatusId);
                    }
                }
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}