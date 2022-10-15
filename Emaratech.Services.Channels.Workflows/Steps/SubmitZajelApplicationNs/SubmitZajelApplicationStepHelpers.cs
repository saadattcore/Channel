using System;
using System.Linq;
using Emaratech.Services.Zajel.Model;

namespace Emaratech.Services.Channels.Workflows.Steps.SubmitZajelApplicationNs
{
    public static class SubmitZajelApplicationStepHelpers
    {
        public static ApplicationInfo.DeliveryModeEnum ToDeliveryModeEnum(this string argDeliverMode)
        {
            ApplicationInfo.DeliveryModeEnum deliveryMode;
            if ((Enum.GetNames(typeof(ApplicationInfo.DeliveryModeEnum)).Contains(argDeliverMode) &&
                 Enum.TryParse(argDeliverMode, out deliveryMode)))
            {
                return deliveryMode;
            }
            else
            {
                throw new ArgumentException($"Unable to parse {argDeliverMode} into {nameof(ApplicationInfo.DeliveryModeEnum)} enum.");
            }
        }

        public static ApplicationInfo.ProductTypeEnum ToProductTypeEnum(this string argProductTypeEnum)
        {
            ApplicationInfo.ProductTypeEnum productTypeEnum;
            if ((Enum.GetNames(typeof(ApplicationInfo.ProductTypeEnum)).Contains(argProductTypeEnum) &&
                 Enum.TryParse(argProductTypeEnum, out productTypeEnum)))
            {
                return productTypeEnum;
            }
            else
            {
                throw new ArgumentException($"Unable to parse {argProductTypeEnum} into {nameof(ApplicationInfo.ProductTypeEnum)} enum.");
            }
        }
    }
}