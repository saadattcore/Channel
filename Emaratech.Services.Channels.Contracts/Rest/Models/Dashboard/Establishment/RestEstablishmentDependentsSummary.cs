using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment
{
    [SwaggerWcfDefinition(Name = "RestEstablishmentDependentsSummary")]
    [DataContract]
    public class RestEstablishmentDependentsSummary
    {
        [DataMember(Name = "userStatus")]
        public RestName UserStatus { get; set; }

        [DataMember(Name = "establishmentExpiryDate")]
        public DateTime? EstablishmentExpiryDate { get; set; }

        [DataMember(Name = "healthInsuranceExpiryDate")]
        public DateTime? HealthInsuranceExpiryDate { get; set; }

        [DataMember(Name = "tradeLicenseExpiryDate")]
        public DateTime? TradeLicenseExpiryDate { get; set; }

        [DataMember(Name = "entryPermitCount")]
        public int? EntryPermitCount { get; set; }

        [DataMember(Name = "residenceCount")]
        public int? ResidenceCount { get; set; }

        [DataMember(Name = "expiringSoonCount")]
        public int? ExpiringSoonCount { get; set; }

        [DataMember(Name = "overstayEntryPermitCount")]
        public int? OverstayEntryPermitCount { get; set; }

        [DataMember(Name = "overstayResidenceCount")]
        public int? OverstayResidenceCount { get; set; }
    }
}
