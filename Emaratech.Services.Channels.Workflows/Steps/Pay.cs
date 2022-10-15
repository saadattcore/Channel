using Emaratech.Services.Application.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;
using log4net;
using RestSharp.Extensions.MonoHttp;
using Newtonsoft.Json;
using FeeConfiguration = Emaratech.Services.Channels.Workflows.Models.FeeConfiguration;
using Emaratech.Services.Payment.Model;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Helpers;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class Pay : ChannelWorkflowStep
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(Pay));

        public InputParameter<string> ApplicationId { get; set; }
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> AuthenticatedSystemId { get; set; }
        public InputParameter<long?> Amount { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> SponsorType { get; set; }
        public InputParameter<string> EstablishmentType { get; set; }
        public InputParameter<IList<FeeConfiguration>> Fees { get; set; }
        public InputParameter<int?> ViolationAmount { get; set; }

        public ReferenceParameter<string> PaymentUrl { get; set; }

        public WorkflowInstanceContext Context { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(ServiceId);
            CheckRequiredInput(AuthenticatedSystemId);
            CheckRequiredInput(UserType);
            CheckRequiredInput(Amount);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            bool? isVirtualBankEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["Payments.IsVirtualBankEnabled"]);

            var systemPaymentUrl = await GetSystemPaymentURL(isVirtualBankEnabled);
            LOG.Debug($"Payment url {systemPaymentUrl}");
            var systemRedirectUrl = await GetSystemRedirectURL();
            LOG.Debug($"Redirect url {systemRedirectUrl}");

            var merchant = await PaymentHelper.GetMerchant(UserType.Get());
            // Build the beneficiaries
            var beneficiaries = Fees.Get()?.SelectMany(x => x.Beneficiaries)
                .Select(x => new RestBeneficiary
                {
                    Account = merchant.Beneficiaries?.FirstOrDefault(b => b.Name.ToLower() == x.Desc.ToLower())?.Code,
                    Amount = x.Amount,
                    Code = merchant.Beneficiaries?.FirstOrDefault(b => b.Name.ToLower() == x.Desc.ToLower())?.Code,
                    Currency = "AED",
                    Desc = x.Desc
                });

            // Associate batchId to application
            var batchId = await ServicesHelper.GetPaymentBatchId();
            LOG.Debug($"Batch id: {batchId}");
            if (ApplicationId.IsFilled())
            {
                await ServicesHelper.UpdateApplicationBatchId(ApplicationId.Get(), batchId);
            }

            var paymentInfo = await ServicesHelper.Pay(batchId, merchant.MerchantId, Amount.Get().Value, AuthenticatedSystemId.Get(),
                systemRedirectUrl
                .Replace(":workflowToken:", Context.WorkflowInstanceId),
                beneficiaries,
                isVirtualBankEnabled
                );


            // Update the application fees
            var feeMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "FeeMatrix");
            if (string.IsNullOrEmpty(feeMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidFeeConfiguration.ToWebFault($"FeeMatrix not configured in system");
            }

            if (ApplicationId.IsFilled())
            {
                var appFees = new List<RestApplicationFee>();
                foreach (var fee in Fees.Get())
                {
                    appFees.Add(new RestApplicationFee
                    {
                        Amount = Convert.ToInt64(fee.Amount),
                        FeeType = fee.FeeTypeId,
                        FeeUnit = 1,
                        SerialNumber = Guid.NewGuid().ToString("N").ToUpper(),
                        TransactionBatchId = paymentInfo.BatchId,
                        TransType = null
                    });
                    LOG.Debug($"Fee: {fee.Amount}");
                }
                var rowsUpdated = await ServicesHelper.UpdateApplicationFees(ApplicationId.Get(), appFees);
                LOG.Debug($"{rowsUpdated} fees inserted for application: {ApplicationId.Get()}");
            }

            if (string.IsNullOrEmpty(PaymentUrl.Value))
            {
                var paymentUrl = $"{systemPaymentUrl}?__action={HttpUtility.UrlEncode(paymentInfo.ForwardURL)}&__method=post&request={HttpUtility.UrlEncode(paymentInfo.PostData)}";
                PaymentUrl.Value = paymentUrl;
                LOG.Debug($"Final payment url is  {paymentUrl}");
            }

            StepState = WorkflowStepState.Done;
            return StepState;
        }

        private async Task<string> GetSystemRedirectURL()
        {
            var redirectUrl = await
                ServicesHelper.GetSystemProperty(AuthenticatedSystemId.Get(),
                    Constants.SystemProperties.PaymentRedirectUrl);
            return ConfigurationManager.AppSettings[redirectUrl] ?? redirectUrl;

        }

        private async Task<string> GetSystemPaymentURL(bool? isVirtualBankEnabled)
        {
            string systemPaymentUrl;

            if (isVirtualBankEnabled == true)
                systemPaymentUrl = await ServicesHelper.GetSystemProperty(AuthenticatedSystemId.Get(), "Payments.VirtualBankUrl");
            else
                systemPaymentUrl = await ServicesHelper.GetSystemProperty(AuthenticatedSystemId.Get(), Constants.SystemProperties.PaymentURL);

            return ConfigurationManager.AppSettings[systemPaymentUrl] ?? systemPaymentUrl;
        }
    }
}