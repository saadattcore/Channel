using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Application.Model;
using log4net;

namespace Emaratech.Services.Channels.Services.Reports
{
    public class ReportServiceImplBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ReportServiceImplBase));

        protected  RestReport FetchRawReport(string applicationId, string userId)
        {
            Log.Debug($"Starting to get report for {applicationId}");
            var api = ApiFactory.Default.GetApplicationApi();
            var report = api.GetReport(userId, applicationId);

            var memory = new MemoryStream(Convert.FromBase64String(report.ReportData));
            GZipStream stream = new GZipStream(memory, CompressionMode.Decompress);
            var newUncompressed = new MemoryStream();
            stream.CopyTo(newUncompressed);
            stream.Close();
            report.ReportData = Encoding.UTF8.GetString(newUncompressed.GetBuffer());
            newUncompressed.Close();



            Log.Debug($"Ending to get report for {applicationId}");
            return report;
        }
    }
}