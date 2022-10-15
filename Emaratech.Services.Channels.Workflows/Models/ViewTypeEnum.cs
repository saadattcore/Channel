using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emaratech.Services.Channels.Workflows.Models
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ViewTypeEnum
    {
        [EnumMember(Value = "mol")]
        MoL = 1,
        [EnumMember(Value = "entrypermit")]
        EntryPermit = 2,
        [EnumMember(Value = "cancel")]
        Cancel = 3
    }
}