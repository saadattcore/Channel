using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Services.Reports.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    public class TravelReportPdfInteface : IProduceReport
    {
        private TravelReportPdfInteface() { }
        public static IProduceReport Create()
        {
            return new TravelReportPdfInteface();
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

            Stream report = null;

           // short fileType = 2;

            
            if (fileType == (short)ServiceType.Residence)
            {
                report = reportGeneratorPdf.BuilReport(reportData, ReportConstants.TravelReportResidence, ReportConstants.ReportingServiceUrlPdf).Result;
            }
            else if (fileType == (short)ServiceType.EntryPermit)
            {
                report = reportGeneratorPdf.BuilReport(reportData, ReportConstants.TravelReportPermit, ReportConstants.ReportingServiceUrlPdf).Result;
            }

            
            return report;          
        }
       
    }
}