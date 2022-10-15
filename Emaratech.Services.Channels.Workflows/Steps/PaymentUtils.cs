using System;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    static internal class PaymentUtils
    {
        public static async Task<string> GetMerchantId(string userType)
        {
            var userTypeLookup = await ServicesHelper.GetLookupItems(Constants.Lookups.UserTypeLookupId);
            var merchantId = userTypeLookup.FirstOrDefault(x => x.ItemId == userType)?.Col4;
            if (String.IsNullOrEmpty(merchantId))
            {
                throw ChannelWorkflowErrorCodes.InvalidPaymentMerchantConfiguration.ToWebFault($"There is no merchant configured for the user type '{userType}'");
            }
            return merchantId;
        }
    }
}