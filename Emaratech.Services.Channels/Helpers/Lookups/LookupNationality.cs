using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupNationality : LookupBase
    {
        protected LookupNationality() : base(ConstantsUtil.LookupIds.Nationality) { }

        
        protected static LookupNationality _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupNationality());
    }
}