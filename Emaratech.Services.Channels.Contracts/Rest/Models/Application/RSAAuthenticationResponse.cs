using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RSAAuthenticationResponse")]
    [DataContract]
   public class RSAAuthenticationResponse
    {
       
      
            [DataMember(Name = "StatusId")]
            public string StatusId { get; set; }

            [DataMember(Name = "StatusDesc")]
            public string StatusDesc { get; set; }
        
    }
}
