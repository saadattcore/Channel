using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestProfileSecurityAnswers
    {
        [DataMember(Name = "questionToken")]
        public string QuestionToken { get; set; }
        [DataMember(Name = "answers")]
        public IList<RestProfileSecurityAnswer> Answers { get; set; }

    }
}