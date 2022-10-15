using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Helpers;
using System.Configuration;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Payment.Model;
using Emaratech.Services.Channels.Contracts.Extensions;
//using ApplicationStatus = Emaratech.Services.Application.Model.RestApplication.ApplicationStatusEnum;

namespace Emaratech.Services.Channels.BusinessLogic.Refund
{
    public class RefundBase
    {
        private readonly string _refundMappingMatrix;
        private readonly string _userId;
        private readonly string _userType;
        private readonly string _systemId;
        private readonly string _feeBeneficiaryMappingMatrix;

        public string FeeRefundMappingMatrix
        {
            get { return this._refundMappingMatrix; }
        }

        public string UserId
        {
            get { return this._userId; }
        }

        public string SysTemId
        {
            get { return this._systemId; }
        }

        public string FeeBeneficiaryMappingMatrix
        {
            get { return this._feeBeneficiaryMappingMatrix; }
        }

        public string UserType
        {
            get { return this._userType; }
        }

        public RefundBase()
        {

        }

        public RefundBase(string userId, string userType, string systemId)
        {
            _refundMappingMatrix = ConfigurationManager.AppSettings["FeeRefundMappingMatrix"];
            _feeBeneficiaryMappingMatrix = ConfigurationManager.AppSettings["FeeBeneficiaryMappingMatrix"];
            _userType = userType;
            _userId = userId;
            _systemId = systemId;
        }
        public RestRefundableApplication GetRefundObject(RestApplicationSearchRow searchResult)
        {
            RestRefundableApplication refundableApplication = new RestRefundableApplication
            {
                ApplicationId = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value,
                FullName = new RestName
                {
                    En = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "fullnamee")?.Value,
                    Ar = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "fullnamea")?.Value,
                },
                Status = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "statusid")?.Value,
                ServiceId = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "serviceid")?.Value,
                VisaType = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visatypeid")?.Value,
                CreatedDate = Convert.ToDateTime(searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "createddate")?.Value),
                StatusDate = Convert.ToDateTime(searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "statusdate")?.Value),
                TransactionBatchId = searchResult.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "transactionbatchid")?.Value
            };

            return refundableApplication;
        }

        public async Task<bool> IsApplicationEligibleForRefund(List<RestApplicationFee> feeDetails, List<MappingMatrixResponse> mappingMatrixResponse, Dictionary<string, string> criteria)
        {
            if (mappingMatrixResponse == null)
            {
                mappingMatrixResponse = await MappingMatrixHelper.GetItemsFromMappingMatrix(_refundMappingMatrix, criteria);
            }
            var feesItem = mappingMatrixResponse.Where(p => p.mappingMatrixItems.Any(m => m.Key == "IsMandatoryForRefund" && m.Value == "1")).ToList();

            //If no mandatory fees then go forward
            if (feesItem.Count <= 0) return true;

            //If there is mandatory fees and even one mandaotry fee type is found in fee details then proceed
            var lstMandatoryFeeTypes = new List<string>();

            foreach (var fee in feesItem)
                lstMandatoryFeeTypes.AddRange(fee.mappingMatrixItems.Where(p => p.Key == "FeeType").Select(r => r.Value).ToList());

            var lstPaidFeeTypes = feeDetails.Select(c => c.FeeType).ToList();

            return lstPaidFeeTypes.Intersect<string>(lstMandatoryFeeTypes).Any();
        }

        public async Task<int?> GetRefundAmount(List<RestApplicationFee> feeDetails, List<MappingMatrixResponse> mappingMatrixResponse)
        {
            int? refundableAmount = 0;

            foreach (var fee in feeDetails)
            {
                var mappingMatrixItem = mappingMatrixResponse.FirstOrDefault(m => m.mappingMatrixItems.Any(p => p.Key == "FeeType" && p.Value == fee.FeeType));

                if (mappingMatrixItem != null) //If fee match found then add it in refund amount and remove the fixed deduct amount for refund services if against any fee type
                {
                    refundableAmount = refundableAmount + (Convert.ToInt32(fee.Amount) - Convert.ToString(mappingMatrixItem?.mappingMatrixItems.FirstOrDefault(m => m.Key == "FixedFeeToDeduct").Value).ToInt32());
                }
            }

            return refundableAmount;
        }

        public async Task<List<RestBeneficiary>> GetRefundBeneficaries(string userType, string beneficiaryMappingMatrixId, List<RestApplicationFee> feeDetails, List<MappingMatrixResponse> mappingMatrixResponse)
        {
            try
            {
                List<RestBeneficiary> lstBeneficaries = new List<RestBeneficiary>();

                foreach (var fee in feeDetails)
                {
                    var mappingMatrixItem =
                        mappingMatrixResponse.FirstOrDefault(
                            m => m.mappingMatrixItems.Any(p => p.Key == "FeeType" && p.Value == fee.FeeType));

                    if (mappingMatrixItem != null)
                    {
                        int feeToDeduct = 0;
                        int.TryParse(mappingMatrixItem.mappingMatrixItems.FirstOrDefault(m => m.Key == "FixedFeeToDeduct").Value, out feeToDeduct);

                        var criteria = new Dictionary<string, string>
                        {
                            {"FeeType", fee.FeeType},
                            {"FeeAmount", fee.Amount.ToString()},
                            {"UserType", userType}
                        };
                        var beneficaryResponse = await MappingMatrixHelper.GetItemsFromMappingMatrix(beneficiaryMappingMatrixId, criteria);
                        var merchant = await PaymentHelper.GetMerchant(userType);

                        lstBeneficaries.AddRange(beneficaryResponse.Select(item => new RestBeneficiary
                        {
                            Account = merchant.Beneficiaries?.FirstOrDefault(b => b.Name.ToLower() == item.mappingMatrixItems.FirstOrDefault(p => p.Key == "BeneficiaryDesc").Value?.ToLower())?.Code,
                            Amount = (Convert.ToInt32(item.mappingMatrixItems.FirstOrDefault(p => p.Key == "BeneficiaryAmount").Value) - feeToDeduct).ToString(),
                            Code = merchant.Beneficiaries?.FirstOrDefault(b => b.Name.ToLower() == item.mappingMatrixItems.FirstOrDefault(p => p.Key == "BeneficiaryDesc").Value?.ToLower())?.Code,
                            Desc = item.mappingMatrixItems.FirstOrDefault(p => p.Key == "BeneficiaryDesc").Value,
                            Currency = item.mappingMatrixItems.FirstOrDefault(p => p.Key == "BeneficiaryCurrency").Value
                        }));
                    }
                }

                return lstBeneficaries;
            }
            catch (Exception exp)
            {
                throw;
            }
        }
        public RestApplicationSearchCriteria GetAllRefundedApplicationsParams(string applicationId)
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
                UserId = UserId,
                StatusId = ConfigurationRepository.GetRefundListStatuses(),
                IsPaid = "true",
                IsBatchIdMarked = "true",
                ApplicationId = applicationId
            };

            return searchCriteria;
        }

        public async Task InsertRefundDetails(RestRefundRequest refundRequest, string refundType, string refundAmount, string totalAmount)
        {
            await ApiFactory.Default.GetApplicationApi().InsertRefundDetailsAsync(new RestApplicationRefundInfo
            {
                ApplicationId = refundRequest.ApplicationId,
                BankName = refundRequest.BankName,
                BeneficiaryName = refundRequest.BeneficiaryName,
                Iban = refundRequest.IbanNumber,
                RefundAmount = refundAmount,
                TotalAmount = totalAmount,
                RefundType = refundType
            });
        }

        public async Task<IList<RestApplicationActions>> GetRefundActions(string statusId, string serviceId)
        {
            return await ApplicationHelper.GetApplicationActions(new RestApplicationActionsCriteria { ApplicationStatus = statusId, ServiceId = serviceId, ApplicationModule = ApplicationModules.Refund, UserType = _userType });
        }
    }
}