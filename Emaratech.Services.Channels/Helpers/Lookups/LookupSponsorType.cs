using Emaratech.Services.Channels.Helpers.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    class LookupSponsorType : LookupBase
    {
        protected LookupSponsorType() : base(ConstantsUtil.LookupIds.SponsorType) { }
        [ThreadStatic]
        protected static LookupSponsorType _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupSponsorType());
    }
}
