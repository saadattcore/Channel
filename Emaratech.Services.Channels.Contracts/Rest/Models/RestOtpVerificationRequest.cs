using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestOtpVerificationRequest")]
    [DataContract]
    public class RestOtpVerificationRequest
    {
        [DataMember(Name = "otpCode")]
        public string otpCode { get; set; }

        [DataMember(Name = "otpToken")]
        public string OtpToken { get; set; }
    }
}
