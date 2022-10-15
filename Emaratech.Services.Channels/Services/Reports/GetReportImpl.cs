using System.IO;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Services.Reports.Pdf;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using System.Collections.Generic;
using System;

namespace Emaratech.Services.Channels.Services.Reports
{
    public class GetReportImpl : ReportServiceImplBase
    {
        protected WebOperationContext WebContext { get; }
        private IProduceReport Generator { get; set; } // State
        private string RequestMimeFormat => WebContext?.IncomingRequest.Accept ?? ReportConstants.Pdf;

        private GetReportImpl(WebOperationContext webContext)
        {
            WebContext = webContext;
        }

        public static GetReportImpl Create(WebOperationContext webContext)
        {
            return new GetReportImpl(webContext);
        }

        public Stream AsPdfOrExcel(string applicationId, string userId)
        {
            var rawReport = FetchRawReport(applicationId, userId);

            if (rawReport == null || rawReport.ReportData == null)
                throw ChannelErrorCodes.ReportStillGenerating.ToWebFault($"System is generating the report, Request# {applicationId}.");

            //Individual SAS Report
            if (rawReport.ServiceId.Equals(ReportConstants.SasReportServiceId))
            {
                Generator = SasReportPdfInteface.Create();
                Generator = ReportConstants.Excel.Equals(RequestMimeFormat)
                    ? Generator.SetTypeExcel()
                    : Generator.SetTypePdf();
                return Generator.AsStream(rawReport.ReportData, WebContext, 0);
            }
            //Establishment SAS Report
            else if (rawReport.ServiceId.Equals(ReportConstants.EstablishmentSasReportServiceId))
            {
                Generator = EstablishmentSasReportPdfInterface.Create();
                Generator = ReportConstants.Excel.Equals(RequestMimeFormat)
                    ? Generator.SetTypeExcel()
                    : Generator.SetTypePdf();
                return Generator.AsStream(rawReport.ReportData, WebContext, 0);
            }
            //Individual Travel Report
            else if (rawReport.ServiceId.Equals(ReportConstants.TravelReportServiceId))
            {
                Generator = TravelReportPdfInteface.Create();
                Generator = Generator.SetTypePdf();
                return Generator.AsStream(rawReport.ReportData, WebContext, short.Parse(rawReport.FileType));
            }

            return null;
        }

        public Stream GetReportStreamResponse(string reportFullName, string reportContent)
        {
            var responseMessage = WebContext?.OutgoingResponse;
            if (responseMessage != null)
            {
                responseMessage.ContentType = ReportConstants.Pdf;
                responseMessage.Headers.Add("Content-Disposition",
                    $"attachment; filename={reportFullName}");
            }

            return new MemoryStream(Convert.FromBase64String(reportContent));
        }
    }
}