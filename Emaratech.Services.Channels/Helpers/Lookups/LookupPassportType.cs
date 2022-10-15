using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupPassportType : LookupBase
    {
        protected LookupPassportType() : base(ConstantsUtil.LookupIds.PassportType) { }

        
        protected static LookupPassportType _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupPassportType());
    }
}
