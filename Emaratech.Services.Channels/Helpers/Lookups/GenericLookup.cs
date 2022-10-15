using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Emaratech.Services.Lookups.Model;

namespace Emaratech.Services.Channels.Helpers.Lookups
{
    public class GenericLookup : ILookup
    {
        protected IEnumerable<RestLookupDetail> LookupDetails { get; set; }

        protected GenericLookup()
        {
        }

        public string GetEn(string argId)
        {
            if (string.IsNullOrEmpty(argId))
                return argId;

            var pick = LookupDetails.FirstOrDefault(x => string.Equals(argId, (string)x.ItemId));
            return pick?.ValueEn;
        }
        public string GetAr(string argId)
        {
            if (string.IsNullOrEmpty(argId))
                return argId;

            var pick = LookupDetails.FirstOrDefault(x => string.Equals(argId, (string)x.ItemId));
            return pick?.ValueAr;
        }
    }

}