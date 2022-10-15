using System.Configuration;

namespace Emaratech.Services.Channels.Services.Reports
{
  
    public static class ReportConstants
    {
        public static readonly string SasReportServiceId = "367D5E45BECF4F67B96E8954A3CEEA03";
        public static readonly string TravelReportServiceId = "E3DFD3CDF2C3491B86A4D43915AAE47F";
        public static readonly string EstablishmentSasReportServiceId = "14537B0958C14DA998C05D24963DC331";

        public static readonly string TravelReportEmailTemplate = "AC52CE630CAB4EC6BE16B8D43E58909B";
        public static readonly string SasReportEmailTemplate = "52045308CDE547B291F6FFCBDBF9CC73";

        public static readonly string SasReport = ConfigurationManager.AppSettings["ReportsLocation"]+"spnsr_details.jasper";
        public static readonly string SasReportExcel = ConfigurationManager.AppSettings["ReportsLocation"] + "spnsr_details_xls.jasper";
        public static readonly string EstablishmentSasReport = ConfigurationManager.AppSettings["ReportsLocation"] +"spnsr_details_estab.jasper";
        public static readonly string EstablishmentSasReportExcel = ConfigurationManager.AppSettings["ReportsLocation"] + "spnsr_details_estab_xls.jasper";

        public static readonly string TravelReportResidence = ConfigurationManager.AppSettings["ReportsLocation"] +"TravelReportRes.jasper";
        public static readonly string TravelReportPermit = ConfigurationManager.AppSettings["ReportsLocation"] +"TravelReportVisa.jasper";
        public static readonly string EntryPermitReport = ConfigurationManager.AppSettings["ReportsLocation"] + "Visa_Visit.jasper";
        public static readonly string InstructionReport = ConfigurationManager.AppSettings["ReportsLocation"] + "Visa_Instructions.jasper";
        public static readonly string ReportingServiceUrlPdf = ConfigurationManager.AppSettings["ReportApiPdf"];
        public static readonly string ReportingServiceUrlExcel = ConfigurationManager.AppSettings["ReportApiExcel"];
        public static readonly string ReportingServiceUrlMultiPdf = ConfigurationManager.AppSettings["ReportApiPdfMulti"];

        public static readonly string ResdenceCancellationReport = ConfigurationManager.AppSettings["ReportsLocation"] + "ResidenceCancellation.jasper";
        public static readonly string EntryCancellationReport = ConfigurationManager.AppSettings["ReportsLocation"] + "ResidenceCancellation.jasper";

        public static readonly string EmailAccountId = "771640C526BA4804916329811C7129B5";
        public static readonly string EmailContentType = "text/html; charset=utf-8";
        public static readonly string EmailSubjectSasReport = "Sponsor And Sponsored Report";
        public static readonly string EmailSubjectTravelReport = "Certificate of Entry Exit";
        public static readonly string EmailSubjectEstablishmentSasReport = "Sponsor And Sponsored Report";
        public const string Pdf = "application/pdf";
        public const string Csv = "application/csv";
        public const string Excel = "application/vnd.ms-excel";//"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}