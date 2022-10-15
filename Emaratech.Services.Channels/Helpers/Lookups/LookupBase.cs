namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupBase : GenericLookup
    {
        protected LookupBase(string argLookupId) : base()
        {
            LookupDetails = ServicesHelper.GetLookupItems(argLookupId).Result;
        }
    }
}