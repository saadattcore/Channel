using System;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestUserDetailedProfile")]
    [DataContract]
    public class RestUserDetailedProfile : RestUserProfile
    {
        [DataMember(Name = "visaIssueDate")]
        public DateTime? VisaIssueDate { get; set; }

        [DataMember(Name = "visaStatus")]
        public string VisaStatus { get; set; }

        [DataMember(Name = "nationalityEn")]
        public string NationalityEn { get; set; }

        [DataMember(Name = "nationalityAr")]
        public string NationalityAr { get; set; }

        [DataMember(Name = "udbNumber")]
        public string UDBNumber { get; set; }

        [DataMember(Name = "passportType")]
        public string PassportType { get; set; }

        [DataMember(Name = "genderEn")]
        public string GenderEn { get; set; }

        [DataMember(Name = "genderAr")]
        public string GenderAr { get; set; }

        [DataMember(Name = "birthPlaceEn")]
        public string BirthPlaceEn { get; set; }

        [DataMember(Name = "birthPlaceAr")]
        public string BirthPlaceAr { get; set; }

        [DataMember(Name = "passportIssueDate")]
        public DateTime? PassportIssueDate { get; set; }

        [DataMember(Name = "passportIssueAuthorityEn")]
        public string PassportIssueAuthorityEn { get; set; }

        [DataMember(Name = "passportIssueAuthorityAr")]
        public string PassportIssueAuthorityAr { get; set; }

        [DataMember(Name = "residenceAccompanied")]
        public string ResidenceAccompanied { get; set; }

        [DataMember(Name = "professionEn")]
        public string ProfessionEn { get; set; }

        [DataMember(Name = "professionAr")]
        public string ProfessionAr { get; set; }

        [DataMember(Name = "placeOfIssueEn")]
        public string PlaceOfIssueEn { get; set; }

        [DataMember(Name = "placeOfIssueAr")]
        public string PlaceOfIssueAr { get; set; }

        [DataMember(Name = "sponsorNameEn")]
        public string SponsorNameEn { get; set; }

        [DataMember(Name = "sponsorNameAr")]
        public string SponsorNameAr { get; set; }
    }

}