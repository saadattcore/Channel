using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupResidenceStatus : LookupBase
    {
        protected LookupResidenceStatus() : base(ConstantsUtil.LookupIds.ResidenceStatus) { }
        
        protected static LookupResidenceStatus _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupResidenceStatus());
    }
}