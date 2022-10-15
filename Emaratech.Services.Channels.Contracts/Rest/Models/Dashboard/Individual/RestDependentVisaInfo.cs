using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual
{
    [SwaggerWcfDefinition(Name = "RestDependentVisaInfo")]
    [DataContract]
    public class RestDependentVisaInfo
    {
        [DataMember(Name = "visaType")]
        public string VisaType { get; set; }

        [DataMember(Name = "visaTypeId")]
        public string VisaTypeId { get; set; }
        
        [DataMember(Name = "visaNumber")]
        public string VisaNumber { get; set; }

        [DataMember(Name = "expityDate")]
        public DateTime? ExpiryDate { get; set; }

        [DataMember(Name = "validityDate")]
        public DateTime? ValidityDate { get; set; }

        [DataMember(Name = "firstName")]
        public RestName FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public RestName LastName { get; set; }

        [DataMember(Name = "nationality")]
        public string Nationality { get; set; }

        [DataMember(Name = "actionsList")]
        public IList<RestDependentActionsList> ActionsList { get; set; }
        
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "relationship")]
        public string Relationship { get; set; }
    }
}