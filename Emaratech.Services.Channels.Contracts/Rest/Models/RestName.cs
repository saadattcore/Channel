using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestName")]
    [DataContract]
    public class RestName
    {
        [DataMember(Name="en")]
        public string En { get; set; }
        [DataMember(Name = "ar")]
        public string Ar { get; set; }
    }
}