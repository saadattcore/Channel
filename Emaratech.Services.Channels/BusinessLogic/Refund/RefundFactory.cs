using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.BusinessLogic.Refund
{
    public class RefundFactory
    {
        public static IRefund GetRefundType(string userId, string userType, string systemId, string refundType)
        {
            if (Convert.ToInt32(refundType) == (int) RefundType.ApplicationRefund)
                return new ApplicationRefund(userId, userType, systemId);

            else if (Convert.ToInt32(refundType) == (int) RefundType.WarrantyRefund)
                return new WarrantyRefund(userId, userType, systemId);

            else
                throw ChannelErrorCodes.BadRequest.ToWebFault("Invalid refund type");

        }

    }
}