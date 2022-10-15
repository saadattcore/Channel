using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Channels.Contracts.Errors;
using log4net;
using Newtonsoft.Json;
using Emaratech.Services.Channels.BusinessLogic.ApplicationReports;

namespace Emaratech.Services.Channels.Services.Reports
{
    public class ReportGenerator
    {
        class ReportEntity
        {
            public string path { get; set; }
            public string data { get; set; }
            public Dictionary<string, string> parameters { get; set; }
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof (ReportGenerator));

        public async Task<Stream> BuilReport(string jsonData, string reportPath, string reportingServiceUrl)
        {
            //WebClient client = new WebClient();
            //client.Headers["Content-Type"] = "application/json;charset=utf-8";
            //client.Encoding = Encoding.UTF8;
            var body = new ReportEntity
            {
                path = reportPath, //"C:\\reports\\TravelReportRes.jasper",
                data = jsonData,
                parameters = new Dictionary<string, string> { { "DATE_PATTERN", "dd/MM/yyyy" } }
            };

            var json = JsonConvert.SerializeObject(body);
            //var utf8 = Encoding.UTF8.GetBytes(json);
            //Log.Debug($"Generating report with data {json}");

            using (var client1 = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client1.PostAsync(new Uri(reportingServiceUrl), content);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw ChannelErrorCodes.InternalServerError.ToWebFault(await result.Content.ReadAsStringAsync());
                }
                var stream = await result.Content.ReadAsStreamAsync();
                return stream;
            }

            //var data = client.UploadData(new Uri(reportingServiceUrl),
            //   utf8);
            //var strReportContent = System.Convert.ToBase64String(data);
            //return strReportContent;
        }

        public async Task<Stream> BuildMultipleRepotsStream(IList<ReportRecordInfo> reports, string reportingServiceUrl)
        {
            var client = new RestSharp.RestClient(reportingServiceUrl);

            var reportEntites = new List<ReportEntity>();

            foreach (var report in reports)
            {
                reportEntites.Add(GetReportEntity(report));

                if (report.HasInstructions)
                {
                    reportEntites.Add(GetInstructionsReport());
                }
            }
            var json = JsonConvert.SerializeObject(reportEntites);
            using (var client1 = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client1.PostAsync(new Uri(reportingServiceUrl), content);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw ChannelErrorCodes.InternalServerError.ToWebFault(await result.Content.ReadAsStringAsync());
                }
                var stream = await result.Content.ReadAsStreamAsync();
                return stream;
            }
        }

        public string BuildMultipleRepots(IList<ReportRecordInfo> reports, string reportingServiceUrl)
        {
            var client = new RestSharp.RestClient(reportingServiceUrl);

            var reportEntites = new List<ReportEntity>();

            foreach(var report in reports)
            {
                reportEntites.Add(GetReportEntity(report));

                if (report.HasInstructions)
                {
                    reportEntites.Add(GetInstructionsReport());
                }
            }

            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            
            request.AddJsonBody(reportEntites);

            var response = client.ExecuteAsPost(request, "POST");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string error = $"Error from jasper server, Error code = {response.StatusCode}, Error Content = {response.Content}, Requested URI = {response.ResponseUri}";
                throw ChannelErrorCodes.InternalServerError.ToWebFault(error);
            }

            var responseBytes = response.RawBytes;
            return Convert.ToBase64String(responseBytes);
        }

        private ReportEntity GetReportEntity(ReportRecordInfo report)
        {
            var reportEntity = new ReportEntity
            {
                path = report.ReportPath,
                data = report.ReportData,
                parameters = new Dictionary<string, string>()
            };

            if(report.IsRequiredDateParameter)
            {
                reportEntity.parameters.Add("DATE_PATTERN", "dd/MM/yyyy");
            }

            if (report.IsRequiredPhotoParameter)
            {
                reportEntity.parameters.Add("PERSON_PHOTO", report.UserPhoto);
            }

            return reportEntity;
        }

        private ReportEntity GetInstructionsReport()
        {
            var instructionsReport = new ReportEntity();
            instructionsReport.data = "{}";
            instructionsReport.path = ReportConstants.InstructionReport;
            instructionsReport.parameters = new Dictionary<string, string> { { "DATE_PATTERN", "dd/MM/yyyy" } };
            return instructionsReport;
        }
    }
}