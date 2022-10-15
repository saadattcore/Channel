using System;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupTravelType : LookupBase
    {

        protected LookupTravelType() : base(ConstantsUtil.LookupIds.TravelType) { }
        
        protected static LookupTravelType _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupTravelType());
    }
}
