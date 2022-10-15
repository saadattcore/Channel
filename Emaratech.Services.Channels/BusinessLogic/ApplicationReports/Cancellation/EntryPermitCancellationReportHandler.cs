using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Vision.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports.Cancellation
{
    public class EntryPermitCancellationReportHandler : CancellationReportBaseHanlder
    {
        protected async override Task<IList<RestIndividualUserFormInfo>> GetUserFormInfo(IList<string> permitIds)
        {
            var entryPermitStatuses = ConfigurationRepository.GetEntryPermitCancellationStatuses();
            var formsInfo = await ServicesHelper.VisionApi.GetIndividualDetailedInformationListByPermitNoAsync(new RestVisionQueryCriteria { Values = permitIds.ToList() });
            var establishmentCode = ServicesHelper.GetUserEstablishmentsCode();

            var allowedForms = formsInfo
                .Where(i => i.IndividualUserInfo != null)
                .Where(i => i.IndividualUserInfo.IndividualPermitInfo != null)
                .Where(i => i.IndividualUserInfo.IndividualPermitInfo.PermitStatusId.HasValue)
                .Where(i => establishmentCode.Contains(i.IndividualUserInfo.IndividualPermitInfo.EstCode))
                .Where(i => entryPermitStatuses.Contains(i.IndividualUserInfo.IndividualPermitInfo.PermitStatusId.Value)).ToList();

            return allowedForms;
        }

        protected override string SerializeAndGetReportData(RestIndividualUserFormInfo permitInfo)
        {
            string ppsID = permitInfo.IndividualUserInfo?.IndividualProfileInformation?.PPSID;

            var travelDetail = ServicesHelper.VisionApi.GetIndividualCurrentTravelStatusByPpsId(new Vision.Model.RestVisionQueryCriteria() { Values = new List<string>() { ppsID } }).FirstOrDefault();
            var indivPermitNo = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitNo;
            int remainingDays = CalculateNoOfDays(permitInfo.IndividualUserInfo?.IndividualPermitInfo?.ValidityDate, permitInfo.IndividualUserInfo?.IndividualPermitInfo?.CancelDate);

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

            JObject dataObj = new JObject();

            dataObj["sponsoredOccupationEn"] = professionLookup.GetEn(permitInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());
            dataObj["sponsoredOccupationAr"] = professionLookup.GetAr(permitInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());

            dataObj["sponsoredNationalityEn"] = nationalityLookup.GetEn(permitInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());
            dataObj["sponsoredNationalityAr"] = nationalityLookup.GetAr(permitInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());

            dataObj["sponsoredNameEn"] = permitInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameEn;
            dataObj["sponsoredNameAr"] = permitInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameAr;

            dataObj["sponsoredNoteEn"] = GetNotes(travelTypeId, "en", 1, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            dataObj["sponsoredNoteAr"] = GetNotes(travelTypeId, "ar", 1, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);

            dataObj["accompanyCount"] = 0;

            dataObj["cancellationReasonAr"] = "طلب الراعي";
            dataObj["cancellationReasonEn"] = "Sponsor Request";

            dataObj["visaTypeAr"] = "إلغــاء تأشيــرة بعـد الدخول";
            dataObj["visaTypeEn"] = "Visa Cancellation After Entry";

            string passportNumber = permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            string passportTypeEn = passportTypeLookup.GetEn(permitInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            string passportTypeAr = passportTypeLookup.GetAr(permitInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            dataObj["sponsoredPassportNo"] = FormatString(passportTypeEn, passportNumber);


            dataObj["sponsoredFileNo"] = permitInfo.IndividualUserInfo.IndividualPermitInfo?.PermitNo;

            dataObj["cancellationDate"] = permitInfo.IndividualUserInfo.IndividualPermitInfo?.CancelDate;

            dataObj["banReasonEn"] = string.Empty;
            dataObj["banReasonAr"] = string.Empty;

            var establishmentCode = permitInfo.IndividualUserInfo.IndividualPermitInfo.EstCode;
            var establishmentProfile = ServicesHelper.GetEstablishmentProfile(establishmentCode).Result;

            dataObj["sponsorPhoneNo"] = establishmentProfile.MobileNo;
            dataObj["sponsorNameAr"] = establishmentProfile.EstabNameAr;
            dataObj["sponsorNameEn"] = establishmentProfile.EstabNameEn;

            dataObj["sponsorTypeEn"] = LookupSponsorType.Instance.GetEn(establishmentProfile.EstabTypeId?.ToString());
            dataObj["sponsorTypeAr"] = LookupSponsorType.Instance.GetAr(establishmentProfile.EstabTypeId?.ToString());

            dataObj["sponsorFileNo"] = establishmentCode;
            dataObj["printedDate"] = DateTime.Now.ToString("dd-MM-yyyy");

            dataObj["sponsorNoteEn"] = GetNotes(travelTypeId, "en", 2, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            dataObj["sponsorNoteAr"] = GetNotes(travelTypeId, "ar", 2, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);

            return JsonConvert.SerializeObject(dataObj, serializerSettings);
        }
    }
}