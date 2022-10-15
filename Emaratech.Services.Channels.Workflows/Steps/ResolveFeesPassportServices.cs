using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Fee.Model;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Contracts.Errors;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveFeesPassportServices : ChannelWorkflowStep
    {
        public InputParameter<JArray> PassportApplication { get; set; }
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<JObject> FeeDetails { get; set; }

        public OutputParameter Fees { get; set; }
        public OutputParameter Amount { get; set; }
        public OutputParameter PaymentUrl { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Fees = new OutputParameter(nameof(Fees));
            Amount = new OutputParameter(nameof(Amount));
            PaymentUrl = new OutputParameter(nameof(PaymentUrl));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(UserType);
            CheckRequiredInput(FeeDetails);
            CheckRequiredInput(PassportApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var fees = new List<Models.FeeConfiguration>();
            var amount = 0;
            JArray feeArray = (JArray)FeeDetails.Get()["feeDetails"];
            foreach (var fee in feeArray)
            {
                var isZajelDelivery = (int)PassportApplication.Get().First["procModeId"]["pmodeId"] == 1;
                var feeType = GetFeeTypeId(fee["mfeeId"].ToString());
                if (fee["active"].ToString() == "Y" && 
                    (feeType != Constants.Fees.DeliveryFeeId || isZajelDelivery))
                {
                    amount += Convert.ToInt32(fee["feeAmount"]);
                    fees.Add(new Models.FeeConfiguration
                    {
                        Amount = fee["feeAmount"].ToString(),
                        FeeTypeId = feeType,
                        ResourceKey = GetFeeResourceKey(fee["mfeeId"].ToString()),
                        Name = fee["feeDescEn"].ToString()
                    });
                }
            }

            var beneficiaryMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "Configuration.FeeBeneficiary");
            if (string.IsNullOrEmpty(beneficiaryMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidFeeConfiguration.ToWebFault($"Beneficiary Matrix not configured in system");
            }

            var feesParams = new List<FeeParameter>()
            {
                new FeeParameter(nameof(UserType), UserType.Get())
            };

            var tasks =
                fees.Select(
                    fee => ResolveFees.GetBeneficiaryParameters(feesParams, fee, beneficiaryMappingMatrix));
            Task.WaitAll(tasks.ToArray());

            Fees.Set(fees);
            Amount.Set(amount);
            PaymentUrl.Set(FeeDetails.Get()["paymentUrl"]);

            return StepState = WorkflowStepState.Done;
        }
        
        private static string GetFeeResourceKey(string feeType)
        {
            switch (feeType)
            {
                case "4":
                case "44":
                case "10":
                    return "DeliveryFee";
                case "5":
                case "46":
                case "12":
                    return "ApplicationFee";
                case "6":
                case "45":
                case "11":
                    return "IssueFees";
            }
            throw ChannelErrorCodes.InvalidSmartAmerFee.ToWebFault($"No fee mapping found for type {feeType}");
        }

        private static string GetFeeTypeId(string feeType)
        {
            switch (feeType)
            {
                case "4":
                case "44":
                    return Constants.Fees.DeliveryFeeId;
                case "5":
                case "46":
                    return Constants.Fees.ApplicationFeeId;
                case "6":
                case "45":
                    return Constants.Fees.IssuanceFeeId;
            }
            return null;
        }
    }
}