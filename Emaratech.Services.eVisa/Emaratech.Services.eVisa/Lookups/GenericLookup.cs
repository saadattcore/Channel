using Emaratech.Services.Lookups.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Lookups
{
    internal class GenericLookup : ILookup
    {
        protected IEnumerable<RestLookupDetail> LookupDetails { get; set; }

        protected GenericLookup()
        {
        }

        public string GetEn(string argId)
        {
            var pick = LookupDetails.FirstOrDefault(x => string.Equals(argId, (string)x.ItemId));
            return pick?.ValueEn;
        }
        public string GetAr(string argId)
        {
            var pick = LookupDetails.FirstOrDefault(x => string.Equals(argId, (string)x.ItemId));
            return pick?.ValueAr;
        }
    }
}
