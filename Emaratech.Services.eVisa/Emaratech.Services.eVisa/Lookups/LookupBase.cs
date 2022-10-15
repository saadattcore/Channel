namespace Emaratech.Services.eVisa.Lookups
{
    internal class LookupBase : GenericLookup
    {
        protected LookupBase(string argLookupId) : base()
        {
            LookupDetails = ServicesHelper.GetLookupItems(argLookupId).Result;
        }
    }
}
