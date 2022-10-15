using System.Collections.Generic;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestSubmitDocumentRequest")]
    [DataContract]
    public class RestSubmitDocumentRequest
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "documentIds")]
        public IEnumerable<string> DocumentIds { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }
    }
}