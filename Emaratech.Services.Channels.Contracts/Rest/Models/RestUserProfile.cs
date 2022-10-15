using System;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestUserProfile")]
    [DataContract]
    public class RestUserProfile
    {
        [DataMember(Name = "firstNameAr")]
        public string FirstNameAr { get; set; }
        [DataMember(Name = "firstNameEn")]
        public string FirstNameEn { get; set; }
        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "email")]
        public string Email { get; set; }
        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [DataMember(Name = "mobile")]
        public string Mobile { get; set; }
        [DataMember(Name = "passportNumber")]
        public string PassportNumber { get; set; }
        [DataMember(Name = "nationalityId")]
        public string NationalityId { get; set; }
        [DataMember(Name = "genderId")]
        public string GenderId { get; set; }
        [DataMember(Name = "fileTypeId")]
        public string FileTypeId { get; set; }
        [DataMember(Name = "visaNumber")]
        public string VisaNumber { get; set; }
        [DataMember(Name = "sponsorNo")]
        public string SponsorNo { get; set; }
        [DataMember(Name = "userType")]
        public string UserType { get; set; }

        [DataMember(Name = "imageId")]
        public string Image { get; set; }

        [DataMember(Name = "visaExpiryDate")]
        public DateTime? VisaExpiryDate { get; set; }

        [DataMember(Name = "passportExpiryDate")]
        public DateTime? PassportExpiryDate { get; set; }

        [DataMember(Name = "name")]
        public name name { get; set; }
    }

    public class name
    {
        [DataMember(Name = "ar")]
        public string ar { get; set; }

        [DataMember(Name = "en")]
        public string en { get; set; }
    }
}