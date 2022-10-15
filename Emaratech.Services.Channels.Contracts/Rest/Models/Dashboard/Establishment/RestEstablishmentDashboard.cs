using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment
{
    [SwaggerWcfDefinition(Name = "EstablishmentDashboard")]
    [DataContract]
    public class RestEstablishmentDashboard
    {
        [DataMember(Name = "dependentsVisaInformation")]
        public List<RestEstablishmentVisaInfo> DependentsVisaInfo = new List<RestEstablishmentVisaInfo>();

        [DataMember(Name = "dependentsApplicationInformation")]
        public List<RestDependentApplicationInfo> DependentsApplicationInfo = new List<RestDependentApplicationInfo>();
    }
}
