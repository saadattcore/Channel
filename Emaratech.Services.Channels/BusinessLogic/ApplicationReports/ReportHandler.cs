namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Emaratech.Services.Application.Model;
    using Emaratech.Services.Channels.Services.Reports;
    using log4net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;
    using Models.Enums;
    using Helpers;
    using Contracts.Rest.Models.Enums;
    using Newtonsoft.Json;

    public abstract class ReportHandler : IReportHandler
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(IReportHandler));

        public abstract Task<IList<ReportRecordInfo>> GetReportEntries(IList<string> ids);

        public string GenerateReport(IList<ReportRecordInfo> reports)
        {
            return new ReportGenerator().BuildMultipleRepots(
                        reports,
                        ReportConstants.ReportingServiceUrlMultiPdf);
        }

        protected string FormatString(string en, string ar)
        {
            var str = new StringBuilder();
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

        protected string GetFormattedPermitNo(string permitNo)
        {
            var permitFormatted = string.Empty;
            var dept = permitNo.Substring(0, 3);
            var year = "20" + permitNo.Substring(5, 2);
            var visaType = permitNo.Substring(3, 1).Equals("0") ? permitNo.Substring(4, 1) : permitNo.Substring(3, 2);
            var serialNo = permitNo.Substring(7, (permitNo.Count() - 7));

            permitFormatted = visaType + serialNo + "/" + year + "/" + dept;
            return permitFormatted;
        }

        protected string GetBarcodeNumber(string entryPermit)
        {
            var barcodeNumber = new StringBuilder();
            barcodeNumber.Append("070");
            barcodeNumber.Append(entryPermit);
            return barcodeNumber.ToString();
        }

        protected string GetApplicantPhotoDocument(string applicationId)
        {
            var documnetList = ServicesHelper.GetApplicationDocuments(applicationId).Result;
            var documnetInfo = documnetList.FirstOrDefault(d => d.DocumentType == 
                        ConfigurationManager.AppSettings[Constants.ConfigurationKeys.PrintPhotoDocumnetId]);

            if (documnetInfo != null)
            {
                var sponsorPhotoDocInfo = ServicesHelper.GetDocument(documnetInfo.DocumenteId).Result;
                var docStr = sponsorPhotoDocInfo.DocumentStream;
                return docStr;
            }

            return string.Empty;
        }
    }
}