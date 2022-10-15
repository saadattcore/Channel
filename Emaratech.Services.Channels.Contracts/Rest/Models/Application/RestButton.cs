using SwaggerWcf.Attributes;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestFormButton")]
    [DataContract]
    public class RestButton
    {
        [DataMember(Name = "resourceKey")]
        public string ResourceKey { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }
    }
}