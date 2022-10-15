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
    public class ValidateViolationInfo : ChannelWorkflowStep
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ValidateViolationInfo));

        public InputParameter<string> ResidenceNo { get; set; }
        public InputParameter<string> PermitNo { get; set; }

        public OutputParameter NotificationMessage { get; set; }
        public OutputParameter ViolationAmount { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            NotificationMessage = new OutputParameter(nameof(NotificationMessage));
            ViolationAmount = new OutputParameter(nameof(ViolationAmount));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            try
            {
                await base.Execute();
                LOG.Debug($"Starting Validate Violation Step ResidenceNo: {ResidenceNo.Get()} - PermitNo: {PermitNo.Get()}");

                RestIndividualViolationInfo violationInfo = null;

                if (!string.IsNullOrEmpty(PermitNo?.Get()))
                    violationInfo = await ServicesHelper.GetIndividualViolationByPermitNo(PermitNo.Get());
                else if (!string.IsNullOrEmpty(ResidenceNo?.Get()))
                    violationInfo = await ServicesHelper.GetIndividualViolationByResNo(ResidenceNo.Get());
                else
                    throw ChannelWorkflowErrorCodes.BadRequest.ToWebFault("Residence and permit no not found in violation step");

                if (violationInfo != null && (violationInfo.LocalAmount > 0))
                {
                    ViolationAmount.Set(violationInfo.LocalAmount);
                    NotificationMessage.Set("OverstayViolationFound");
                    LOG.Debug($"Violation found - local amount {violationInfo.LocalAmount}");
                }

                StepState = WorkflowStepState.Done;

                return StepState;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                throw;
            }
        }

        public static FeeConfiguration GetViolationFee(int amount)
        {
            return new FeeConfiguration
            {
                Amount = amount.ToString(),
                Name = "Violation",
                FeeTypeId = "9771D313C76B4E4CBB86EAB4847019B3",
                ResourceKey = "ViolationExemptionRequestFee"
            };
        }
    }
}
