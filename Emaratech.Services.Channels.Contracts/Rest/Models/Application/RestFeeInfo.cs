using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestFeeInfo")]
    [DataContract]
    public class RestFeeInfo
    {
        [DataMember(Name = "feeTypeId")]
        public string FeeTypeId { get; set; }
        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }
        [DataMember(Name = "resourceKey")]
        public string ResourceKey { get; set; }
    }
}