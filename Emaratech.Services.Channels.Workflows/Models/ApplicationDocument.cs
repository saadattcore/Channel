using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Document.Model;

namespace Emaratech.Services.Channels.Workflows.Models
{
    [DataContract]
    public class ApplicationDocument : RestDocument
    {
        [DataMember(Name = "documentId")]
        public string DocumentId { get; set; }
    }
}
