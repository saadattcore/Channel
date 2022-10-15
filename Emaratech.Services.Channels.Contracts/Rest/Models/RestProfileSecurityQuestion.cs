using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestProfileSecurityQuestion
    {
        [DataMember(Name = "code")]
        public QuestionCode Code { get; set; }
        [DataMember(Name = "question")]
        public string Question { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "lookupId")]
        public string LookupId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    [DataContract]
    public enum QuestionType
    {
        [EnumMember]
        Date,
        [EnumMember]
        Text,
        [EnumMember]
        Toggle,
        [EnumMember]
        Number,
        [EnumMember]
        Lookup
    }
}