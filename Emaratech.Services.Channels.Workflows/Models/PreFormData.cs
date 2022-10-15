using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Workflows.Models
{
    [DataContract]
    public class PreFormData
    {
        [DataMember(Name = "formId")]
        public string FormId { get; set; }
    }
}