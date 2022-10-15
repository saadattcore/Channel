using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestSubmitApplicationRequest")]
    [DataContract]
    public class RestSubmitApplicationRequest
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "applicationData")]
        public string ApplicationData { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }
    }
}