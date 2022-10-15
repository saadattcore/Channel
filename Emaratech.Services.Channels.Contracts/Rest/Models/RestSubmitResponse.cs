using System.Collections.Generic;
using System.Runtime.Serialization;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{


    [SwaggerWcfDefinition(Name = "RestSubmitApplicationResponse")]
    [DataContract]
    public class RestSubmitResponse
    {
        [DataMember(Name = "workflowToken")]
        public string WorkflowToken { get; set; }

        [DataMember(Name = "paymentUrl")]
        public string PaymentUrl { get; set; }
        
        [DataMember(Name = "documents")]
        public IEnumerable<RestDocumentInfo> Documents { get; set; }

        [DataMember(Name = "fees")]
        public IEnumerable<RestFeeInfo> Fees { get; set; }

        [DataMember(Name = "view")]
        public string View { get; set; }

        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "notificationMessage")]
        public string NotificationMessage { get; set; }

        [DataMember(Name = "formId")]
        public string FormId { get; set; }

        [DataMember(Name = "formConfiguration")]
        public string FormConfiguration { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }
        
        [DataMember(Name = "listData")]
        public IEnumerable<RestListData> ListData { get; set; }

        [DataMember(Name = "additionalButtons")]
        public IEnumerable<RestButton> AdditionalButtons { get; set; }

        [DataMember(Name = "rsaDeviceId")]
        public string RSADeviceId { get; set; }
    }
}