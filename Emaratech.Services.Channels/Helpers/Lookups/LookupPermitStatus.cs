using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupPermitStatus : LookupBase
    {
        protected LookupPermitStatus() : base(ConstantsUtil.LookupIds.PermitStatus) { }
        
        protected static LookupPermitStatus _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupPermitStatus());
    }
}