using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionTypeCode
    {
        [EnumMember]
        Citizen = 4,
        [EnumMember]
        Resident = 2
    }
}