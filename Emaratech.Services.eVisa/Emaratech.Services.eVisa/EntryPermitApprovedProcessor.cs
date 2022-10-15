using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Emaratech.Services.Application.Model;
using Emaratech.Services.eVisa.Lookups;
using Emaratech.Services.eVisa.Reports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    class EntryPermitApprovedProcessor : StatusProcessor
    {
        private ILookup nationalityLookup => LookupNationality.Instance;
        private ILookup visaTypeLookup => LookupVisaType.Instance;
        //private ILookup residenceStatusLookup => LookupResidenceStatus.Instance;

        //private ILookup permitStatusLookup => LookupPermitStatus.Instance;
        private ILookup professionLookup => LookupProfession.Instance;

        private ILookup passportTypeLookup => LookupPassportType.Instance;

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var id = ConfigurationSystem.AppSettings["EntryPermitNewCategory"];
            return ApplicationRepository.GetApprovedStatus(id);
        }

        private string GetApplicantPhotoDocument(string applicationId)
        {
            // var docList = ServicesHelper.GetApplicationApi.GetApplicationDocuments("2011134");
            var docList = ServicesHelper.GetApplicationApi.GetApplicationDocuments(applicationId);
            var docInfo = docList.FirstOrDefault(d => d.DocumentType == ConfigurationSystem.AppSettings["SponsoredPhotoDocId"]);
            if (docInfo != null)
            {
                var sponsorPhotoDocInfo = ServicesHelper.GetDocumentApi.GetDocument(docInfo.DocumenteId);
                var docStr = sponsorPhotoDocInfo.DocumentStream;

                // return Convert.FromBase64String(docStr);
                // return (new MemoryStream(Convert.FromBase64String(docStr)));
                return docStr;
            }
            return string.Empty;

        }

        private string FormatString(string en, string ar)
        {
            StringBuilder str = new StringBuilder();
            if (!string.IsNullOrEmpty(en))
            {
                str.Append(en);
            }
            if (!string.IsNullOrEmpty(ar))
            {
                str.Append(" / ");
                str.Append(ar);
            }
            return str.ToString();
        }

        private string GetFormattedPermitNo(string permitNo)
        {
            string permitFormatted = string.Empty;
            string dept = permitNo.Substring(0, 3);
            string year = "20" + permitNo.Substring(5, 2);
            string visaType = permitNo.Substring(3, 1).Equals("0") ? permitNo.Substring(4, 1) : permitNo.Substring(3, 2);
            string serialNo = permitNo.Substring(7, (permitNo.Count() - 7));

            permitFormatted = visaType + serialNo + "/" + year + "/" + dept;
            return permitFormatted;
        }

        public string GetBarcodeNumber(string entryPermit)
        {
            StringBuilder barcodeNum = new StringBuilder();
            barcodeNum.Append("070");
            barcodeNum.Append(entryPermit);
            return barcodeNum.ToString();
        }

        protected override string GetSubject(RestApplicationSearchRow application)
            => ReportUtil.eVisaReportName;

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            var data = DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues);

            string content = TemplateManager
                .GetTemplate(
                  data, "TemplateIdApprovedApplication"
                    );
            return content;
        }

        public JObject GenerateeVisa(string applicationId, string permitNumber)
        {
            var permitInfo = ServicesHelper.GetVisionApi.GetIndividualDetailedInformationByPermitNo(permitNumber);
            var indivPermitNo = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitNo;
            SponsorInfo sponsor = ReportUtil.GetSponsorInfo(permitInfo.IndividualUserInfo?.IndividualPermitInfo?.SponsorTypeId, permitInfo , FileType.EntryPermit);

            JObject dataObj = new JObject();

            var permitIssueDate = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitIssueDate.Value.ToString("dd/MM/yyyy");
            var permitIssuePlaceEn = "Dubai";
            var permitIssuePlaceAr = "دبي";
            dataObj["entryPermitNo"] = indivPermitNo;
            dataObj["dateandplace"] = permitIssueDate + " " + permitIssuePlaceEn;
            dataObj["dateandplaceAr"] = permitIssueDate + " " + permitIssuePlaceAr;

            dataObj["validity"] = permitInfo.IndividualUserInfo?.IndividualPermitInfo.PermitExpiryDate.Value.ToString("dd/MM/yyyy");
            dataObj["uidno"] = permitInfo.IndividualUserInfo?.IndividualProfileInformation.UDBNo;
            dataObj["fullname"] = permitInfo.IndividualUserInfo?.IndividualProfileInformation.FullNameEn;
            dataObj["fullNameAr"] = permitInfo.IndividualUserInfo?.IndividualProfileInformation.FullNameAr;

            dataObj["nationality"] = nationalityLookup.GetEn(permitInfo.IndividualUserInfo?.IndividualProfileInformation.NationalityId.ToString());
            dataObj["nationalityAr"] = nationalityLookup.GetAr(permitInfo.IndividualUserInfo?.IndividualProfileInformation.NationalityId.ToString());

            dataObj["placeofbirth"] = permitInfo.IndividualUserInfo?.IndividualProfileInformation.BirthPlaceEn;
            dataObj["placeofbirthAr"] = permitInfo.IndividualUserInfo?.IndividualProfileInformation.BirthPlaceAr;

            dataObj["dateofbirth"] = permitInfo.IndividualUserInfo?.IndividualProfileInformation.BirthDate.Value.ToString("dd/MM/yyyy");

            string passportNumber = permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            string passportTypeEn = passportTypeLookup.GetEn(permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportTypeId.ToString());
            string passportTypeAr = passportTypeLookup.GetAr(permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportTypeId.ToString());
            dataObj["passportno"] = FormatString(passportTypeEn, passportNumber);
            dataObj["passportnoAr"] = FormatString(passportTypeAr, passportNumber);

            dataObj["profession"] = professionLookup.GetEn(permitInfo.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());
            dataObj["professionAr"] = professionLookup.GetAr(permitInfo.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            dataObj["sponsorname"] = sponsor.SponsorNameEn;
            dataObj["sponsornameAr"] = sponsor.SponsorNameAr;

            dataObj["sponsoraddress"] = "TEL: " + sponsor.SponsorPhoneNumber + ", ADDRESS: " + sponsor.Address;
            dataObj["entrypermitnoformated"] = GetFormattedPermitNo(indivPermitNo);

            string visaTypeDesc = visaTypeLookup.GetAr(permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString()) + "\n" + visaTypeLookup.GetEn(permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString());

            dataObj["visatype"] = visaTypeDesc;
            dataObj["barcodevalue"] = GetBarcodeNumber(indivPermitNo);
            dataObj["barcodeentrypermitno"] = GetBarcodeNumber(indivPermitNo);
            dataObj["serialNo"] = applicationId;
            dataObj["wifename"] = "None";
            dataObj["childname"] = "None";

            return dataObj;
        }

        protected override string GetAttachment(RestApplicationSearchRow application)
        {
            string applicationId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
            string visaNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;

            var eVisaData = GenerateeVisa(applicationId, visaNumber);
            var jsonData = JsonConvert.SerializeObject(eVisaData);
            var userPhoto = GetApplicantPhotoDocument(applicationId);

            string eVisaMainReport = ReportUtil.eVisaReport;
            string eVisaInstrucitonsReport = ReportUtil.eVisaInstructionsReport;
            List<string> reportNames = new List<string>();
            reportNames.Add(eVisaMainReport);
            reportNames.Add(eVisaInstrucitonsReport);

            // this.Log.Debug($"Before calling built report method of util and extracted payload for attach {jsonData}");
            var reportContent = ReportUtil.BuildReport(jsonData, reportNames, ReportUtil.ReportingServiceUrlMultiPdf, userPhoto);
            // this.Log.Debug($"Reponse from reporting content *** {reportContent}");    
            return reportContent;
        }

    }
}