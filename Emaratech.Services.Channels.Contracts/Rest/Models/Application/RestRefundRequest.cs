using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestRefundRequest")]
    [DataContract]
    public class RestRefundRequest
    {
        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "IbanNumber")]
        public string IbanNumber { get; set; }

        [DataMember(Name = "bankName")]
        public string BankName { get; set; }

        [DataMember(Name = "beneficiaryName")]
        public string BeneficiaryName { get; set; }
    }
}
