using Emaratech.Services.Application.Model;
using Emaratech.Services.Common.Configuration;
using Emaratech.Services.eVisa.Lookups;
using Emaratech.Services.eVisa.Reports;
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
    class OnArrivalVisaProcessor : StatusProcessor
    {
        private ILookup nationalityLookup => LookupNationality.Instance;
        private ILookup visaTypeLookup => LookupVisaType.Instance;
        private ILookup professionLookup => LookupProfession.Instance;
        private ILookup passportTypeLookup => LookupPassportType.Instance;

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var id = ConfigurationSystem.AppSettings["EntryPermitExtendCategory"];
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
            => ReportUtil.onArrivalVisaExtensionSubject;

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            string content = TemplateManager
                .GetTemplate(
                    DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues), "TemplateOnArrivalVisaExtension"
                    );
            return content;
        }

        public JObject GenerateeVisa(string applicationId, string permitNumber , string transactionBatchId)
        {
            // string permitNo = "2010314400247";
            //string residenceNo = "20120127176057";
            // var vision = new Vision.Api.VisionIndividualApi(ConfigurationSystem.AppSettings["VisionApi"]);

            var permitInfo = ServicesHelper.GetVisionApi.GetIndividualDetailedInformationByPermitNo(permitNumber);
            var indivPermitNo = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitNo;
            JObject dataObj = new JObject();

            var permitIssueDate = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitIssueDate.Value.ToString("dd/MM/yyyy");
            var permitIssuePlaceEn = "Dubai";
            var permitIssuePlaceAr = "دبي";

            dataObj["entryPermitNo"] = indivPermitNo;//getFormattedPermitNo(indivPermitNo);//permitIssueDate + " " + permitIssuePlaceEn;
            dataObj["dateandplace"] = permitIssueDate + " " + permitIssuePlaceEn;
            dataObj["dateandplaceAr"] = permitIssueDate + " " + permitIssuePlaceAr;

            dataObj["extendedUpTo"] = permitInfo.IndividualUserInfo?.IndividualPermitInfo.PermitExpiryDate.Value.ToString("dd/MM/yyyy");

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

            var sponsorMobileNo = permitInfo.IndividualSponsorInfo?.ContactInfo.FirstOrDefault(c => c.ContactTypeId == ContactType.Mobile.GetHashCode());
           // var sponsorNo = permitInfo.IndividualSponsorInfo?.SponsorshipInfo.SponsorshipNo;

            var estCode = permitInfo.IndividualUserInfo.IndividualPermitInfo?.EstCode;
            var sponsorInfo = ServicesHelper.GetVisionEstablishmentApi.GetEstablishmentProfile(estCode);

            dataObj["sponsorname"] = !string.IsNullOrEmpty(sponsorInfo?.EstabNameEn) ? sponsorInfo.EstabNameEn : string.Empty;
            dataObj["sponsornameAr"] = !string.IsNullOrEmpty(sponsorInfo?.EstabNameAr) ? sponsorInfo.EstabNameAr : string.Empty;
            dataObj["sponsoraddress"] = ReportUtil.FormatSponsorAddress(sponsorMobileNo?.CONTACTDETAIL, sponsorInfo?.AddressEn);
            dataObj["entrypermitnoformated"] = GetFormattedPermitNo(indivPermitNo);

            string visaTypeDesc = visaTypeLookup.GetAr(permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString()) + "\n" + visaTypeLookup.GetEn(permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString());
            dataObj["visatype"] = visaTypeDesc;//visaTypeLookup.GetEn(permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString());          

            dataObj["barcodevalue"] = GetBarcodeNumber(indivPermitNo);//permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId;
            var entrypermit = GetBarcodeNumber(indivPermitNo);
            entrypermit = String.Join("  ", entrypermit.Select(c => c));
            dataObj["barcodeentrypermitno"] = entrypermit ;//indivPermitNo;//permitInfo.IndividualUserInfo.IndividualPermitInfo.VisaTypeId;
            dataObj["serialNo"] = transactionBatchId + "/1"; 
            dataObj["wifename"] = "None";
            dataObj["childname"] = "None";

            dataObj["notesEn"] = "ENJOY YOUR VISIT & LEAVE BEFORE YOUR VISA EXPIRES SO WE CAN WELCOME YOU AGAIN";
            dataObj["notesAr"] = "تمتع بزيارتك و غادر قبل انتهائها ليتم الترحيب بك مرة أخرى";

            return dataObj;
        }

        protected override string GetAttachment(RestApplicationSearchRow application)
        {
            string applicationId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
            string visaNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;
            string transactionBatchId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "transactionbatchid")?.Value;
                       

            var eVisaData = GenerateeVisa(applicationId, visaNumber, transactionBatchId);
            var jsonData = JsonConvert.SerializeObject(eVisaData);
            var userPhoto = GetApplicantPhotoDocument(applicationId);

            string eVisaMainReport = ReportUtil.eOnArrivalVisaExtension;
            //string eVisaInstrucitonsReport = ReportUtil.eVisaInstructionsReport;

            List<string> reportNames = new List<string>();
            reportNames.Add(eVisaMainReport);
            //reportNames.Add(eVisaInstrucitonsReport);

            var reportContent = BuildReport(jsonData, reportNames, ReportUtil.ReportingServiceUrlMultiPdf, userPhoto);
            return reportContent;
        }

        private string BuildReport(string jsonData, List<string> reportNames, string reportingServiceUrl, string userPhoto)
        {
            RestSharp.RestClient client = new RestSharp.RestClient(reportingServiceUrl);

            var reportList = new List<ReportEntity>();

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("DATE_PATTERN", "dd/MM/yyyy");
            parameters.Add("PERSON_PHOTO", userPhoto);

            var reportObj = new ReportEntity();
            reportObj.data = jsonData;
            reportObj.path = reportNames[0];
            reportObj.parameters = parameters;

            reportList.Add(reportObj);

            RestSharp.RestRequest request = new RestSharp.RestRequest(RestSharp.Method.POST);
            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(reportList);
            request.AddJsonBody(reportList);
            var response = client.ExecuteAsPost(request, "POST");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string error = $"Error from jasper server Error code = {response.StatusCode} | Error Content = {response.Content} | Requested URI = {response.ResponseUri}";
                throw new Exception(error);
            }

            var reportByteArray = response.RawBytes;
            var strReportContent = System.Convert.ToBase64String(reportByteArray);
            return strReportContent;

        }


    }
}
