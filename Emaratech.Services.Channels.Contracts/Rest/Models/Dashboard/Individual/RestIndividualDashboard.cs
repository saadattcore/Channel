using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual
{
    [SwaggerWcfDefinition(Name = "IndividualDashboard")]
    [DataContract]
    public class RestIndividualDashboard
    {
        [DataMember(Name = "dependentsVisaInformation")]
        public List<RestDependentVisaInfo> DependentsVisaInfo = new List<RestDependentVisaInfo>();

        [DataMember(Name = "dependentsApplicationInformation")]
        public List<RestDependentApplicationInfo> DependentsApplicationInfo = new List<RestDependentApplicationInfo>();
    }
}
