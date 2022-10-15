using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestUploadDocumentRequest")]
    [DataContract]
    public class RestUploadDocumentRequest
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }
        
        [DataMember(Name = "documentTypeId")]
        public string DocumentTypeId { get; set; }

        [DataMember(Name = "document")]
        public string Document { get; set; }

        [DataMember(Name = "documentName")]
        public string DocumentName { get; set; }
    }
}