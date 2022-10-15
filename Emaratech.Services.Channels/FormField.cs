using Emaratech.Services.Forms.Model;

namespace Emaratech.Services.Channels
{
    public class FormField
    {
        public string Name { get; set; }
        public FieldCustomBehavior CustomBehavior { get; set; }
        public RestRenderItem OriginalField { get; set; }
    }

    public enum FieldCustomBehavior
    {
        None = 0,
        Phone = 1,
        MobilePhone = 2,
        Lookup = 3,
        Date = 4,
        VisaNumber = 5
    }
}
