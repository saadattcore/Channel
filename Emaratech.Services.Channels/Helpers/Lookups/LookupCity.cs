using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupCity : LookupBase
    {
        protected LookupCity() : base(ConstantsUtil.LookupIds.City) { }

        
        protected static LookupCity _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupCity());
    }
}
