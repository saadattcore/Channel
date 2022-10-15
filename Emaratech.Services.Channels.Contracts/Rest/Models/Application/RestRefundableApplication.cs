using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    [SwaggerWcfDefinition(Name = "RestRefundableApplication")]
    [DataContract]

    public class RestRefundableApplication
    {
        [DataMember(Name = "applicationId")]
        public string ApplicationId { get; set; }

        [DataMember(Name = "transactionBatchId")]
        public string TransactionBatchId { get; set; }

        [DataMember(Name = "fullName")]
        public RestName FullName { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "serviceId")]
        public string ServiceId { get; set; }

        [DataMember(Name = "visaType")]
        public string VisaType { get; set; }

        [DataMember(Name = "createdDate")]
        public DateTime? CreatedDate { get; set; }

        [DataMember(Name = "statusDate")]
        public DateTime? StatusDate { get; set; }

        [DataMember(Name = "totalAmount")]
        public int? TotalAmount { get; set; }

        [DataMember(Name = "refundableAmount")]
        public int? RefundableAmount { get; set; }

        [DataMember(Name = "refundType")]
        public string RefundType { get; set; }

        [DataMember(Name = "paymentType")]
        public string PaymentType { get; set; }

        public RestRefundableApplication()
        {
            TotalAmount = 0;
            RefundableAmount = 0;
            Actions = new List<RestApplicationActions>();
        }

        [DataMember(Name = "actions")]
        public IList<RestApplicationActions> Actions { get; set; }

        [DataMember(Name = "errorCode")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "refundRequestDate")]
        public DateTime? RefundRequestDate { get; set; }
    }
}
