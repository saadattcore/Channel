using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupEducation : LookupBase
    {
        protected LookupEducation() : base(ConstantsUtil.LookupIds.Education) { }

        
        protected static LookupEducation _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupEducation());
    }
}
