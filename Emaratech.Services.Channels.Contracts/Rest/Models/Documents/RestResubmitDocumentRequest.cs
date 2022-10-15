using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestResubmitDocumentRequest")]
    [DataContract]
    public class RestResubmitDocumentRequest
    {
        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "documentIds")]
        public IEnumerable<string> DocumentIds { get; set; }
    }
}