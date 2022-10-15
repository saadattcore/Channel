using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupProfession : LookupBase
    {
        protected LookupProfession() : base(ConstantsUtil.LookupIds.Profession) { }
        
        protected static LookupProfession _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupProfession());
    }
}