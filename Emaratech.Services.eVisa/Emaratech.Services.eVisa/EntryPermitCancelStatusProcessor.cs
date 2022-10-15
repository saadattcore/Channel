using Emaratech.Services.Application.Model;
using Emaratech.Services.Common.Configuration;
using Emaratech.Services.eVisa.Lookups;
using Emaratech.Services.eVisa.Reports;
using Emaratech.Services.Vision.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa
{
    class EntryPermitCancelStatusProcessor : StatusProcessor
    {

        #region Private Fields
        private ILookup nationalityLookup => LookupNationality.Instance;
        private ILookup visaTypeLookup => LookupVisaType.Instance;
        private ILookup professionLookup => LookupProfession.Instance;
        private ILookup passportTypeLookup => LookupPassportType.Instance;
        #endregion

        #region  Overriden Methods

        protected override string GetSubject(RestApplicationSearchRow application)
         => ReportUtil.entryPermitCancelApprovedSubject;

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            string content = TemplateManager.GetTemplate(DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues), "TemplateIdEntryPermitNewCancelApplication");
            return content;
        }

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var id = ConfigurationSystem.AppSettings["EntryPermitCancelCategory"];
            return ApplicationRepository.GetApprovedStatus(id);
        }

        protected override string GetAttachment(RestApplicationSearchRow application)
        {
            string applicationId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
            string visaNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;

            var eVisaData = GenerateeVisa(applicationId, visaNumber);
            var jsonData = JsonConvert.SerializeObject(eVisaData);

            string eVisaMainReport = ReportUtil.eResidenceCancelReport;
            // string eVisaInstrucitonsReport = ReportUtil.eVisaInstructionsReport;
            List<string> reportNames = new List<string>();
            reportNames.Add(eVisaMainReport);
            // reportNames.Add(eVisaInstrucitonsReport);

            var reportContent = ReportUtil.BuildReportWithoutInstruction(jsonData, reportNames, ReportUtil.ReportingServiceUrlMultiPdf, string.Empty);

            return reportContent;
        }

        #endregion

        private JObject GenerateeVisa(string applicationId, string permitNumber)
        {
            #region Data Fecthing and Travel Detail Info
            var permitInfo = ServicesHelper.GetVisionApi.GetIndividualDetailedInformationByPermitNo(permitNumber);
            var sponsor = ReportUtil.GetSponsorInfo(permitInfo.IndividualUserInfo?.IndividualPermitInfo?.SponsorTypeId, permitInfo, FileType.EntryPermit);
            string ppsID = permitInfo.IndividualUserInfo?.IndividualProfileInformation?.PPSID;
            var travelDetail = ServicesHelper.GetVisionApi.GetIndividualCurrentTravelStatusByPpsId(new Vision.Model.RestVisionQueryCriteria() { Values = new List<string>() { ppsID } }).FirstOrDefault();
            var indivPermitNo = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitNo;
            int remainingDays = ReportUtil.CalculateNoOfDays(permitInfo.IndividualUserInfo?.IndividualPermitInfo?.ValidityDate, permitInfo.IndividualUserInfo?.IndividualPermitInfo?.CancelDate);

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
            #endregion

            #region Sponsored Details
            dataObj["sponsoredOccupationEn"] = professionLookup.GetEn(permitInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());
            dataObj["sponsoredOccupationAr"] = professionLookup.GetAr(permitInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());
            dataObj["sponsoredNationalityEn"] = nationalityLookup.GetEn(permitInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());
            dataObj["sponsoredNationalityAr"] = nationalityLookup.GetAr(permitInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());
            dataObj["sponsoredNameEn"] = permitInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameEn;
            dataObj["sponsoredNameAr"] = permitInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameAr;
            dataObj["sponsoredFileNo"] = ReportUtil.FormatPermitNo(permitInfo.IndividualUserInfo.IndividualPermitInfo?.PermitNo);
            string passportNumber = permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            string passportTypeEn = passportTypeLookup.GetEn(permitInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            string passportTypeAr = passportTypeLookup.GetAr(permitInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            dataObj["sponsoredPassportNo"] = ReportUtil.FormatString(passportTypeEn, passportNumber);
            dataObj["sponsoredNoteEn"] = ReportUtil.GetNotes(travelTypeId, "en", 1, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            dataObj["sponsoredNoteAr"] = ReportUtil.GetNotes(travelTypeId, "ar", 1, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            #endregion

            #region Sponsor Details
            dataObj["sponsorTypeEn"] = Lookups.LookupSponsorType.Instance.GetEn(sponsor.SponsorTypeId);
            dataObj["sponsorTypeAr"] = Lookups.LookupSponsorType.Instance.GetAr(sponsor.SponsorTypeId);
            dataObj["sponsorPassportNo"] = sponsor.SponsorPassportNo;
            dataObj["sponsorPhoneNo"] = sponsor.SponsorPhoneNumber;
            dataObj["cancellationReasonAr"] = "طلب الراعي";
            dataObj["cancellationReasonEn"] = "Sponsor Request";
            dataObj["sponsorNameAr"] = sponsor.SponsorNameAr;
            dataObj["sponsorNameEn"] = sponsor.SponsorNameEn;
            dataObj["sponsorFileNo"] = sponsor.SponsorFileNumber;
            dataObj["sponsorNoteEn"] = ReportUtil.GetNotes(travelTypeId, "en", 2, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            dataObj["sponsorNoteAr"] = ReportUtil.GetNotes(travelTypeId, "ar", 2, permitInfo.IndividualUserInfo.IndividualPermitInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            #endregion

            #region Other Details
            dataObj["visaTypeAr"] = "إلغــاء تأشيــرة بعـد الدخول";
            dataObj["visaTypeEn"] = "Visa Cancellation After Entry";
            dataObj["cancellationDate"] = permitInfo.IndividualUserInfo.IndividualPermitInfo?.CancelDate;
            dataObj["banReasonEn"] = string.Empty;
            dataObj["banReasonAr"] = string.Empty;
            dataObj["printedDate"] = DateTime.Now.ToString("dd-MM-yyyy");
            dataObj["accompanyCount"] = 0;
            #endregion

            return dataObj;
        }
    }
}
