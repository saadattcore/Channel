using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step ID = 1D3A398F042842DB98F83B0C75ECBA41
    public class GetEntryPermitTravelStatus : GetTravelStatus
    {
        public InputParameter<string> PermitNo { get; set; }
        public InputParameter<string> EntryPermitPPSID { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug($"Going to get travel status for permit no {PermitNo.Get()?.ToString()}");
            await base.Execute();

            CheckRequiredInput(PermitNo);
            CheckRequiredInput(EntryPermitPPSID);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var travelInfo = await ServicesHelper.GetIndividualCurrentTravelStatusByPermitNo(PermitNo.Get()?.ToString());

            if (travelInfo == null)
                Log.Debug($"Travel status not found for permit no {PermitNo.Get()?.ToString()}");

            //If the record found then assign value if not found then it means person is outside country
            var travelType = travelInfo?.TravelTypeId ?? Convert.ToString((int)TravelType.Exit);

            SetIsInsideCountryNode(travelType);
            return StepState = WorkflowStepState.Done;
        }
    }
}