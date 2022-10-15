using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.LegalAdvice
{
    [DataContract]
    public class AdviceTypeLookup
    {
        [DataMember(Name = "adviceTypeId")]
        public string AdviceTypeId { get; set; }

        [DataMember(Name = "nameEn")]
        public string NameEn { get; set; }

        [DataMember(Name = "nameAr")]
        public string NameAr { get; set; }
    }
}