using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [DataContract]
    public class RestApplicationActions
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "serviceId")]
        public string ServiceId { get; set; }

        [DataMember(Name = "userType")]
        public string UserType { get; set; }

        [DataMember(Name = "statusId")]
        public string StatusId { get; set; }
    }
}
