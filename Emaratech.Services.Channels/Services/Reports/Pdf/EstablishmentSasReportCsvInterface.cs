using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    public class EstablishmentSasReportCsvInterface : ReportGenerator, IProduceReport
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EstablishmentSasReportCsvInterface));


        public class EstReport
        {
            public string name { get; set; }
            public string fileno { get; set; }
            public IList<EntryPermit> visa { get; set; }
            public IList<Residence> resident { get; set; }
        }

        public class EntryPermit
        {
            public string entrypermitNo { get; set; }
            public string sno { get; set; }
            public string visaType { get; set; }
            public string name { get; set; }
            public string udbNo { get; set; }
            public string gender { get; set; }
            public string passNo { get; set; }
            public string nationality { get; set; }
            public string profession { get; set; }
            public string issueDate { get; set; }
            public string expDate { get; set; }
            public string status { get; set; }
            public string closedDate { get; set; }
            public string cancelDate { get; set; }
            public string lastTravelDate { get; set; }
            public string lastTravelType { get; set; }
        }

        public class Residence
        {
            public string visaNo { get; set; }
            public string sno { get; set; }
            public string visaType { get; set; }
            public string name { get; set; }
            public string udbNo { get; set; }
            public string gender { get; set; }
            public string passNo { get; set; }
            public string nationality { get; set; }
            public string profession { get; set; }
            public string issueDate { get; set; }
            public string expDate { get; set; }
            public string status { get; set; }
            public string closedDate { get; set; }
            public string cancelDate { get; set; }
            public string lastTravelDate { get; set; }
            public string lastTravelType { get; set; }
        }
        public sealed class EntryPermitClassMap : CsvClassMap<EntryPermit>
        {
            public EntryPermitClassMap()
            {
                Map(m => m.sno).Index(0).Name("التسلسل رقم");
                Map(m => m.entrypermitNo).Index(0).Name("رقم اذن الدخول");
                Map(m => m.visaType).Index(0).Name("نوع التأشيرة");
                Map(m => m.name).Index(0).Name("الاسم");
                Map(m => m.udbNo).Index(0).Name("الرقم الموحد");
                Map(m => m.gender).Index(0).Name("الجنس");
                Map(m => m.passNo).Index(0).Name("رقم الجواز");
                Map(m => m.nationality).Index(0).Name("الجنسية");
                Map(m => m.profession).Index(0).Name("المهنة");
                Map(m => m.issueDate).Index(0).Name("تاريخ الاصدار");
                Map(m => m.lastTravelDate).Index(0).Name("رحلة تاريخ آخر");
                Map(m => m.expDate).Index(0).Name("تاريخ الانتهاء");
                Map(m => m.cancelDate).Index(0).Name("تاريخ الالغاء");
                Map(m => m.closedDate).Index(0).Name("تاريخ الاغلاق");
                Map(m => m.status).Index(0).Name("الوضعية");

            }
        }
        public sealed class ResidenceClassMap : CsvClassMap<Residence>
        {
            public ResidenceClassMap()
            {
                Map(m => m.sno).Index(0).Name("التسلسل رقم");
                Map(m => m.visaNo).Index(0).Name("رقم اذن الدخول");
                Map(m => m.visaType).Index(0).Name("نوع التأشيرة");
                Map(m => m.name).Index(0).Name("الاسم");
                Map(m => m.udbNo).Index(0).Name("الرقم الموحد");
                Map(m => m.gender).Index(0).Name("الجنس");
                Map(m => m.passNo).Index(0).Name("رقم الجواز");
                Map(m => m.nationality).Index(0).Name("الجنسية");
                Map(m => m.profession).Index(0).Name("المهنة");
                Map(m => m.issueDate).Index(0).Name("تاريخ الاصدار");
                Map(m => m.lastTravelDate).Index(0).Name("رحلة تاريخ آخر");
                Map(m => m.expDate).Index(0).Name("تاريخ الانتهاء");
                Map(m => m.cancelDate).Index(0).Name("تاريخ الالغاء");
                Map(m => m.closedDate).Index(0).Name("تاريخ الاغلاق");
                Map(m => m.status).Index(0).Name("الوضعية");

            }
        }

        public IProduceReport SetTypeExcel()
        {
            return this;
        }

        public IProduceReport SetTypePdf()
        {
            return this;
        }



        private Task<Stream> ProcessWithSharpZipLib(IDictionary<string,Stream> streams )
        {
            var memory = new MemoryStream();

            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(memory);
            zipOutputStream.SetLevel(0); //0-9, 9 being the highest level of compression
            zipOutputStream.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Off;

            foreach(var data in streams)
            {
                var stream = data.Value;

                ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(ICSharpCode.SharpZipLib.Zip.ZipEntry.CleanName(data.Key));

                zipOutputStream.PutNextEntry(entry);
                stream.CopyTo(zipOutputStream);
                stream.Close();
                zipOutputStream.CloseEntry();

            }
            zipOutputStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
            zipOutputStream.Close();
            memory.Seek(0, SeekOrigin.Begin)
                ;
            return Task.FromResult((Stream)memory);
        }


        public Stream AsStream(string reportData, WebOperationContext currentContext, short fileType)
        {
            if (currentContext != null)
            {
                currentContext.OutgoingResponse.ContentType = ReportConstants.Csv;
                currentContext.OutgoingResponse.Headers.Add("Content-Disposition", 
                $"attachment; filename=gdrfa-report.csv")
                ;
            }


            EstReport report = JsonConvert.DeserializeObject<EstReport>(reportData);

            Log.Debug($"Generating report for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");
            var memory = new MemoryStream();

            Log.Debug($"Generating permites for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");

            using (var writer = new CsvWriter(new StreamWriter(memory, Encoding.UTF8, 4096, true)))
            {
                writer.Configuration.RegisterClassMap<EntryPermitClassMap>();
                writer.WriteHeader<EntryPermit>();
                writer.WriteRecords(report.visa);
            }
            Log.Debug($"Finish Generating permits for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");

            
            Log.Debug($"Generating residents for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");

            using (var writer = new CsvWriter(new StreamWriter(memory, Encoding.UTF8, 4096, true)))
            {
                writer.Configuration.RegisterClassMap<ResidenceClassMap>();
                writer.WriteHeader<Residence>();
                writer.WriteRecords(report.resident);
            }
            Log.Debug($"Finish Generating residents for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");


            memory.Seek(0, SeekOrigin.Begin);

            Log.Debug($"Starting Generating zip for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");
            
           

            Log.Debug($"Finish Generating report for {report.fileno} with permits {report.visa?.Count} and residencies {report.resident?.Count}");

            return memory;
        }

    }
}