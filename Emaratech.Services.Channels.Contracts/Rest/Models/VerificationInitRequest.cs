using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "VerificationInitRequest")]
    [DataContract]
    public class VerificationInitRequest
    {
        [DataMember(Name = "mobileNumber")]
        public string MobileNumber { get; set; }
    }
}