using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidatePermitInsideCountry : ChannelWorkflowStep
    {
        public InputParameter<string> PermitNo { get; set; }
        public InputParameter<string> PPSID { get; set; }
        protected static readonly ILog Log = LogManager.GetLogger(typeof(GetTravelStatus));

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug("Executing validate permit inside country step");
            await base.Execute();
            CheckRequiredInput(PermitNo);
            CheckRequiredInput(PPSID);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            Log.Debug($"Validating Inside country for permit by ppsID {PPSID.Get()}");
            var travelInfo = await ServicesHelper.GetIndividualCurrentTravelStatusByPpsId(PPSID.Get());

            if (travelInfo == null || Convert.ToInt32(travelInfo.TravelTypeId) == (int)TravelType.Exit)
            {
                Log.Debug($"Person is not inside country with permit no {PermitNo.Get()} and pps ID {PPSID.Get()}");
                throw ChannelWorkflowErrorCodes.PersonOutsideCountry.ToWebFault($"Applicant with Permit No {PermitNo.Get()} should be inside the country.");
            }

            return StepState = WorkflowStepState.Done;
        }
    }
}
