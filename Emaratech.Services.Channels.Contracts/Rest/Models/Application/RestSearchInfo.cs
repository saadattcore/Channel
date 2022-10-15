using System.Runtime.Serialization;
using SwaggerWcf.Attributes;
using System;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestSearchInfo")]
    [DataContract]
    public class RestSearchInfo
    {
        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [DataMember(Name = "passportNo")]
        public string PassportNo { get; set; }

        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }

        [DataMember(Name = "unifiedNumber")]
        public string UnifiedNumber { get; set; }
    }
}