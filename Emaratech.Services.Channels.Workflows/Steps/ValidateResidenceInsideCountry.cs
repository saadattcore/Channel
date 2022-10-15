using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //Step Id = E6025F49645046A2B5CBC0E2F5759931
    public class ValidateResidenceInsideCountry : ChannelWorkflowStep
    {
        public InputParameter<string> ResidenceNo { get; set; }
        protected static readonly ILog Log = LogManager.GetLogger(typeof(GetTravelStatus));

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug("Executing validate residence inside country step");
            await base.Execute();
            CheckRequiredInput(ResidenceNo);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            Log.Debug($"Validating Inside country for residence no {ResidenceNo.Get()}");

            var travelInfo = await ServicesHelper.GetIndividualCurrentTravelStatusByResNo(ResidenceNo.Get());

            //If not found then it means person is inside the country
            if (Convert.ToInt32(travelInfo?.TravelTypeId ?? "0") == (int)TravelType.Exit)
            {
                Log.Debug($"Person is not inside country with residence no {ResidenceNo.Get()}");
                throw ChannelWorkflowErrorCodes.PersonOutsideCountry.ToWebFault($"Applicant with residence number {ResidenceNo.Get()} should be inside the country");
            }

            return StepState = WorkflowStepState.Done;
        }
    }
}