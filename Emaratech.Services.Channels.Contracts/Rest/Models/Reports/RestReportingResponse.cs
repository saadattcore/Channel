using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [SwaggerWcf.Attributes.SwaggerWcfDefinition(Name = "RestReportingResponse")]
    [DataContract]
    public class RestReportingResponse
    {
        [DataMember(Name = "ReportingResponse")]
        public IList<RestKeyValueList> _ReportingResponse { get; set; }
    }
}
