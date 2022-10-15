using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Workflows.Engine;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Vision.Model;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ValidationViolationPaymentHistory : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidationViolationPaymentHistory));

        public InputParameter<string> ResidenceNo { get; set; }
        public InputParameter<string> PermitNo { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug($"Starting Validate Violation Payment History Step ResidenceNo: {ResidenceNo.Get()} - PermitNo: {PermitNo.Get()}");

            int? violationPaymentCount = 0;

            if (!string.IsNullOrEmpty(PermitNo?.Get()))
                violationPaymentCount = await ServicesHelper.GetIndividualViolationPaymentHistoryByPermitNo(PermitNo.Get());
            else if (!string.IsNullOrEmpty(ResidenceNo?.Get()))
                violationPaymentCount = await ServicesHelper.GetIndividualViolationPaymentHistoryByResNo(ResidenceNo.Get());
            else
                throw ChannelWorkflowErrorCodes.BadRequest.ToWebFault("Residence and permit no not found in violation payment history step");

            if (Convert.ToInt32(violationPaymentCount) > 0)
            {
                Log.Debug($"Violation payment history found with count {violationPaymentCount}");
                throw ChannelWorkflowErrorCodes.ViolationPaymentHistoryFound.ToWebFault("Violation payment history found for this dependent");
            }

            StepState = WorkflowStepState.Done;
            return StepState;
        }
    }
}