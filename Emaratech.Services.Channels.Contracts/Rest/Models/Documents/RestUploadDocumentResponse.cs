using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestUploadDocumentResponse")]
    [DataContract]
    public class RestUploadDocumentResponse
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "documentId")]
        public string DocumentId { get; set; }
    }
}