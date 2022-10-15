using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Payment.Model;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Common.Caching;

namespace Emaratech.Services.Channels.Helpers
{
    public class PaymentHelper
    {
        public static  Task<Merchant> GetMerchant(string userType)
        {
            return Cache.Default.Run(nameof(GetMerchant),userType,async () =>
            {

                // Get the merchant id corresponding to the user type
                var userTypeLookup = await LookupHelper.GetLookupItems(Constants.Lookups.UserTypeLookupId);
                var merchantMapping = userTypeLookup.FirstOrDefault(x => x.ItemId == userType)?.Col4;
                if (string.IsNullOrEmpty(merchantMapping))
                {
                    throw ChannelErrorCodes.InvalidPaymentMerchantConfiguration.ToWebFault($"There is no merchant mapping configured for the user type '{userType}'");
                }

                var paymentConfiguration = await GetPaymentConfiguration();
                var merchant = paymentConfiguration?.Merchants?.FirstOrDefault(x => x.MappingName == merchantMapping);
                if (merchant == null)
                {
                    throw ChannelErrorCodes.InvalidPaymentMerchantConfiguration.ToWebFault($"There is no merchant configured for the user type '{userType}' in config file");
                }
                return merchant;
            });
        }

        private static Task<PaymentConfiguration> GetPaymentConfiguration()
        {
            return ApiFactory.Default.GetPaymentApi().GetPaymentConfigurationAsync();
        }
    }
}