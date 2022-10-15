using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestUser
    {
        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "otpToken")]
        public string OTPToken { get; set; }

        [DataMember(Name = "registerNewMobileNumberToken")]
        public string RegisterNewMobileNumberToken { get; set; }
    }
}