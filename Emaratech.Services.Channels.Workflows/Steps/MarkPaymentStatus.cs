using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class MarkPaymentStatus : ChannelWorkflowStep
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MarkPaymentStatus));

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> AuthenticatedSystemId { get; set; }
        public InputParameter<string> Request { get; set; }
        public InputParameter<string> ApplicationId { get; set; }
        public InputParameter<int?> ViolationAmount { get; set; }
        public InputParameter<string> PPSID { get; set; }
        public OutputParameter BatchId { get; set; }
        public OutputParameter Error { get; set; }
        public OutputParameter PaymentCompleteUrl { get; set; }
        public OutputParameter AppStatus { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            BatchId = new OutputParameter(nameof(BatchId));
            Error = new OutputParameter(nameof(Error));
            PaymentCompleteUrl = new OutputParameter(nameof(PaymentCompleteUrl));
            AppStatus = new OutputParameter(nameof(AppStatus));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            try
            {
                LOG.Debug("Executing Mark Payment Status");

                await base.Execute();

                LOG.Debug($"System ID is {Convert.ToString(SystemId.Get())} and Request is {Request.Get()} and application is {ApplicationId.Get()}");

                CheckRequiredInput(SystemId);
                CheckRequiredInput(UserType);
                CheckRequiredInput(Request);
                CheckRequiredInput(ApplicationId);

                if (ParametersRequiringInput.Count > 0)
                {
                    LOG.Debug("Input is required from workflow");
                    return StepState = WorkflowStepState.InputRequired;
                }

                LOG.Debug($"Required inputs verified with request {Request.Get()}");

                bool? isVirtualBankEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["Payments.IsVirtualBankEnabled"]);
                var merchant = await PaymentHelper.GetMerchant(UserType.Get());

                // Call payment api to mark success or failure
                var res = await ServicesHelper.MarkPaymentStatus(Request.Get(), AuthenticatedSystemId.Get(), merchant.MerchantId, isVirtualBankEnabled);
                BatchId.Set(res.BatchId);

                var appStatus = res.Status.GetValueOrDefault(false) ? ApplicationStatus.Paid : ApplicationStatus.PaymentFailed;
                AppStatus.Set(appStatus);

                LOG.Debug($"Batch ID is {res.BatchId} and App Status is {Convert.ToString(appStatus)}");
                // Call application api to update payment status
                string url;
                if (res.Status.GetValueOrDefault(false))
                {
                    LOG.Debug("Payment is successful");
                     await ServicesHelper.UpdateApplicationPaymentStatus(ApplicationId.Get(), res.BatchId, Convert.ToBoolean(isVirtualBankEnabled) ? (int) PaymentType.VirtualBank : (int) PaymentType.NoqodiExpress);

                    if (ViolationAmount != null && ViolationAmount.IsFilled())
                    {
                        await ServicesHelper.UpdateIndividualViolation(ViolationAmount.Get().Value, res.BatchId, PPSID.Get());
                    }
                    url = await ServicesHelper.GetSystemProperty(AuthenticatedSystemId.Get(), "Payments.MobileSuccessUrl");
                }
                else
                {
                    LOG.Debug("Payment is failed");
                     await ServicesHelper.UpdateApplicationFailedPaymentStatusAsync(ApplicationId.Get());
                    url = await ServicesHelper.GetSystemProperty(AuthenticatedSystemId.Get(), "Payments.MobileFailureUrl");
                }

                url = ConfigurationManager.AppSettings[url] ?? url;
                url = url.Replace(":application:", ApplicationId.Get());
                url = url.Replace(":reference:", res.BatchId);
                PaymentCompleteUrl.Set(url);

                LOG.Debug("Payment URL set");

                StepState = WorkflowStepState.Done;
                return StepState;
            }
            catch (Exception exp)
            {
                LOG.Error(exp);
                throw;
            }
        }
    }
}