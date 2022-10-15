using SwaggerWcf.Attributes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [SwaggerWcfDefinition(Name = "ReportsRequest")]
    [DataContract]
    public class RestReportsRequest
    {
        [DataMember(Name = "ids")]
        public IList<string> Ids { get; set; }

        [DataMember(Name = "reportType")]
        public string ReportType { get; set; }
    }
}
