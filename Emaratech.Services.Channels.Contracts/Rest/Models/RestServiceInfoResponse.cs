using System.Collections.Generic;
using System.Runtime.Serialization;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using SwaggerWcf.Attributes;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestServiceInfoResponse")]
    [DataContract]
    public class RestServiceInfoResponse
    {
        [DataMember(Name = "documents")]
        public IEnumerable<RestDocumentInfo> Documents { get; set; }

        [DataMember(Name = "fees")]
        public IEnumerable<RestFeeInfo> Fees { get; set; }

        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "view")]
        public string View { get; set; }

        [DataMember(Name = "formId")]
        public string FormId { get; set; }

        [DataMember(Name = "formConfiguration")]
        public string FormConfiguration { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }

        [DataMember(Name = "dependents")]
        public IEnumerable<RestDependentVisaInfo> Dependents { get; set; }

        [DataMember(Name = "additionalButtons")]
        public IEnumerable<RestButton> AdditionalButtons { get; set; }
    }
}