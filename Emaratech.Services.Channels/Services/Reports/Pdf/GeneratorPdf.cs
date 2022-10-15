using System.IO;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    public class GeneratorPdf : ReportGenerator
    {

        ISasReport ReportGenerator { get { return SasReport.Instance; } }

        public Stream CreateResponseStream(string report)
        {
            var root = RootUtils.Deserialize(report);
            return ReportGenerator.Produce(root);
        }

        public Stream CreateResponseStreamTraveStatusReport(string report)
        {
            //var root = RootUtils.Deserialize(report);
            return ReportGenerator.ProduceTravelReport(report);
        }

       
    }
}