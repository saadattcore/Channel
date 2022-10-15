using System.IO;
using System.ServiceModel.Web;
using Emaratech.Services.Channels.Services.Reports.Excel;
using System;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    public class SasReportPdfInteface : ReportGenerator, IProduceReport
    {
        private SasReportPdfInteface() { }
        public static IProduceReport Create()
        {
            return new SasReportPdfInteface();
        }
        public IProduceReport SetTypeExcel()
        {
            return ExcelReportInteface.Create();
        }

        public IProduceReport SetTypePdf()
        {
            return this;
        }


        public Stream AsStream(string reportData, WebOperationContext currentContext, short fileType)
        {
            var reportGeneratorPdf = new GeneratorPdf();
            currentContext.OutgoingResponse.ContentType = ReportConstants.Pdf;

            // short fileType = 2;
            var report = BuilReport(reportData, ReportConstants.SasReport, ReportConstants.ReportingServiceUrlPdf).Result;


            return report;
        }
    }
}