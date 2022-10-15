using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [SwaggerWcfDefinition(Name = "RestReportsAllowed")]
    [DataContract]
    public class RestReportsAllowed
    {
        [DataMember(Name = "serviceId")]
        public string ServiceId { get; set; }

        [DataMember(Name = "serviceResourceKey")]
        public string ServiceResourceKey { get; set; }
    }
}
