using System;


namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupResidenceType : LookupBase
    {
        protected LookupResidenceType() : base(ConstantsUtil.LookupIds.ResidenceType) { }
        
        protected static LookupResidenceType _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupResidenceType());
    }
}
