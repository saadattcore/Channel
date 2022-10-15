using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestOtpVerificationPasswordChangeRequest")]
    [DataContract]
    public class RestOtpVerificationPasswordChangeRequest : RestOtpVerificationRequest
    {
        [DataMember(Name = "newPassword")]
        public string NewPassword { get; set; }
    }
}