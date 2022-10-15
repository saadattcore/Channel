using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Lookups
{
    internal class LookupProfession : LookupBase
    {
        protected LookupProfession() : base(ConstantsUtil.LookupIds.Profession) { }

        [ThreadStatic]
        protected static LookupProfession _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupProfession());
    }
}
