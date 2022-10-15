using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.WcfCommons.Faults.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports.Cancellation
{
    public class ResidenceCancellationReportHandler : CancellationReportBaseHanlder
    {
        protected async override Task<IList<RestIndividualUserFormInfo>> GetUserFormInfo(IList<string> residenceIds)
        {
            var residenceCancellationStatuses = ConfigurationRepository.GetResidanceCancellationStatuses();
            var formsInfo = await ServicesHelper.VisionApi.GetIndividualDetailedInformationListByResNoAsync(new RestVisionQueryCriteria { Values = residenceIds.ToList() });
            var establishmentCode = ServicesHelper.GetUserEstablishmentsCode();

            var allowedForms = formsInfo
                .Where(i => i.IndividualUserInfo != null)
                .Where(i => i.IndividualUserInfo.IndividualResidenceInfo != null)
                .Where(i => i.IndividualUserInfo.IndividualResidenceInfo.ResidenceStatusId.HasValue)
                .Where(i => establishmentCode.Contains(i.IndividualUserInfo.IndividualPermitInfo.EstCode))
                .Where(i => residenceCancellationStatuses.Contains(i.IndividualUserInfo.IndividualResidenceInfo.ResidenceStatusId.Value))
                .ToList();

            return allowedForms;
        }

        protected override string SerializeAndGetReportData(RestIndividualUserFormInfo residenceInfo)
        {
            string ppsID = residenceInfo.IndividualUserInfo?.IndividualProfileInformation?.PPSID;

            var travelDetail = ServicesHelper.VisionApi.GetIndividualCurrentTravelStatusByPpsId(new Vision.Model.RestVisionQueryCriteria() { Values = new List<string>() { ppsID } }).FirstOrDefault();
            int remainingDays = CalculateNoOfDays(residenceInfo.IndividualUserInfo?.IndividualResidenceInfo?.ValidityDate, residenceInfo.IndividualUserInfo?.IndividualResidenceInfo?.CancelDate);

            string travelDate = string.Empty;
            int travelTypeId = 0;

            if (travelDetail != null)
            {
                if (travelDetail.TravelDate.HasValue)
                {
                    travelDate = travelDetail.TravelDate.Value.ToString("dd-MM-yyyy");
                }

                if (!string.IsNullOrEmpty(travelDetail.TravelTypeId))
                {
                    travelTypeId = Convert.ToInt32(travelDetail.TravelTypeId);
                }
            }

            var indivPermitNo = residenceInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceNo;
            JObject dataObj = new JObject();

            dataObj["sponsoredOccupationEn"] = professionLookup.GetEn(residenceInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());
            dataObj["sponsoredOccupationAr"] = professionLookup.GetAr(residenceInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());

            dataObj["sponsoredNationalityEn"] = nationalityLookup.GetEn(residenceInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());
            dataObj["sponsoredNationalityAr"] = nationalityLookup.GetAr(residenceInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());

            dataObj["sponsoredNameEn"] = residenceInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameEn;
            dataObj["sponsoredNameAr"] = residenceInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameAr;

            dataObj["sponsoredNoteEn"] = GetNotes(travelTypeId, "en", 1, residenceInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDetail.TravelDate?.ToString("dd-MM-yyyy"), remainingDays);
            dataObj["sponsoredNoteAr"] = GetNotes(travelTypeId, "ar", 1, residenceInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDetail.TravelDate?.ToString("dd-MM-yyyy"), remainingDays);

            dataObj["accompanyCount"] = 0;

            dataObj["cancellationReasonAr"] = "طلب الراعي";
            dataObj["cancellationReasonEn"] = "Sponsor Request";

            dataObj["visaTypeAr"] = "إلغــاء إقامــــة";
            dataObj["visaTypeEn"] = "Residence Cancellation";

            string passportNumber = residenceInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            string passportTypeEn = passportTypeLookup.GetEn(residenceInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            string passportTypeAr = passportTypeLookup.GetAr(residenceInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            dataObj["sponsoredPassportNo"] = FormatString(passportTypeEn, passportNumber);

            dataObj["sponsoredFileNo"] = FormatPermitNo(residenceInfo.IndividualUserInfo.IndividualResidenceInfo?.ResidenceNo);

            dataObj["cancellationDate"] = residenceInfo.IndividualUserInfo.IndividualResidenceInfo?.CancelDate?.ToString("dd-MM-yyyy");

            dataObj["banReasonEn"] = string.Empty;
            dataObj["banReasonAr"] = string.Empty;

            var establishmentCode = residenceInfo.IndividualUserInfo.IndividualResidenceInfo.EstCode;
            var establishmentProfile = ServicesHelper.GetEstablishmentProfile(establishmentCode).Result;

            dataObj["sponsorPhoneNo"] = establishmentProfile.MobileNo;
            dataObj["sponsorNameAr"] = establishmentProfile.EstabNameAr;
            dataObj["sponsorNameEn"] = establishmentProfile.EstabNameEn;

            dataObj["sponsorTypeEn"] = LookupSponsorType.Instance.GetEn(establishmentProfile.EstabTypeId?.ToString());
            dataObj["sponsorTypeAr"] = LookupSponsorType.Instance.GetAr(establishmentProfile.EstabTypeId?.ToString());

            dataObj["sponsorFileNo"] = establishmentCode;
            dataObj["printedDate"] = DateTime.Now.ToString("dd-MM-yyyy");

            dataObj["sponsorNoteEn"] = GetNotes(travelTypeId, "en", 2, residenceInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDetail.TravelDate.ToString(), remainingDays);
            dataObj["sponsorNoteAr"] = GetNotes(travelTypeId, "ar", 2, residenceInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDetail.TravelDate.ToString(), remainingDays);

            return JsonConvert.SerializeObject(dataObj, serializerSettings);
        }
    }
}