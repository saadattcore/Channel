using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestSubmitAction")]
    [DataContract]
    public class RestSubmitActionRequest
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }
    }
}