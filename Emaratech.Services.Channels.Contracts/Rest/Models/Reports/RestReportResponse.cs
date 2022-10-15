using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Reports
{
    [DataContract]
    public class RestReportResponse
    {
        [DataMember(Name = "reportContentAsBase64String")]
        public string ReportContentAsBase64String { get; set; }

        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

        [DataMember(Name = "fileType")]
        public string FileType { get; set; }

        [DataMember(Name = "validApplications")]
        public List<string> ValidApplications { get; set; }

        [DataMember(Name = "NotValidApplications")]
        public List<string> NotValidApplications { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
