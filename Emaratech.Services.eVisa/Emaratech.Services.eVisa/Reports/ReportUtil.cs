using Emaratech.Services.Common.Configuration;
using Emaratech.Services.Vision.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Reports
{
    public static class ReportUtil
    {
        public static readonly string SasReportServiceId = "367D5E45BECF4F67B96E8954A3CEEA03";
        public static readonly string TravelReportServiceId = "E3DFD3CDF2C3491B86A4D43915AAE47F";
        public static readonly string EstablishmentSasReportServiceId = "14537B0958C14DA998C05D24963DC331";

        private static readonly string eReportPath = ConfigurationSystem.AppSettings["ReportPath"];
        public static readonly string eVisaReport = System.IO.Path.Combine(eReportPath, ConfigurationSystem.AppSettings["EntryPermitReportName"]);
        public static readonly string eResidenceCancelReport = System.IO.Path.Combine(eReportPath, ConfigurationSystem.AppSettings["ResidenceCancelReportName"]);
        public static readonly string eOnArrivalVisaExtension = System.IO.Path.Combine(eReportPath, ConfigurationSystem.AppSettings["OnArrivalVisaExtensionReportName"]);
        public static readonly string eEntryPermitCancelReport = "C:\\reports\\";
        public static string eVisaInstructionsReport
        {
            get
            {
                return System.IO.Path.Combine(eReportPath, ConfigurationSystem.AppSettings["InstructionReportName"]);
            }
        }

        public static readonly string ReportingServiceUrlPdf = ConfigurationSystem.AppSettings["ReportApiPdf"];
        public static readonly string ReportingServiceUrlExcel = ConfigurationSystem.AppSettings["ReportApiExcel"];
        public static readonly string ReportingServiceUrlMultiPdf = ConfigurationSystem.AppSettings["ReportApiPdfMulti"];
        public static readonly string eVisaReportName = "Entry Permit New Application Approved";
        public static readonly string entryPermitRejectionReportName = "REJECTION";

        public static readonly string entryPermitNewRejectedSubject = "Entry Permit New Application Rejected";
        public static readonly string entryPermitCancelApprovedSubject = "Entry Permit Cancel Application No:{0}";
        public static readonly string residenceNewApprovedSubject = "Residence Application Approved";
        public static readonly string residenceNewRejectedSubject = "Residence Application Rejected";
        public static readonly string residenceCancelApprovedSubject = "Residence Application Cancelled";
        public static readonly string residenceRenewApprovedSubject = "Renew Residence Application Approved";
        public static readonly string residenceRenewRejectedSubject = "Renew Residence Rejected";
        public static readonly string entryPermitPosted = "Entry Permit Application No: {0} posted succesfully";
        public static readonly string onArrivalVisaExtensionSubject = "On Arrival Visa Extension Approved";


        internal static string FormatString(string en, string ar)
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
        internal static string GetApplicantPhotoDocument(string applicationId)
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
        internal static string BuildReport(string jsonData, List<string> reportNames, string reportingServiceUrl, string userPhoto)
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

            var reportObj1 = new ReportEntity();
            reportObj1.data = "{}";
            reportObj1.path = reportNames[1];
            reportObj1.parameters = new Dictionary<string, string> { { "DATE_PATTERN", "dd/MM/yyyy" } };


            reportList.Add(reportObj);
            reportList.Add(reportObj1);


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


        internal static string BuildReportWithoutInstruction(string jsonData, List<string> reportNames, string reportingServiceUrl, string userPhoto)
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
    

    internal static string FormatPermitNo(string permitNo)
    {
        if (string.IsNullOrEmpty(permitNo))
            return string.Empty;

        return $"{permitNo.Substring(0, 3)}/{permitNo.Substring(3, 4)}/{permitNo.Substring(7)}";
    }

    internal static string FormatSponsorAddress(string telphone, string address)
    {
        if (string.IsNullOrEmpty(address) || address.Equals("."))
            return string.Empty;

        StringBuilder formattedAddress = new StringBuilder();

        formattedAddress.Append("ADDRESS: ");
        formattedAddress.Append(address);

        if (!string.IsNullOrEmpty(telphone))
        {
            formattedAddress.Append("TEL: ");
            formattedAddress = formattedAddress.Append(telphone);
        }

        return formattedAddress.ToString();
    }

    internal static string GetNotes(int travelTypeId, string locale, int noteType, string validatyDateEn, string travelDateEn, int? noOfDays)
    {
        string note = string.Empty;

        switch (noteType)
        {
            case 1:
                if (travelTypeId == (int)TravelType.Entry)
                {

                    if (locale == "en")
                    {
                        if (noOfDays == 0)
                            note = $"Note: must leave the country or change status up to: {System.DateTime.Now.ToString("dd-MM-yyyy")} . If the person does not comply with this, he will bear all legal procedures.";
                        else
                            note = $"Note: must leave the country or change status up to: {validatyDateEn} . If the person does not comply with this, he will bear all legal procedures.";
                    }
                    else if (locale == "ar")
                    {
                        if (noOfDays == 0)
                            note = $"يجب مغادرة الدولة أو تعديل الوضع بحد أقصى بتاريخ: { System.DateTime.Now.ToString("dd-MM-yyyy")} وفي حال عدم الالتزام بذلك يتحمل المذكور أعلاه كافة الإجراءات القانونية المترتبة على ذلك";
                        else
                            note = $"يجب مغادرة الدولة أو تعديل الوضع بحد أقصى بتاريخ: {validatyDateEn } وفي حال عدم الالتزام بذلك يتحمل المذكور أعلاه كافة الإجراءات القانونية المترتبة على ذلك";
                    }
                }
                else if (travelTypeId == (int)TravelType.Exit)
                {
                        if (locale == "en")
                            note = $"Note: the person has left the country on:{travelDateEn}";
                        else if (locale == "ar")
                        {
                            if(!string.IsNullOrEmpty(travelDateEn))
                            {

                            }
                            note = $"{travelDateEn} :ملاحظة: غادر المذكور أعلاه الدولة بتاريخ ";
                        }
                }
                break;

            case 2:
                if (travelTypeId == (int)TravelType.Entry)
                {
                    if (locale == "en")
                        note = $"The sponsor complies to notify GDRFA-D if the sponsored doesn’t leave the country or change status within ({noOfDays}) day(s) from the date of cancellation, and to bear all legal actions against it.";
                    else if (locale == "ar")
                        note = $"يتعهد الكفيل بإبلاغ الإدارة في حال عدم مغادرة المكفول أو تعديل وضعه خلال ({noOfDays}) يوم من تاريخ الإلغاء، وأن يتحمل كافة الإجراءات القانونية تجاه ذلك";
                }
                else
                    note = string.Empty;
                break;

            default:
                break;
        }

        return note;
    }

    internal static int CalculateNoOfDays(DateTime? validityDate, DateTime? cancellationDate)
    {
        int remainingDays = (validityDate.Value - DateTime.Now).Days;

        if (remainingDays > 30)
        {
            remainingDays = 30;
        }
        if (remainingDays < 0)
        {
            remainingDays = 0;
        }
        return remainingDays;
    }

    internal static SponsorInfo GetSponsorInfo(int? sponsorTypeId, RestIndividualUserFormInfo permitInfo, FileType fileType)
    {
        SponsorInfo sponsor = new SponsorInfo();

        if (sponsorTypeId == (int?)SponsorType.Establishment)
        {
            string estCode = FileType.EntryPermit == fileType ? permitInfo.IndividualUserInfo?.IndividualPermitInfo?.EstCode
                   : permitInfo.IndividualUserInfo?.IndividualResidenceInfo?.EstCode;

            var sponsorInfo = ServicesHelper.GetVisionEstablishmentApi.GetEstablishmentProfile(estCode);

            if (sponsorInfo != null)
            {
                sponsor.SponsorNameEn = sponsorInfo.EstabNameEn;
                sponsor.SponsorNameAr = sponsorInfo.EstabNameAr;
                sponsor.SponsorFileNumber = estCode;
                sponsor.SponsorTypeId = sponsorInfo.EstabTypeId?.ToString();
                sponsor.SponsorPhoneNumber = sponsorInfo.TelephoneNo;
            }
        }
        else
        {
            var sponsorNo = permitInfo.IndividualSponsorInfo?.SponsorshipInfo.SponsorshipNo;
            var sponsorInfo = ServicesHelper.GetVisionApi.GetIndividualDetailedInformationBySponsorNo(sponsorNo);

            sponsor.SponsorNameEn = permitInfo.IndividualSponsorInfo?.ProfileInfo?.FullNameEn;
            sponsor.SponsorNameAr = permitInfo.IndividualSponsorInfo?.ProfileInfo?.FullNameAr;
            sponsor.SponsorFileNumber = permitInfo.IndividualSponsorInfo.SponsorshipInfo?.SponsorshipNo;
            sponsor.SponsorTypeId = permitInfo.IndividualSponsorInfo?.SponsorshipInfo?.SponsorshipFileTypeId?.ToString();
            sponsor.SponsorPhoneNumber = permitInfo.IndividualSponsorInfo.ContactInfo?.FirstOrDefault(c => c.ContactTypeId == ContactType.Mobile.GetHashCode())?.CONTACTDETAIL;

            if (sponsorInfo != null)
            {
                sponsor.SponsorPassportNo = sponsorInfo.IndividualUserInfo.IndividualPassportInformation?.PassportNumber;
                sponsor.Address = sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(a => a.AddressTypeId == AddressType.Inside.GetHashCode())?.Address1;
            }

        }

        return sponsor;
    }
}
}
