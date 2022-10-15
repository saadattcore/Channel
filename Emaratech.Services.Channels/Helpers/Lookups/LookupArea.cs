using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupArea : LookupBase
    {
        protected LookupArea() : base(ConstantsUtil.LookupIds.Area) { }

        
        protected static LookupArea _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupArea());
    }
}
