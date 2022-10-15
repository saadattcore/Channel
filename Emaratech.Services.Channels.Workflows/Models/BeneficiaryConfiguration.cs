using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Workflows.Models
{
    [DataContract]
    public class BeneficiaryConfiguration
    {
        [DataMember(Name = "amount")]
        public string Amount { get; set; }
        [DataMember(Name = "account")]
        public string Account { get; set; }
        [DataMember(Name = "desc")]
        public string Desc { get; set; }
        [DataMember(Name = "code")]
        public string Code { get; set; }

    }
}