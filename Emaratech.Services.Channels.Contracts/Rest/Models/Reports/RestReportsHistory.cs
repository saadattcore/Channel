using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [SwaggerWcfDefinition(Name = "RestReportsHistory")]
    [DataContract]
    public class RestReportsHistory
    {
        [DataMember(Name = "serviceId")]
        public string ServiceId { get; set; }

        [DataMember(Name = "serviceResourceKey")]
        public string ServiceResourceKey { get; set; }

        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "paymentDate")]
        public string PaymentDate { get; set; }

        [DataMember(Name = "name")]
        public RestName Name { get; set; }
    }   
}
