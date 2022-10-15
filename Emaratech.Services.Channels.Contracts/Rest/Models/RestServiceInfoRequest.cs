using System.Runtime.Serialization;
using SwaggerWcf.Attributes;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestServiceInfoRequest")]
    [DataContract]
    public class RestServiceInfoRequest
    {
        [DataMember(Name = "legalAdviceNumber")]
        public string LegalAdviceNumber { get; set; }

        [DataMember(Name = "permitNumber")]
        public string PermitNumber { get; set; }

        [DataMember(Name = "residenceNumber")]
        public string ResidenceNumber { get; set; }

        [DataMember(Name = "platformId")]
        public string PlatformId { get; set; }

        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "searchInfo")]
        public RestSearchInfo SearchInfo { get; set; }
    }
}