using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestProfileImage")]
    [DataContract]
    public class RestProfileImage
    {
        [DataMember(Name="profileImage")]
        public string ProfileImage { get; set; }
    }
}