using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestPaymentResponse")]
    [DataContract]
    public class RestPaymentResponse
    {
        [DataMember(Name = "paymentCompleteUrl")]
        public string PaymentCompleteUrl { get; set; }
    }
}