using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestApplicationStatusRequest")]
    [DataContract]
    public class RestApplicationStatusRequest
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }
    }
}
