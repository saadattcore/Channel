using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;
using System.Collections.Generic;
using Emaratech.Services.Fee.Model;
using Emaratech.Services.MappingMatrix.Model;
using log4net;
using FeeConfiguration = Emaratech.Services.Channels.Workflows.Models.FeeConfiguration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveFees : ChannelWorkflowStep
    {

        private static readonly ILog LOG = LogManager.GetLogger(typeof(ResolveFees));

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> SponsorType { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> SponsorSponsorType { get; set; }
        public InputParameter<string> EstablishmentType { get; set; }
        public InputParameter<string> EstablishmentCode { get; set; }
        public InputParameter<int?> ViolationAmount { get; set; }
        public InputParameter<IList<FeeConfiguration>> WorkflowFee { get; set; }
        public InputParameter<string> JobId { get; set; }

        public OutputParameter Fees { get; set; }
        public OutputParameter Amount { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Fees = new OutputParameter(nameof(Fees));
            Amount = new OutputParameter(nameof(Amount));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(ServiceId);
            CheckRequiredInput(JobId);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var feeMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "FeeMatrix");
            if (string.IsNullOrEmpty(feeMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidFeeConfiguration.ToWebFault($"FeeMatrix not configured in system");
            }

            var sponsorEstCode = EstablishmentCode.Get();

            var feesParams = await GetFeeParameters(SystemId?.Get(), UnifiedApplication?.Get(),
                ServiceId?.Get(), UserType?.Get(), SponsorSponsorType?.Get(), EstablishmentType?.Get(),
                feeMappingMatrix, GetType(), JobId?.Get(), sponsorEstCode);

            var fees = (await ServicesHelper.GetFees(feeMappingMatrix, feesParams)).Select(
                x => new FeeConfiguration()
                {
                    Amount = x.Amount,
                    FeeTypeId = x.FeeTypeId,
                    Name = x.Name,
                    ResourceKey = x.ResourceKey
                }).ToList();

            if (ViolationAmount != null && ViolationAmount.IsFilled())
            {
                fees.Add(ValidateViolationInfo.GetViolationFee(ViolationAmount.Get().Value));
            }

            if (WorkflowFee?.Get() != null)
            {
                fees = fees.Concat(WorkflowFee.Get())
                    .ToList();
            }
            var beneficiaryMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "Configuration.FeeBeneficiary");
            if (string.IsNullOrEmpty(beneficiaryMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidFeeConfiguration.ToWebFault($"Beneficiary Matrix not configured in system");
            }

            var tasks =
                fees.Select(
                    fee => GetBeneficiaryParameters(feesParams, fee, beneficiaryMappingMatrix));
            Task.WaitAll(tasks.ToArray());

            Fees.Set(fees);

            long amount = 0;
            if (fees.Any())
            {
                amount = fees.Select(f => Convert.ToInt64(f.Amount)).Sum();
            }
            Amount.Set(amount);

            StepState = WorkflowStepState.Done;

            return StepState;
        }

        public static async Task GetBeneficiaryParameters(IList<FeeParameter> feeParameters, FeeConfiguration feeConfiguration, string mappingMatrixId)
        {
            var parameters = feeParameters.Select(x => new SearchVersion(x.Name, x.Value))
                .ToList();

            if (!parameters.Any(x => x.Name == "FeeType"))
            {
                parameters.Add(new SearchVersion("FeeType", feeConfiguration.FeeTypeId));
            }
            else
            {
                parameters.First(x => x.Name == "FeeType").Value = feeConfiguration.FeeTypeId;
            }
            parameters.Add(new SearchVersion("FeeAmount", feeConfiguration.Amount));

            var beneficiaries = (await ServicesHelper.ResolveMappingMatrix(mappingMatrixId, parameters,
                (x, y) => new BeneficiaryConfiguration() { Account = x[3], Amount = x[2], Code = x[1], Desc = x[4] }, true)).ToList();

            feeConfiguration.Beneficiaries = beneficiaries;

        }
        public static async Task<List<FeeParameter>> GetFeeParameters(string systemId, JObject applicationData, string serviceId, string userType, string sponsorType, string establishmentType, string feeMatrix, Type stepType, string jobId, string establishmentCode)
        {
            string isInside = applicationData?["ApplicantDetails"]?["IsInsideUAE"]?.ToString();

            var feesParams = new List<FeeParameter>()
            {
                new FeeParameter("Service", serviceId),
                new FeeParameter(nameof(UserType), userType),
                new FeeParameter(nameof(SponsorType), sponsorType),
                new FeeParameter(nameof(EstablishmentType), establishmentType),
                new FeeParameter("EstablishmentCode", establishmentCode),
                new FeeParameter("ProfessionId", jobId)
            };
            var parameters = await DataHelper.GetMatrixParameters(applicationData, feeMatrix, systemId);
            foreach (var kvp in parameters)
            {
                if (feesParams.All(f => f.Name != kvp.Key))
                {
                    if (kvp.Key == "Urgent")
                    {
                        feesParams.Add(new FeeParameter(kvp.Key, kvp.Value == "1" ? "Y" : "N"));
                        feesParams.Add(new FeeParameter("Delivery", kvp.Value == "0" ? "Y" : "N"));
                    }
                    else if (!stepType.GetProperties().Any(p => p.Name == kvp.Key) && !feesParams.Exists(f => f.Name == kvp.Key))
                    {
                        // If there is any property with the same key, ignore this parameter
                        // Input param should be hardcoded and output param is the mapping matrix output, not a search criteria
                        feesParams.Add(new FeeParameter(kvp.Key, kvp.Value));
                    }
                }
            }
            if (feesParams.FirstOrDefault(x => x.Name == "InsideOutside") == null)
            {
                feesParams.Add(new FeeParameter("InsideOutside", isInside));
            }
            return feesParams;
        }
    }
}