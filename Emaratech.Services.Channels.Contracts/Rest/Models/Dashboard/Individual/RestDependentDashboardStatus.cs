using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual
{
    [SwaggerWcfDefinition(Name = "RestDependentDashboardStatus")]
    [DataContract]
    public class RestDependentDashboardStatus
    {
        [DataMember(Name = "isCancelAllowed")]
        public bool IsCancelAllowed { get; set; }

        [DataMember(Name = "isRenewAllowed")]
        public bool IsRenewAllowed { get; set; }

        [DataMember(Name = "isTransferPassportAllowed")]
        public bool IsTransferPassportAllowed { get; set; }

        [DataMember(Name = "isDocumentRequiredAllowed")]
        public bool IsDocumentRequiredAllowed { get; set; }

        [DataMember(Name = "isNewResidenceAllowed")]
        public bool IsNewResidenceAllowed { get; set; }

        public RestDependentDashboardStatus()
        {
            IsCancelAllowed = false;
            IsRenewAllowed = false;
            IsTransferPassportAllowed = false;
            IsDocumentRequiredAllowed = false;
            IsNewResidenceAllowed = false;
        }

        public void DisableAllActions()
        {
            IsCancelAllowed = false;
            IsRenewAllowed = false;
            IsTransferPassportAllowed = false;
            IsDocumentRequiredAllowed = false;
            IsNewResidenceAllowed = false;
        }


        //Items to remove for new implementation

        [DataMember(Name = "applicationNumber")]
        public string ApplicationNumber { get; set; }

        [DataMember(Name = "applicationStatus")]
        public string ApplicationStatus { get; set; }

        [DataMember(Name = "status")]
        public string Status
        {
            get;
            set;
        }

    }
}
