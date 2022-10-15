using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Lookups
{
    internal class LookupNationality : LookupBase
    {
        protected LookupNationality() : base(ConstantsUtil.LookupIds.Nationality) { }

        [ThreadStatic]
        protected static LookupNationality _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupNationality());
    }
}
