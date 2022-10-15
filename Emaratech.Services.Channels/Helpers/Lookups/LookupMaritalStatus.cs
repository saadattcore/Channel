using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class LookupMaritalStatus : LookupBase
    {
        protected LookupMaritalStatus() : base(ConstantsUtil.LookupIds.MaritalStatus) { }

        
        protected static LookupMaritalStatus _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupMaritalStatus());
    }
}
