using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    public class RestProfileSecurityAnswer
    {
        [DataMember(Name = "code")]
        public QuestionCode Code { get; set; }
        [DataMember(Name = "answer")]
        public string Answer { get; set; }
    }
}