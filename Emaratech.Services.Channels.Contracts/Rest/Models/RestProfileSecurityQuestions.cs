using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestProfileSecurityQuestions
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }
        [DataMember(Name = "questions")]
        public IEnumerable<RestProfileSecurityQuestion> Questions { get; set; }
    }
}