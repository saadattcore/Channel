using Emaratech.Services.Application.Model;
using Emaratech.Services.eVisa.Reports;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.eVisa.Lookups;
using Newtonsoft.Json;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    class ResidenceCancelStatusProcessor : StatusProcessor
    {
        #region Private Fields
        private ILookup nationalityLookup => LookupNationality.Instance;
        private ILookup visaTypeLookup => LookupVisaType.Instance;
        private ILookup professionLookup => LookupProfession.Instance;
        private ILookup passportTypeLookup => LookupPassportType.Instance;
        #endregion

        #region Specific Overriden Methods 
        protected override string GetSubject(RestApplicationSearchRow application)
  => ReportUtil.residenceCancelApprovedSubject;

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            string content = TemplateManager.GetTemplate(DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues), "TemplateIdCancelResidence");
            return content;
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
          //  reportNames.Add(eVisaInstrucitonsReport);

            var reportContent = ReportUtil.BuildReportWithoutInstruction(jsonData, reportNames, ReportUtil.ReportingServiceUrlMultiPdf, string.Empty);

            return reportContent;
        }

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var id = ConfigurationSystem.AppSettings["ResidenceCancelCategory"];
            return ApplicationRepository.GetApprovedStatus(id);
        }
        #endregion

        private JObject GenerateeVisa(string applicationId, string permitNumber)
        {
            #region Data Fetching and Travel Detail Info
            var permitInfo = ServicesHelper.GetVisionApi.GetIndividualDetailedInformationByResNo(permitNumber);
            var sponsor = ReportUtil.GetSponsorInfo(permitInfo.IndividualUserInfo?.IndividualResidenceInfo?.SponsorTypeId, permitInfo , FileType.Residence);
            string ppsID = permitInfo.IndividualUserInfo?.IndividualProfileInformation?.PPSID;
            var travelDetail = ServicesHelper.GetVisionApi.GetIndividualCurrentTravelStatusByPpsId(new Vision.Model.RestVisionQueryCriteria() { Values = new List<string>() { ppsID } }).FirstOrDefault();
            int remainingDays = ReportUtil.CalculateNoOfDays(permitInfo.IndividualUserInfo?.IndividualResidenceInfo?.ValidityDate, permitInfo.IndividualUserInfo?.IndividualResidenceInfo?.CancelDate);

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

            var indivPermitNo = permitInfo.IndividualUserInfo.IndividualResidenceInfo.ResidenceNo;
            JObject dataObj = new JObject();
            #endregion

            #region Sponsored Details
            dataObj["sponsoredOccupationEn"] = professionLookup.GetEn(permitInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());
            dataObj["sponsoredOccupationAr"] = professionLookup.GetAr(permitInfo.IndividualUserInfo.IndividualProfileInformation?.ProfessionId?.ToString());
            dataObj["sponsoredNationalityEn"] = nationalityLookup.GetEn(permitInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());
            dataObj["sponsoredNationalityAr"] = nationalityLookup.GetAr(permitInfo.IndividualUserInfo.IndividualProfileInformation?.NationalityId?.ToString());
            dataObj["sponsoredNameEn"] = permitInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameEn;
            dataObj["sponsoredNameAr"] = permitInfo.IndividualUserInfo.IndividualProfileInformation?.FullNameAr;
            dataObj["sponsoredFileNo"] = ReportUtil.FormatPermitNo(permitInfo.IndividualUserInfo.IndividualResidenceInfo?.ResidenceNo);
            dataObj["sponsoredNoteEn"] = ReportUtil.GetNotes(travelTypeId, "en", 1, permitInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            dataObj["sponsoredNoteAr"] = ReportUtil.GetNotes(travelTypeId, "ar", 1, permitInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);

            string passportNumber = permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            string passportTypeEn = passportTypeLookup.GetEn(permitInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            string passportTypeAr = passportTypeLookup.GetAr(permitInfo.IndividualUserInfo.IndividualPassportInformation?.PassportTypeId.ToString());
            dataObj["sponsoredPassportNo"] = ReportUtil.FormatString(passportTypeEn, passportNumber);

            #endregion

            #region Sponsor Details

            dataObj["sponsorTypeEn"] = Lookups.LookupSponsorType.Instance.GetEn(sponsor.SponsorTypeId);
            dataObj["sponsorTypeAr"] = Lookups.LookupSponsorType.Instance.GetAr(sponsor.SponsorTypeId);
            dataObj["sponsorNameAr"] = sponsor.SponsorNameAr;
            dataObj["sponsorNameEn"] = sponsor.SponsorNameEn;
            dataObj["sponsorFileNo"] = sponsor.SponsorFileNumber;
            dataObj["sponsorPassportNo"] = sponsor.SponsorPassportNo;
            dataObj["sponsorPhoneNo"] = sponsor.SponsorPhoneNumber;
            dataObj["sponsorNoteEn"] = ReportUtil.GetNotes(travelTypeId, "en", 2, permitInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);
            dataObj["sponsorNoteAr"] = ReportUtil.GetNotes(travelTypeId, "ar", 2, permitInfo.IndividualUserInfo.IndividualResidenceInfo?.ValidityDate?.ToString("dd-MM-yyyy"), travelDate, remainingDays);

            #endregion

            #region Other Details
            dataObj["cancellationReasonAr"] = "طلب الراعي";
            dataObj["cancellationReasonEn"] = "Sponsor Request";
            dataObj["visaTypeAr"] = "إلغــاء إقامــــة";
            dataObj["visaTypeEn"] = "Residence Cancellation";
            dataObj["accompanyCount"] = 0;
            dataObj["cancellationDate"] = permitInfo.IndividualUserInfo.IndividualResidenceInfo?.CancelDate?.ToString("dd-MM-yyyy");
            dataObj["banReasonEn"] = string.Empty;
            dataObj["banReasonAr"] = string.Empty;
            dataObj["printedDate"] = DateTime.Now.ToString("dd-MM-yyyy"); 
            #endregion

            return dataObj;
        }
    }
}
