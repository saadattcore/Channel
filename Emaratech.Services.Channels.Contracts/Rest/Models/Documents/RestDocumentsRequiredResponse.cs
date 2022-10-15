using System.Runtime.Serialization;
using SwaggerWcf.Attributes;
using System.Collections.Generic;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestDocumentsRequiredResponse")]
    [DataContract]
    public class RestDocumentsRequiredResponse : RestDocumentInfo
    {
        //[DataMember(Name = "documentContent")]
        //public string DocumentContent { get; set; }

        [DataMember(Name = "rejectionReasons")]
        public List<string> RejectionReasons { get; set; }

        [DataMember(Name = "documentId")]
        public string documentId { get; set; }
    }
}
