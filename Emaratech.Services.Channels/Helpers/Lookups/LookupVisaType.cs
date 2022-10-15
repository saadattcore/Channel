using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupVisaType : LookupBase
    {
        protected LookupVisaType() : base(ConstantsUtil.LookupIds.VisaType) { }
        
        protected static LookupVisaType _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupVisaType());
    }
}