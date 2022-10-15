using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RSAAuthenticationRequest")]
    [DataContract]
    public class RSAAuthenticationRequest
    {
        [DataMember(Name = "oldToken")]
        public string oldToken { get; set; }

        [DataMember(Name = "token")]
        public string token { get; set; }
        [DataMember(Name = "pin")]
        public string pin { get; set; }
        [DataMember(Name = "deviceId")]
        public string deviceId { get; set; }

    }
}
