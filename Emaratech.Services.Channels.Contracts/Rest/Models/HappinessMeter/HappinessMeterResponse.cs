using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.HappinessMeter
{
    [SwaggerWcfDefinition(Name = "HappinessMeterResponse")]
    [DataContract]
    public class HappinessMeterResponse
    {
        [DataMember(Name = "postURL")]
        public string PostURL { get; set; }

        [DataMember(Name = "signature")]
        public string Signature { get; set; }

        [DataMember(Name = "jsonRequest")]
        public string JsonRequest { get; set; }

        [DataMember(Name = "random")]
        public string Random { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }

        [DataMember(Name = "nonce")]
        public string Nonce { get; set; }

        [DataMember(Name = "lang")]
        public string Lang { get; set; }

        [DataMember(Name = "clientId")]
        public string ClientId { get; set; }        


    }
}
