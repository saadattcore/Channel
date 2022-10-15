using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestApplicationStatusWrapper")]
    [DataContract]
    public class RestApplicationStatusWrapper
    {
        [DataMember(Name = "applicationStatus")]
        public List<RestApplicationStatus> lstApplicationStatus { get; set; }
    }
}
