using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Lookups
{
    internal class LookupPassportType : LookupBase
    {
        protected LookupPassportType() : base(ConstantsUtil.LookupIds.PassportType) { }

        [ThreadStatic]
        protected static LookupPassportType _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupPassportType());
    }
}
