using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestDocumentInfo")]
    [DataContract]
    public class RestDocumentInfo
    {
        [DataMember(Name = "documentTypeId")]
        public string DocumentTypeId { get; set; }
        [DataMember(Name = "documentId")]
        public string DocumentId { get; set; }
        [DataMember(Name = "mimeType")]
        public string MimeType { get; set; }
        [DataMember(Name = "size")]
        public int Size { get; set; }
        [DataMember(Name = "width")]
        public int Width { get; set; }
        [DataMember(Name = "height")]
        public int Height { get; set; }
        [DataMember(Name = "resourceKey")]
        public string ResourceKey { get; set; }
        [DataMember(Name = "required")]
        public bool Required { get; set; }
        [DataMember(Name = "documentName")]
        public string Name { get; set; }
    }
}