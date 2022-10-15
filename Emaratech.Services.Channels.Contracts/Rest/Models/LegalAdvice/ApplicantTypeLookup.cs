using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.LegalAdvice
{
    [DataContract]
    public class ApplicantTypeLookup
    {
        [DataMember(Name = "applicantTypeId")]
        public string ApplicantTypeId { get; set; }

        [DataMember(Name = "nameEn")]
        public string NameEn { get; set; }

        [DataMember(Name = "nameAr")]
        public string NameAr { get; set; }
    }
}