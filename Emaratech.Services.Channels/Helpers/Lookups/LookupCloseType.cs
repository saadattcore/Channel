using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupCloseType : LookupBase
    {

        protected LookupCloseType() : base(ConstantsUtil.LookupIds.CloseType) { }
        
        protected static LookupCloseType _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupCloseType());
    }
}
