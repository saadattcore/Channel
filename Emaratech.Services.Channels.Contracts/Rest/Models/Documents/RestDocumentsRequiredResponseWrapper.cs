using System.Runtime.Serialization;
using SwaggerWcf.Attributes;
using System.Collections.Generic;
namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestDocumentsRequiredResponseWrapper")]
    [DataContract]
    public class RestDocumentsRequiredResponseWrapper
    {
        [DataMember(Name = "documents")]
        public List<RestDocumentsRequiredResponse> Documents { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }

        [DataMember(Name = "statusId")]
        public string StatusId { get; set; }
    }
}
