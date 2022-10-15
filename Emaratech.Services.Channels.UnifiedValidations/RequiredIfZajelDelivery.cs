using Emaratech.Services.Forms.Validation;
using System.Collections.Generic;
using Emaratech.Services.Forms.Validation.Models;

namespace Emaratech.Services.Channels.UnifiedValidations
{
    public class RequiredIfZajelDelivery : IValidation
    {
        public bool Validate(FullName name, string value, object data, IDictionary<FullName, string> formData)
        {
            var pickupField = new FullName { Entity = "ApplicationDetails", Name = "ResidencyPickup" };
            string pickupFieldValue;
            if (formData.TryGetValue(pickupField, out pickupFieldValue) && pickupFieldValue == "C7D97783C4F146F281D4AF91B5A61DED")
            {
                return !string.IsNullOrEmpty(value);
            }
            return true;
        }
    }
}