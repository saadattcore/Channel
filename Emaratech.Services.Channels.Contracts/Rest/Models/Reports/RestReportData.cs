using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [SwaggerWcfDefinition(Name = "RestReportsHistory")]
    [DataContract]
    public class RestReportData
    {
        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "reportData")]
        public Stream ReportContent { get; set; }
    }
}
