using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestRefundResponse")]
    [DataContract]
    public class RestRefundResponse
    {
        [DataMember(Name = "status")]
        public bool Status { get; set; }

        [DataMember(Name = "batchId")]
        public string BatchId { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }
    }
}
