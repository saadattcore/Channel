using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupGender : LookupBase
    {
        protected LookupGender() : base(ConstantsUtil.LookupIds.Gender) { }

        
        protected static LookupGender _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupGender());
    }
}