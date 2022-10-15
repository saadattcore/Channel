using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupEmirate : LookupBase
    {
        protected LookupEmirate() : base(ConstantsUtil.LookupIds.Emirate) { }

        
        protected static LookupEmirate _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupEmirate());
    }
}
