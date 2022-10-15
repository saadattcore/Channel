using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestProfile
    {
        [DataMember(Name = "mobileNumbers")]
        public IList<string> MobileNumbers { get; set; }
    }
}