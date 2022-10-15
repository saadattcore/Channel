using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Payment.Model;

namespace Emaratech.Services.Channels.BusinessLogic.Refund
{
    public interface IRefund
    {
        Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplications();

        Task<RestRefundableApplication> FetchRefundableApplicationDetail(string applicationId);

        Task<RestRefundInfo> ProcessApplicationRefund(RestRefundRequest refundRequest);
    }
}