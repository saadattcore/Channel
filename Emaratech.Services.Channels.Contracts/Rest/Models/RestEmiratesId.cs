using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name= "RestEmiratesId")]
    [DataContract]
    public class RestEmiratesId
    {

        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }


    }
}