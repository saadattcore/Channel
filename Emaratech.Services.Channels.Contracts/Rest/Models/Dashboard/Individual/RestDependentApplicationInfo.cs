using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual
{
    [SwaggerWcfDefinition(Name = "RestDependentApplicationInfo")]
    [DataContract]
    public class RestDependentApplicationInfo
    {
        [DataMember(Name = "actionsList")]
        public IList<RestDependentActionsList> ActionsList { get; set; }

        [DataMember(Name = "applicationNumber")]
        public string ApplicationNumber { get; set; }

        [DataMember(Name = "visaNumber")]
        public string VisaNumber { get; set; }

        [DataMember(Name = "visaNumberType")]
        public string VisaNumberType { get; set; }

        [DataMember(Name = "createdDate")]
        public DateTime? CreatedDate { get; set; }

        [DataMember(Name = "applicationStatus")]
        public string ApplicationStatus { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "firstName")]
        public RestName FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public RestName LastName { get; set; }

        [DataMember(Name = "visaType")]
        public string VisaType { get; set; }

        [DataMember(Name = "visaTypeId")]
        public string VisaTypeId { get; set; }

        [DataMember(Name = "nationality")]
        public string Nationality { get; set; }

        [DataMember(Name = "relationship")]
        public string Relationship { get; set; }
    }
}
