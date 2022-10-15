using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Lookups
{
    class LookupRelationShip : LookupBase
    {
        protected LookupRelationShip() : base(ConstantsUtil.LookupIds.RelationShip) { }

        [ThreadStatic]
        protected static LookupRelationShip _instance;

        public static GenericLookup Instance => _instance ?? (_instance = new LookupRelationShip());
    }
}
