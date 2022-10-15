using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupReligion : LookupBase
    {
        protected LookupReligion() : base(ConstantsUtil.LookupIds.Religion) { }

        
        protected static LookupReligion _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupReligion());
    }
}
