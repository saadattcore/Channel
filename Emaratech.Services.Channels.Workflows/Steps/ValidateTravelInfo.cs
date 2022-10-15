using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidateTravelInfo : ChannelWorkflowStep
    {
        public InputParameter<string> PPSID { get; set; }
        public InputParameter<string> ResidenceNo { get; set; }

        public OutputParameter CancellationReason { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            CancellationReason = new OutputParameter(nameof(CancellationReason));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(PPSID);
            CheckRequiredInput(ResidenceNo);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var travelInfo = await ServicesHelper.GetIndividualCurrentTravelStatusByPpsId(PPSID.Get());
            //if (travelInfo == null)
            //{
            //    throw ChannelWorkflowErrorCodes.ResidentCancelFailed.ToWebFault($"Residence cancel fail because travel info not found for {ResidenceNo.Get()}");
            //}

            //var residenceInfo = await ServicesHelper.GetIndividualResidenceInfo(ResidenceNo.Get());


            //If travel info is null then it means resident is inside the country
            CancellationReason.Set(WorkflowConstants.CancellationReasonEntry);

            if (travelInfo != null)
            {
                if (Convert.ToInt32(travelInfo.TravelTypeId) == (int)TravelType.Exit)
                {
                    CancellationReason.Set(WorkflowConstants.CancellationReasonExit);

                    if ((DateTime.Now - travelInfo.TravelDate).Value.TotalDays > 180)
                        throw ChannelWorkflowErrorCodes.ResidenceCancelFailPersonOutsideCountry.ToWebFault($"Residence cancel failed because travel date is {travelInfo.TravelDate} for residence number {ResidenceNo.Get()}");

                    //if ((residenceInfo.ResidenceExpiryDate - DateTime.Now).Value.Days < 180)
                    //    throw ChannelWorkflowErrorCodes.ResidenceCancelFailPersonOutsideCountry.ToWebFault($"Residence cancel failed because resident expiry date is {residenceInfo.ResidenceExpiryDate} for residence number {ResidenceNo.Get()}");
                }
                else
                    throw ChannelWorkflowErrorCodes.ResidentCancelFailed.ToWebFault($"Residence cancel fail because travel date not found for {ResidenceNo.Get()}");
            }
            return StepState = WorkflowStepState.Done;
        }
    }
}