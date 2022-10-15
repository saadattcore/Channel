using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Entities
{
    [DataContract]
    public class ServiceInfo
    {
        [DataMember]
        public string CategoryId { get; set; }
        [DataMember]
        public string CategoryEn { get; set; }
        [DataMember]
        public string CategoryAr { get; set; }

        [DataMember]
        public string SubCategoryId { get; set; }
        [DataMember]
        public string SubCategoryEn { get; set; }
        [DataMember]
        public string SubCategoryAr { get; set; }

        [DataMember]
        public string NameEn { get; set; }
        [DataMember]
        public string NameAr { get; set; }
        [DataMember]
        public string Id { get; set; }
    }
}
