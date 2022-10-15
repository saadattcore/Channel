using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Payment.Model;
using Emaratech.Services.Vision.Api;
using log4net;
using Newtonsoft.Json;
//using ApplicationStatus = Emaratech.Services.Application.Model.RestApplication.ApplicationStatusEnum;

namespace Emaratech.Services.Channels.BusinessLogic.Refund
{
    public class WarrantyRefund : RefundBase, IRefund
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WarrantyRefund));

        public WarrantyRefund(string userId, string userType, string systemId) : base(userId, userType, systemId)
        {

        }

        public async Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplications()
        {
            var searchCriteria = GetWarrantyRefundSearchParmas(base.UserId, ConfigurationRepository.GetWarrantyRefundStatuses());

            var searchResult = await ApplicationHelper.GetApplicationSearchResult(searchCriteria);
            var visionApi = ApiFactory.Default.GetVisionIndividualApi();

            //Remove all those applications whose visa status is not permanant closed or has violation or close date is greater than 5 years
            foreach (var searchedApplication in searchResult.Reverse<RestApplicationSearchRow>())
            {
                string visaNumber = searchedApplication.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;
                string visaNumberType = searchedApplication.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumbertype")?.Value;
                string applicationId = searchedApplication.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;

                if (!await IsApplicationEligibleForWarrantyRefund(visionApi, visaNumber, visaNumberType))
                {
                    Log.Debug($"Application with application id {applicationId} not eligible for refund due to vision rules failure");
                    searchResult.Remove(searchedApplication);
                }
            }

            IList<RestRefundableApplication> refundableApplications = searchResult.Select(base.GetRefundObject).ToList();

            foreach (var application in refundableApplications.Reverse<RestRefundableApplication>())
            {
                var feeDetails = (await ApiFactory.Default.GetApplicationApi().GetApplicationFeesAsync(application.ApplicationId));
                var criteria = new Dictionary<string, string> { { "WarrantyRefund", "1" } };
                var feeRefundMappingMatrix = await MappingMatrixHelper.GetItemsFromMappingMatrix(base.FeeRefundMappingMatrix, criteria);

                if (!(await IsApplicationEligibleForRefund(feeDetails, feeRefundMappingMatrix, criteria)))
                {
                    Log.Debug($"Application with application id {application.ApplicationId} not eligible for refund due to no refund fee");
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

            var searchCriteria = GetWarrantyRefundSearchParmas(base.UserId, string.Empty, applicationId);

            var searchResult = (await ApplicationHelper.GetApplicationSearchResult(searchCriteria)).FirstOrDefault();
            var visionApi = ApiFactory.Default.GetVisionIndividualApi();

            string visaNumber = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;
            string visaNumberType = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumbertype")?.Value;

            if (!await IsApplicationEligibleForWarrantyRefund(visionApi, visaNumber, visaNumberType))
            {
                Log.Debug($"Application with application id {applicationId} not eligible for refund due to vision rules failure");
                throw ChannelErrorCodes.BadRequest.ToWebFault("Application is not allowed to refund");
            }

            RestRefundableApplication refundableApplication = base.GetRefundObject(searchResult);

            var feeDetails = (await ApiFactory.Default.GetApplicationApi().GetApplicationFeesAsync(applicationId));
            var criteria = new Dictionary<string, string> { { "WarrantyRefund", "1" } };
            var feeRefundMappingMatrix = await MappingMatrixHelper.GetItemsFromMappingMatrix(FeeRefundMappingMatrix, criteria);
            refundableApplication.Actions = await base.GetRefundActions(refundableApplication.Status, refundableApplication.ServiceId);

            Log.Debug($"Got fee details for application {applicationId}");

            if (!(await base.IsApplicationEligibleForRefund(feeDetails, feeRefundMappingMatrix, criteria)))
                throw ChannelErrorCodes.BadRequest.ToWebFault($"Application with application id {applicationId} is not allowed to refund due to no refund fee");

            refundableApplication.TotalAmount = Convert.ToInt32(feeDetails.Sum(f => f.Amount));
            refundableApplication.RefundableAmount = await base.GetRefundAmount(feeDetails, feeRefundMappingMatrix);

            return refundableApplication;
        }

        public async Task<RestRefundInfo> ProcessApplicationRefund(RestRefundRequest refundRequest)
        {
            if (string.IsNullOrEmpty(refundRequest?.ApplicationId))
                throw ChannelErrorCodes.BadRequest.ToWebFault("Application id for refund is empty");

            try
            {
                var searchCriteria = GetWarrantyRefundSearchParmas(UserId, ConfigurationRepository.GetWarrantyRefundStatuses(), refundRequest.ApplicationId);
                var searchResult = (await ApplicationHelper.GetApplicationSearchResult(searchCriteria)).FirstOrDefault();

                if (searchResult == null)
                    throw ChannelErrorCodes.RefundFailed.ToWebFault("Application is not ready for refund or already refunded.");

                var visionApi = ApiFactory.Default.GetVisionIndividualApi();

                string visaNumber = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;
                string visaNumberType = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumbertype")?.Value;

                if (!await IsApplicationEligibleForWarrantyRefund(visionApi, visaNumber, visaNumberType))
                    throw ChannelErrorCodes.BadRequest.ToWebFault("Application is not allowed to refund");

                var refundableApplication = base.GetRefundObject(searchResult);
                var feeDetails = (await ApiFactory.Default.GetApplicationApi().GetApplicationFeesAsync(refundRequest.ApplicationId));
                var criteria = new Dictionary<string, string> { { "WarrantyRefund", "1" } };
                var feeRefundMatrixResponse = await MappingMatrixHelper.GetItemsFromMappingMatrix(base.FeeRefundMappingMatrix, criteria);

                Log.Info($"Prcessing refund and got fee details for application {refundRequest.ApplicationId}");

                if (!(await base.IsApplicationEligibleForRefund(feeDetails, feeRefundMatrixResponse, criteria)))
                {
                    Log.Info($"Application with application id {refundRequest?.ApplicationId} is not allowed to refund due to no refund fee");
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

                Log.Info($"Going to insert refund details for application id {refundRequest.ApplicationId}");
                await base.InsertRefundDetails(refundRequest, Convert.ToString((int)RefundType.WarrantyRefund), refundInfo.Amount, Convert.ToString(feeDetails.Sum(p => p.Amount)));
                Log.Info($"Refund object is {JsonConvert.SerializeObject(refundInfo)}");

                return refundInfo;
            }
            catch (Exception exp)
            {
                Log.Error(exp);
                throw;
            }
        }

        private RestApplicationSearchCriteria GetWarrantyRefundSearchParmas(string userId, string statuses, string applicationId = null)
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
                IsPaid = "true"
            };

            //Emaratech.Services.Application.Model.ApplicationStatus app = new Application.Model.ApplicationStatus();
            //Emaratech.Services.Application.Model.ApplicationStatus
            return searchCriteria;
        }

        private async Task<bool> IsApplicationEligibleForWarrantyRefund(IVisionIndividualApi visionApi, string visaNumber, string visaNumberType)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(visaNumber))
            {
                if (visaNumberType == Convert.ToString((int)ServiceType.Residence))
                {
                    var residenceInfo = await visionApi.GetIndividualResidenceInfoAsync(visaNumber);

                    if (residenceInfo?.ResidenceStatusId == (int)ResidenceStatusType.PermanentClosed)
                    {
                        var violation = await visionApi.GetIndividualViolationByResNoAsync(visaNumber);
                        if (violation == null || Convert.ToInt32(violation.OverStayDays) <= 0)
                            if (DateTime.Now.AddYears(-5) <= residenceInfo?.CloseDate) //If its more than 5 years of being closed then don't allow
                                if (Convert.ToInt32(await visionApi.GetIndividualViolationPaymentByResNoAsync(visaNumber)) <= 0) //If there is no violation payment history found on same file
                                    result = true;
                    }
                }

                else if (visaNumberType == Convert.ToString((int)ServiceType.EntryPermit) || string.IsNullOrEmpty(visaNumberType))
                {
                    var permitInfo = await visionApi.GetIndividualPermitInfoAsync(visaNumber);
                    if (permitInfo?.PermitStatusId == (int)PermitStatusType.PermanentClosed)
                    {
                        Log.Debug($"Permit Info of {visaNumber} is {JsonConvert.SerializeObject(permitInfo)}");
                        var violation = await visionApi.GetIndividualViolationByPermitNoAsync(visaNumber);
                        Log.Debug($"Violation of {visaNumber} is {JsonConvert.SerializeObject(violation)}");
                        if (violation == null || Convert.ToInt32(violation.OverStayDays) <= 0)
                            if (DateTime.Now.AddYears(-5) <= permitInfo?.CloseDate) //If its more than 5 years of being closed then don't allow
                                if (Convert.ToInt32(await visionApi.GetIndividualViolationPaymentByPermitNoAsync(visaNumber)) <= 0) //If there is no violation payment history found only then allow
                                    result = true;
                    }
                }
            }
            Log.Debug($"Result of visa numer {visaNumber} is {result}");

            return result;
        }
    }
}