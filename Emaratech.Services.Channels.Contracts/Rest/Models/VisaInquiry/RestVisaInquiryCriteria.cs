using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.VisaInquiry
{
    [DataContract]
    public class RestVisaInquiryCriteria
    {
        [DataMember]
        public string serviceType { get; set; }
        [DataMember]
        public string visaNumber { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public DateTime? dateOfBirth { get; set; }
        [DataMember]
        public string nationalityId { get; set; }
    }
}
