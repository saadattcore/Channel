using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.HappinessMeter
{
    [SwaggerWcfDefinition(Name = "HappinessMeterRequest")]
    [DataContract]
    public class HappinessMeterRequest
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "themeColor")]
        public string ThemeColor { get; set; }

        [DataMember(Name = "lang")]
        public string Lang { get; set; }
    }
}
