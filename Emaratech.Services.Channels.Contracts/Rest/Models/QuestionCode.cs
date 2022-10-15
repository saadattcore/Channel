using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionCode
    {
        [EnumMember]
        [QuestionTypeAttr(type: QuestionType.Date)]
        LastCountryEntryDate,
        [EnumMember]
        [QuestionTypeAttr(type: QuestionType.Number)]
        NumberOfSponsoredResidents,
        [EnumMember]
        [QuestionTypeAttr(type: QuestionType.Number)]
        NumberOfSponsoredVisas,
        [EnumMember]
        [QuestionTypeAttr(type: QuestionType.Text)]
        PassportNumber,
        [EnumMember]
        [QuestionTypeAttr(type: QuestionType.Number)]
        UnifiedNumber,
    }
    public class QuestionTypeAttr : Attribute
    {
        internal QuestionTypeAttr(QuestionType type)
        {
            this.Type = type;
        }
        internal QuestionTypeAttr(QuestionType type, string lookupId)
        {
            this.Type = type;
            this.LookupId = lookupId;
        }
        public QuestionType Type { get; private set; }
        public string LookupId { get; private set; }
    }


}