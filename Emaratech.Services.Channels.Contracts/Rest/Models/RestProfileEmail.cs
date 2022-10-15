using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestProfileEmail")]
    [DataContract]
    public class RestProfileEmail
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}