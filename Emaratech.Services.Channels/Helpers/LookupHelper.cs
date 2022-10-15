using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Lookups.Model;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers
{
    public class LookupHelper
    {
        private static IEnumerable<RestLookupDetail> _nationalityLookuptDetails;
        private static IEnumerable<RestLookupDetail> _passportTypeLookupDetails;
        private static IEnumerable<RestLookupDetail> _professionLookupDetails;
        private static IEnumerable<RestLookupDetail> _visaTypeLookupDetails;

        public static IEnumerable<RestLookupDetail> NationalityLookuptDetails => _nationalityLookuptDetails ??
                (_nationalityLookuptDetails = GetLookupItems("9E863B86C5234DC1A8C2829DCC1F6EF7").Result);
        public static IEnumerable<RestLookupDetail> PassportTypeLookupDetails => _passportTypeLookupDetails ??
                (_passportTypeLookupDetails = GetLookupItems("6ED3A3EF939A474EA213F66ECAD40A65").Result);
        public static IEnumerable<RestLookupDetail> ProfessionLookupDetails => _professionLookupDetails ??
                (_professionLookupDetails = GetLookupItems("E82C489A44214C35B9B8B32300ECB6F6").Result);
        public static IEnumerable<RestLookupDetail> VisaTypeLookupDetails => _visaTypeLookupDetails ??
                (_visaTypeLookupDetails = GetLookupItems("F278CB4C7C81429496D75807E7E7DD11").Result);

        public static async Task<IEnumerable<RestLookupDetail>> GetLookupItems(string lookupId)
        {
            return (await ApiFactory.Default.GetLookupApi().GetLookupDetailsAsync(lookupId, "1", "1000")).Data;
        }

        public static string GetLookupCol1ById(string lookup, string ID)
        {
            return
                ApiFactory.Default.GetLookupApi()
                    .GetLookupDetail(lookup, ID).Col1;
        }

        public static string GetLookupCol2ById(string lookup, string ID)
        {
            return
                ApiFactory.Default.GetLookupApi()
                    .GetLookupDetail(lookup, ID).Col2;
        }

        public static string GetLookupCol4ById(string lookup, string id)
        {
            return
                ApiFactory.Default.GetLookupApi()
                    .GetLookupDetail(lookup, id).Col4;
        }

        public static string GetLookupItemIdByCol1(string lookup, string col1)
        {
            return
                ApiFactory.Default.GetLookupApi()
                    .GetLookupByCodeCol1Paging(lookup, col1, "1", "1")
                    .Data.Single()
                    .ItemId;
        }

        public static string GetLookupItemIdByCol2(string lookup, string col2)
        {
            return
                ApiFactory.Default.GetLookupApi()
                    .GetLookupByCodeCol2Paging(lookup, col2, "1", "1")
                    .Data.Single()
                    .ItemId;
        }

        public static string GetEn(IEnumerable<RestLookupDetail> lookupDetails, string argId)
        {
            if (string.IsNullOrEmpty(argId))
                return argId;

            var pick = lookupDetails.FirstOrDefault(x => string.Equals(argId, (string)x.ItemId));
            return pick?.ValueEn;
        }

        public static string GetAr(IEnumerable<RestLookupDetail> lookupDetails, string argId)
        {
            if (string.IsNullOrEmpty(argId))
                return argId;

            var pick = lookupDetails.FirstOrDefault(x => string.Equals(argId, (string)x.ItemId));
            return pick?.ValueAr;
        }

        public static string GetLookupIdByVisaStatus(VisaStatus? status)
        {
            return LookupHelper.GetLookupItemIdByCol1(System.Configuration.ConfigurationManager.AppSettings["StatusLookupTypeId"], status.ToString());
        }

        public static string GetLookupIdByVisaType(string visaType)
        {

            return GetLookupItemIdByCol1(System.Configuration.ConfigurationManager.AppSettings["ServiceTypeLookupId"], visaType);
        }

        public static string GetLookupIdByDashboardSearchType(string visaType)
        {
            if (string.IsNullOrEmpty(visaType))
                visaType = Convert.ToString((int) DashboardSearchType.EntryPermit);

            return GetLookupItemIdByCol1(System.Configuration.ConfigurationManager.AppSettings["ServiceTypeLookupId"], ((DashboardSearchType)int.Parse(visaType)).GetAttribute<DashboardSearchTypeAttr>().Type);
        }
        public static string GetLookupIdByVisaTypeCol2(string visaType)
        {
            return string.IsNullOrEmpty(visaType) ? string.Empty : LookupHelper.GetLookupItemIdByCol2(System.Configuration.ConfigurationManager.AppSettings["ServiceTypeLookupId"], visaType);
        }
        public static List<string> GetLookupItemsByCol4(string lookup, string col4Value)
        {
            List<string> selectedList = new List<string>();
            var lookupsList = ApiFactory.Default.GetLookupApi()
                       .GetLookupByCodeCol4Paging(lookup, col4Value, "1", "2000")
                       .Data.ToList();
            foreach (var item in lookupsList)
            {
                selectedList.Add(item.ItemId);
            }
            return selectedList;
        }
    }
}