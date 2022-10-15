using Emaratech.Services.Channels.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Emaratech.Services.Channels.Helpers
{
    public class ConfigurationRepository
    {
        public string GetUserTypeLocalLookupResult()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.UserTypeLocalLookupResult];
        }

        public static string GetApplicationRefundStatuses()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.ApplicationRefundStatuses];
        }

        public static string GetRefundListStatuses()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.RefundListStatuses];
        }

        public static string GetWarrantyRefundStatuses()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.WarrantyRefundStatuses];
        }

        public static string GetIsUsedEmiratesIdAllowed()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.IsUsedEmiratesIdAllowed];
        }

        public static string GetApplicationPrintStatuses()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.ApplicationPrintStatuses];
        }

        public static string GetEntryPermitApplicationType()
        {
            return System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigurationKeys.EntryPermitApplicationType];
        }

        public static List<int> GetEntryPermitPrintStatuses()
        {
            var statuses = System.Configuration
                                 .ConfigurationManager
                                 .AppSettings[Constants.ConfigurationKeys.EntryPermitPrintStatuses];

            return statuses.Split(',').Select(int.Parse).ToList();
        }

        public static List<int> GetEntryPermitCancellationStatuses()
        {
            var statuses = System.Configuration
                                 .ConfigurationManager
                                 .AppSettings[Constants.ConfigurationKeys.Print_PermitCancellation_Statuses];

            return statuses.Split(',').Select(int.Parse).ToList();
        }

        public static List<int> GetResidanceCancellationStatuses()
        {
            var statuses = System.Configuration
                                 .ConfigurationManager
                                 .AppSettings[Constants.ConfigurationKeys.Print_ResidenceCancellation_Statuses];

            return statuses.Split(',').Select(int.Parse).ToList();
        }
    }
}