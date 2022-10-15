using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerWcf.Attributes;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment
{
    [SwaggerWcfDefinition(Name = "RestEstablishmentVisaInfo")]
    [DataContract]
    public class RestEstablishmentVisaInfo
    {
        [DataMember(Name = "visaType")]
        public string VisaType { get; set; }

        [DataMember(Name = "visaTypeId")]
        public string VisaTypeId { get; set; }

        //public VisaType VisaTypeEnum { get; set; }

        [DataMember(Name = "visaNumber")]
        public string VisaNumber { get; set; }

        [DataMember(Name = "expityDate")]
        public DateTime? ExpiryDate { get; set; }

        //[DataMember(Name = "createdDate")]
        //public DateTime? CreatedDate { get; set; }

        //[DataMember(Name = "firstNameEn")]
        //public string FirstNameEn { get; set; }

        [DataMember(Name = "firstName")]
        public RestName FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public RestName LastName { get; set; }

        [DataMember(Name = "nationality")]
        public string Nationality { get; set; }

        [DataMember(Name = "actionsList")]
        public IList<RestDependentActionsList> ActionsList { get; set; }

        //[DataMember(Name = "dependentsDashboardStatus")]
        //public RestDependentDashboardStatus DependentsDashboardStatus { get; set; }

        //[DataMember(Name = "applicationActions")]
        //public RestApplicationActions ApplicationActions { get; set; }

        //public VisaStatus StatusEnum { get; set; }

        [DataMember(Name = "status")]
        public string Status
        {
            get;
            set;
        }

        [DataMember(Name = "relationship")]
        public string Relationship { get; set; }
    }
}




//}