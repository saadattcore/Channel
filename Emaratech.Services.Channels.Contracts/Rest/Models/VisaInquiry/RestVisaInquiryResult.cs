using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.VisaInquiry
{
    [DataContract]
    public class RestVisaInquiryResult
    {
        [DataMember]
        public RestName fullName { get; set; }
        [DataMember]
        public string visaType { get; set; }
        [DataMember]
        public string visaStatus { get; set; }
        [DataMember]
        public DateTime? entryDate { get; set; }
        [DataMember]
        public DateTime? expiryDate { get; set; }
        [DataMember]
        public DateTime? validityDateBeforeEntry { get; set; }
        [DataMember]
        public DateTime? validityDateAfterEntry { get; set; }
    }
}
