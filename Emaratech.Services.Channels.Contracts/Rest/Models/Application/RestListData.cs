using SwaggerWcf.Attributes;
using System;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestListData")]
    [DataContract]
    public class RestListData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "isEdit")]
        public bool IsEdit { get; set; }

        [DataMember(Name = "name")]
        public RestName Name { get; set; }

        [DataMember(Name = "unifiedNo")]
        public string UnifiedNo { get; set; }

        [DataMember(Name = "birthDate")]
        public DateTime BirthDate { get; set; }

        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }
    }
}