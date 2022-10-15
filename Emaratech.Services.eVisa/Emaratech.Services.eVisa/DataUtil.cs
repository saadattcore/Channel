using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Application.Model;
using Emaratech.Services.eVisa.Lookups;
using System.Configuration;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    public static class DataUtil
    {
        public static IDictionary<string, IDictionary<string, string>> ModelToDictionary(IList<RestApplicationSearchKeyValues> values)
        {
            var dictionary = new Dictionary<string, IDictionary<string, string>>();
            foreach (var value in values)
            {
                IDictionary<string, string> entity;
                if (!dictionary.TryGetValue(value.Entity, out entity))
                {
                    entity = new Dictionary<string, string>();
                    dictionary[value.Entity] = entity;
                }
                entity[value.PropertyName] = value.Value;

            }

            var relationShipId = values.Where(x => x.Entity == "SponsorDetails")
                 .FirstOrDefault(x => x.PropertyName == "SPONSORRELATIONID");

            if (!string.IsNullOrEmpty(relationShipId.Value))
            {
                var relation = LookupRelationShip.Instance.GetEn(relationShipId.Value);
                dictionary["SponsorDetails"]["SPONSORRELATION"] = relation;
            }

            var sponsorName = values.Where(x => x.Entity == "SponsorDetails")
                 .FirstOrDefault(x => x.PropertyName == "SPONSORFULLNAMEE");

            if(string.IsNullOrEmpty(sponsorName?.Value))
            {
                dictionary["SponsorDetails"]["SPONSORFULLNAMEE"] = "Applicant";
            }


            var host = ConfigurationSystem.AppSettings["WebHost"];
            
            var hostEntry = new Dictionary<string, string>();
            hostEntry.Add("Host", host);

            dictionary.Add("Parameters", hostEntry);
            return dictionary;
        }
    }
}
