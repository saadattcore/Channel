using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestApproveDisclaimerRequest")]
    [DataContract]
    public class RestApproveDisclaimerRequest
    {
        [DataMember(Name = "disclaimerApproved")]
        public bool DisclaimerApproved { get; set; }

        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }
    }
}