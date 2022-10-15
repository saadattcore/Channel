using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestPayApplication")]
    [DataContract]
    public class RestPayApplication
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "request")]
        public string Request { get; set; }
    }
}
