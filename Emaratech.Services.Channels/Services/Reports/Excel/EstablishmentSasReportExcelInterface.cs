using Emaratech.Services.Channels.Services.Reports.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.Services.Reports.Excel
{
    public class EstablishmentSasReportExcelInterface : IProduceReport
    {
        private EstablishmentSasReportExcelInterface() { }
        public static IProduceReport Create()
        {
            return new EstablishmentSasReportExcelInterface();
        }

        public IProduceReport SetTypeExcel()
        {
            return this;
        }

        public IProduceReport SetTypePdf()
        {
            return EstablishmentSasReportPdfInterface.Create();
        }

        public Stream AsStream(string reportData, WebOperationContext currentContext, short fileType)
        {
            if ( reportData.Length > EstablishmentSasReportPdfInterface.MaxSasReportDownloadSize)
            {
                return new EstablishmentSasReportCsvInterface().AsStream(reportData, currentContext, fileType);
            }

            var reportGenerator = new GeneratorExcel();
            currentContext.OutgoingResponse.ContentType = ReportConstants.Excel;
            var report = reportGenerator.CreateResponseStream(reportData, ReportConstants.EstablishmentSasReportExcel);
            return report;
        }
    }
}