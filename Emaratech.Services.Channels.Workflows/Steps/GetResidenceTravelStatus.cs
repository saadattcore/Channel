using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using log4net;
using System;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step ID = 725A3F3354E64956B82A77AFBD8DEC11
    public class GetResidenceTravelStatus : GetTravelStatus
    {
        public InputParameter<string> ResidenceNo { get; set; }
        public InputParameter<string> ResidencePPSID { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug($"Going to get residence travel status for residence No {ResidenceNo.Get()?.ToString()}");
            await base.Execute();

            CheckRequiredInput(ResidenceNo);
            CheckRequiredInput(ResidencePPSID);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            string travelType;
            var travelInfo = await ServicesHelper.GetIndividualCurrentTravelStatusByResNo(ResidenceNo.Get()?.ToString());

            if (travelInfo == null)
            {
                Log.Debug($"Travel status not found for res no {ResidenceNo.Get()?.ToString()} thats why marking it inside");
                travelType = Convert.ToString((int)TravelType.Entry);
            }
            else
                travelType = Convert.ToString(travelInfo.TravelTypeId);

            Log.Debug($"Going to set node of travel type in document for res No {ResidenceNo.Get()?.ToString()} with travel status {travelType}");
            SetIsInsideCountryNode(travelType);
            return StepState = WorkflowStepState.Done;
        }
    }
}