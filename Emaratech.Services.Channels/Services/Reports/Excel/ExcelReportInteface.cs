using System.IO;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Services.Reports.Pdf;

namespace Emaratech.Services.Channels.Services.Reports.Excel
{
    public class ExcelReportInteface : IProduceReport
    {
        private ExcelReportInteface() { }
        public static IProduceReport Create()
        {
            return new ExcelReportInteface();
        }

        public IProduceReport SetTypeExcel()
        {
            return this;
        }

        public IProduceReport SetTypePdf()
        {
            return SasReportPdfInteface.Create();
        }

        public Stream AsStream(string reportData, WebOperationContext currentContext, short fileType)
        {
            var reportGenerator = new GeneratorExcel();
            currentContext.OutgoingResponse.ContentType = ReportConstants.Excel;
            var report = reportGenerator.CreateResponseStream(reportData, ReportConstants.SasReportExcel);
            return report;
        }
    }
}