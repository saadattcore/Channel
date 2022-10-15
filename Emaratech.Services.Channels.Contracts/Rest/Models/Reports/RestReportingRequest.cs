using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [SwaggerWcfDefinition(Name = "RestReportingRequest")]
    [DataContract]
    public class RestReportingRequest
    {
        [DataMember(Name = "conenctionString")]
        public string ConenctionString { get; set; }
        [DataMember(Name = "query")]
        public string Query { get; set; }
    }
}
