using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Services.Reports.Excel;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
       public class EstablishmentSasReportPdfInterface : ReportGenerator , IProduceReport 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EstablishmentSasReportPdfInterface));

           public static readonly int MaxSasReportDownloadSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSasReportDownloadSize"] ?? "4194304");

        private EstablishmentSasReportPdfInterface() { }
        public static IProduceReport Create()
        {
            return new EstablishmentSasReportPdfInterface();
        }
        public IProduceReport SetTypeExcel()
        {
            Log.Debug("Loding Establishment Excel");
            return EstablishmentSasReportExcelInterface.Create();
        }

        public IProduceReport SetTypePdf()
        {
            return this;
        }


        public Stream AsStream(string reportData, WebOperationContext currentContext, short fileType)
        {
            if ( reportData.Length > MaxSasReportDownloadSize)
            {
                return new EstablishmentSasReportCsvInterface().AsStream(reportData, currentContext, fileType);
            }
            currentContext.OutgoingResponse.ContentType = ReportConstants.Pdf;
            var report = BuilReport(reportData, ReportConstants.EstablishmentSasReport, ReportConstants.ReportingServiceUrlPdf).Result;
            
            return report;
           
        }

    
    }
}