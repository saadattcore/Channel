using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual
{
    [SwaggerWcfDefinition(Name = "RestDashboardSearchRequest")]
    [DataContract]
    public class RestDashboardSearchRequest
    {
        [DataMember(Name = "startDate")]
        public DateTime? StartDate { get; set; }

        [DataMember(Name = "endDate")]
        public DateTime? EndDate { get; set; }

        [DataMember(Name = "criteria")]
        public string Criteria { get; set; }

        [DataMember(Name = "serviceType")]
        public string ServiceType { get; set; }

        [DataMember(Name = "establishmentCode")]
        public string EstablishmentCode { get; set; }

        [DataMember(Name = "statusId")]
        public string StatusId { get; set; }

        [DataMember(Name = "receiptTransactionNo")]
        public string ReceiptTransactionNo { get; set; }

        [DataMember(Name = "passportNumber")]
        public string PassportNumber { get; set; }

        public string ApplicationStatuses { get; set; }
        public bool? IsBatchIdMarked { get; set; }

        public string UserId { get; set; }

        public string SponsorNo { get; set; }

        public string ResidenceNo { get; set; }

        public string ServiceId { get; set; }

        public string Platform { get; set; }

        public ApplicationModules ApplicationModule { get; set; }
    }
}
