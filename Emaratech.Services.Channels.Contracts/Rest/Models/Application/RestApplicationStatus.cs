using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestApplicationStatus")]
    [DataContract]
    public class RestApplicationStatus
    {
        [DataMember(Name = "statusCode")]
        public string Status_Code { get; set; }

        [DataMember(Name = "statusDesc")]
        public RestName StatusDesc { get; set; }
    }
}
