using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
  public  class RSAAuthenticationData
    {
        [DataMember(Name = "oldToken")]
        public string OldToken { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }
        [DataMember(Name = "pin")]
        public string Pin { get; set; }
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

        [DataMember(Name = "actionType")]
        public string ActionType { get; set; }
    }
}
