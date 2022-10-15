using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Models
{
    [DataContract]
    public class FeeConfiguration
    {
        [DataMember(Name = "amount")]
        public string Amount { get; set; }

        [DataMember(Name = "feeTypeId")]
        public string FeeTypeId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "resourceKey")]
        public string ResourceKey { get; set; }

        [DataMember(Name = "beneficiaries")]
        public IList<BeneficiaryConfiguration> Beneficiaries { get; set; }
    }
}