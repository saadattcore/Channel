using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emaratech.Services.Channels.Workflows.Models
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ViewEnum
    {
        [EnumMember(Value = "form")]
        Form = 1,
        [EnumMember(Value = "preform")]
        PreForm = 2,
        [EnumMember(Value = "document")]
        Documents = 3,
        [EnumMember(Value = "fee")]
        Fees = 4,
        [EnumMember(Value = "formAndPay")]
        FormAndPay = 5,
        [EnumMember(Value = "dependents")]
        Dependents = 6,
        [EnumMember(Value = "disclaimer")]
        Disclaimer = 7,
        [EnumMember(Value = "anyDocument")]
        AnyDocument = 8,
        [EnumMember(Value = "list")]
        List = 9,
        [EnumMember(Value = "endWorkflow")]
        EndWorkflow = 10
    }
}