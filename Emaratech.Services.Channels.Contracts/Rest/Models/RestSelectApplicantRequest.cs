using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestSelectApplicantRequest")]
    [DataContract]
    public class RestSelectApplicantRequest
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "permitNumber")]
        public string PermitNumber { get; set; }

        [DataMember(Name = "residenceNumber")]
        public string ResidenceNumber { get; set; }
    }
}