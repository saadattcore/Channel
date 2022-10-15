using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestLookup
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        
        [DataMember(Name = "valueEn")]
        public string ValueEn { get; set; }
        
        [DataMember(Name = "valueAr")]
        public string ValueAr { get; set; }
    }
}