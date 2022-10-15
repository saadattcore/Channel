using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.App_Start;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Payment.Model;
using Emaratech.Services.Channels.Models.Enums;
using log4net;
using Newtonsoft.Json;
using System.Configuration;
//using ApplicationStatus = Emaratech.Services.Application.Model.RestApplication.ApplicationStatusEnum;

namespace Emaratech.Services.Channels.BusinessLogic.Refund
{
    public class ApplicationRefund : RefundBase, IRefund
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationRefund));

        public ApplicationRefund(string userId, string userType, string systemId) : base(userId, userType, systemId)
        {

        }

        public async Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplications()
        {
            var searchCriteria = GetApplicationRefundSearchParmas(base.UserId, ConfigurationRepository.GetApplicationRefundStatuses());

            var searchResult = await ApplicationHelper.GetApplicationSearchResult(searchCriteria);
            IList<RestRefundableApplication> refundableApplications = searchResult.Select(base.GetRefundObject).ToList();

            foreach (var application in refundableApplications.Reverse<RestRefundableApplication>())
            {
                var feeDetails = (await ApiFactory.Default.GetApplicationApi().GetApplicationFeesAsync(application.ApplicationId));
                var criteria = new Dictionary<string, string> { { "ApplicationRefund", "1" } };
                var feeRefundMappingMatrix = await MappingMatrixHelper.GetItemsFromMappingMatrix(base.FeeRefundMappingMatrix, criteria);

                if (!(await IsApplicationEligibleForRefund(feeDetails, feeRefundMappingMatrix, criteria)))
                {
                    Log.Debug($"Application with application id {application.ApplicationId} not elibible for refund");
                    refundableApplications.Remove(application);
                }
                else
                    application.Actions = await base.GetRefundActions(application.Status, application.ServiceId);
            }

            return refundableApplications.AsEnumerable();
        }

        public async Task<RestRefundableApplication> FetchRefundableApplicationDetail(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
                throw ChannelErrorCodes.BadRequest.ToWebFault("Application id for refund is empty");

            var searchCriteria = GetApplicationRefundSearchParmas(UserId, string.Empty, applicationId);
            var searchResult = (await ApplicationHelper.GetApplicationSearchResult(searchCriteria)).FirstOrDefault();
            RestRefundableApplication refundableApplication = base.GetRefundObject(searchResult);

            var feeDetails = (await ApiFactory.Default.GetApplicationApi().GetApplicationFeesAsync(applicationId));
            var criteria = new Dictionary<string, string> { { "ApplicationRefund", "1" } };
            var feeRefundMappingMatrix = await MappingMatrixHelper.GetItemsFromMappingMatrix(FeeRefundMappingMatrix, criteria);

            Log.Debug($"Got fee details for application {applicationId}");

            refundableApplication.TotalAmount = Convert.ToInt32(feeDetails.Sum(f => f.Amount));
            refundableApplication.RefundableAmount = await base.GetRefundAmount(feeDetails, feeRefundMappingMatrix);
            refundableApplication.Actions = await base.GetRefundActions(refundableApplication.Status, refundableApplication.ServiceId);

            if (!(await base.IsApplicationEligibleForRefund(feeDetails, feeRefundMappingMatrix, criteria)))
                throw ChannelErrorCodes.BadRequest.ToWebFault($"Application with id {applicationId} is not allowed to refund");


            return refundableApplication;
        }

        public async Task<RestRefundInfo> ProcessApplicationRefund(RestRefundRequest refundRequest)
        {

            if (string.IsNullOrEmpty(refundRequest?.ApplicationId))
                throw ChannelErrorCodes.BadRequest.ToWebFault("Application id for refund is empty");

            try
            {
                var searchCriteria = GetApplicationRefundSearchParmas(UserId, ConfigurationRepository.GetApplicationRefundStatuses(), refundRequest.ApplicationId);
                var searchResult = (await ApplicationHelper.GetApplicationSearchResult(searchCriteria)).FirstOrDefault();

                if (searchResult == null)
                    throw ChannelErrorCodes.RefundFailed.ToWebFault("Application is not ready for refund or already refunded.");

                var refundableApplication = base.GetRefundObject(searchResult);

                var feeDetails = (await ApiFactory.Default.GetApplicationApi().GetApplicationFeesAsync(refundRequest.ApplicationId));
                var criteria = new Dictionary<string, string> { { "ApplicationRefund", "1" } };
                var feeRefundMatrixResponse = await MappingMatrixHelper.GetItemsFromMappingMatrix(base.FeeRefundMappingMatrix, criteria);

                Log.Info($"Prcessing refund and got fee details for application {refundRequest.ApplicationId}");

                if (!(await base.IsApplicationEligibleForRefund(feeDetails, feeRefundMatrixResponse, criteria)))
                {
                    Log.Info($"Application with application id {refundRequest.ApplicationId} is not allowed to refund due to no refund fee");
                    throw ChannelErrorCodes.BadRequest.ToWebFault("Application is not allowed to refund");
                }

                RestRefundInfo refundInfo = new RestRefundInfo
                {
                    Amount = Convert.ToString(await base.GetRefundAmount(feeDetails, feeRefundMatrixResponse)),
                    BatchId = refundableApplication.TransactionBatchId,
                    SystemId = base.SysTemId,
                    BankDetails = new RestBankDetails
                    {
                        BankName = refundRequest.BankName,
                        BeneficiaryName = refundRequest.BeneficiaryName,
                        IBAN = refundRequest.IbanNumber
                    },
                    Beneficiaries = await base.GetRefundBeneficaries(UserType, FeeBeneficiaryMappingMatrix, feeDetails, feeRefundMatrixResponse)
                };

                Log.Info($"Going to insert refund details for {refundRequest.ApplicationId}");
                await base.InsertRefundDetails(refundRequest, Convert.ToString((int)RefundType.ApplicationRefund), refundInfo.Amount, Convert.ToString(feeDetails.Sum(p => p.Amount)));
                Log.Info($"Refund object is {JsonConvert.SerializeObject(refundInfo)}");

                return refundInfo;
            }
            catch (Exception exp)
            {
                Log.Error(exp);
                throw;
            }
        }

        private RestApplicationSearchCriteria GetApplicationRefundSearchParmas(string userId, string statuses, string applicationId = null)
        {
            var searchCriteria = new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "*"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicantDetails",
                        PropertyName = "*"
                    },
                },
                UserId = userId,
                ApplicationId = applicationId,
                IsBatchIdMarked = "true",
                StatusId = statuses,
                IsPaid = "true",
                StatusFromDate = DateTime.Now.AddDays(-60),
                StatusToDate = DateTime.Now
            };

            return searchCriteria;
        }
    }
}