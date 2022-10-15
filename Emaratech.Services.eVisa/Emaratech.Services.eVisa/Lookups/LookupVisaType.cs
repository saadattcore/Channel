using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Lookups
{
    internal class LookupVisaType : LookupBase
    {
        protected LookupVisaType() : base(ConstantsUtil.LookupIds.VisaType) { }
        [ThreadStatic]
        protected static LookupVisaType _instance;
        public static GenericLookup Instance => _instance ?? (_instance = new LookupVisaType());
    }
}
