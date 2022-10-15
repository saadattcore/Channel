using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.LegalAdvice
{
    [DataContract]
    public class NationalityLookup
    {
        [DataMember(Name = "nationalityId")]
        public string NationalityId { get; set; }

        [DataMember(Name = "nameEn")]
        public string NameEn { get; set; }

        [DataMember(Name = "nameAr")]
        public string NameAr { get; set; }
    }
}