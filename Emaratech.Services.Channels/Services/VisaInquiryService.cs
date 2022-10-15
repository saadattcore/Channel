using Emaratech.Services.Channels.Contracts.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using log4net;
using Emaratech.Services.UserManagement.Model;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Contracts.Errors;
using System.Text.RegularExpressions;
using System;
using System.Configuration;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Contracts.Rest.Models.VisaInquiry;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService : IVisaInquiryService
    {
        public async Task<RestVisaInquiryResult> FetchVisaInquiryInformation(RestVisaInquiryCriteria request)
        {
            try
            {
                string serviceType = LookupHelper.GetLookupCol2ById(ConfigurationManager.AppSettings["ServiceTypeLookupId"], request.serviceType);

                var result = await visionCommonApi.GeteVisaInquiryResultAsync(int.Parse(serviceType), request.visaNumber);

                RestVisaInquiryResult visaInquiryResult = null;

                if (result != null)
                {
                    Log.Debug("Got result of visa inquiry");

                    if (result.FirstNameEn?.ToLower() == request?.firstName?.ToLower() && Convert.ToDateTime(result.BirthDate).ToString("MM-dd-yyyy") == request.dateOfBirth?.ToString("MM-dd-yyyy") && Convert.ToString(result.NationalityId) == request.nationalityId)
                    {
                        Log.Debug("Validation passed for eVisa inquiry.");

                        visaInquiryResult = new RestVisaInquiryResult();

                        var travelDetail = await visionApi.GetIndividualTravelLastEntryByPPSIDAsync(result.PPSID);

                        //If person is inside country with same permit or residence number then set validate after entry 
                        //otherwise set validate before entry

                        if (travelDetail != null)
                        {
                            if (travelDetail.TravelTypeId == Convert.ToString((int) TravelType.Entry))
                            {
                                visaInquiryResult.entryDate = travelDetail.TravelDate;

                                if (serviceType == Convert.ToString((int) ServiceType.EntryPermit) && travelDetail.PermitNo == result.VisaNo)
                                    visaInquiryResult.validityDateAfterEntry = result.ValidityDate;

                                if (serviceType == Convert.ToString((int) ServiceType.Residence) && travelDetail.ResidenceNo == result.VisaNo)
                                    visaInquiryResult.validityDateAfterEntry = result.ValidityDate;

                            }
                        }

                        if (string.IsNullOrEmpty(Convert.ToString(visaInquiryResult.validityDateAfterEntry)))
                            visaInquiryResult.validityDateBeforeEntry = result.ValidityDate;

                        visaInquiryResult.fullName = new RestName { En = result.FullNameEn, Ar = result.FullNameAr };
                        visaInquiryResult.visaType = Convert.ToString(result.VisaTypeId);
                        visaInquiryResult.visaStatus = Convert.ToString(result.VisaStatusId);

                        visaInquiryResult.expiryDate = result.VisaExpiryDate;

                    }
                    else
                    {
                        Log.Debug("Validation of name, date of birth or nationality failed for eVisa Inquiry.");
                    }
                }

                if (visaInquiryResult == null)
                    throw ChannelErrorCodes.eVisaInquiryResultNotfound.ToWebFault($"eVisa inquiry result not found for permit number {request.visaNumber}");

                return visaInquiryResult;
            }
            catch (Exception exp)
            {
                Log.Error(exp);
                throw exp;
            }
        }
    }
}