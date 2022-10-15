using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Common.Caching;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Lookups.Model;
using log4net;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels
{
    public static class DataHelper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(DataHelper));
        private const string GetObjectPathCachePrefix =nameof(GetObjectPath);

        public static object GetPropertyFromObject(string property, object obj)
        {
            string value = null;
            if (!string.IsNullOrEmpty(property) && obj != null)
            {
                var props = property.Split('.');
                var type = obj?.GetType();
                foreach (var prop in props)
                {
                    if (type != null)
                    {
                        var pi = type.GetProperty(prop);
                        if (pi != null)
                        {
                            obj = pi.GetValue(obj);
                            type = obj?.GetType();
                            value = obj?.ToString();
                        }
                    }
                }
                return value;
            }
            return null;
        }

        public static string GetFormattedDate(string stringToFormat, string format = "yyyy-MM-dd")
        {
            DateTime date;
            if (TryParseDateTimeExtended(stringToFormat, out date))
            {
                return date.ToString(format);
            }
            return stringToFormat;
        }

        public static async Task<Dictionary<string, string>> GetMatrixParameters(JObject unifiedApplication, string matrixId, string systemId)
        {
            var result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(matrixId))
            {
                var matrix = await ServicesHelper.GetMappingMatrix(matrixId);

                // Get the other dimensions val
                var dimensionsAndParameters = matrix.Dimensions.Select(d => d.ColumnName)
                    .Concat(matrix.Parameters.Select(p => p.ParameterName));

                foreach (var name in dimensionsAndParameters)
                {
                    // If there is no parameter for this dimension, add it
                    if (!result.ContainsKey(name))
                    {
                        var value = await GetFieldValue(unifiedApplication, systemId, name);
                        if (value != null)
                        {
                            result.Add(name, value);
                        }
                    }
                }
            }
            return result;
        }

        class ObjectPath
        {
            public string Entity { get; set; }
            public string Field { get; set; }
        }
        public static async Task<string> GetFieldValue(JObject unifiedApplication, string systemId, string name)
        {
            var appFieldsMatrixId = await ServicesHelper.GetSystemProperty(systemId, Constants.SystemProperties.MappingMatrixApplicationFieldsIdKey);
            var fieldPaths = await GetObjectPath(appFieldsMatrixId, name);

            return fieldPaths.Select(fieldPath => unifiedApplication[fieldPath.Entity][fieldPath.Field]?.Value<string>()).FirstOrDefault(node => node != null);
        }

        private static Task<IEnumerable<ObjectPath>> GetObjectPath(string matrixId, string logicalFieldId)
        {
            if (!string.IsNullOrEmpty(matrixId) && !string.IsNullOrEmpty(logicalFieldId))
            {
                return Cache.Default.Run(GetObjectPathCachePrefix,  $"{matrixId}_{logicalFieldId}",
                async () =>
                {

                    var paths = await ServicesHelper.ResolveMappingMatrix(
                        matrixId,
                        new List<string> {logicalFieldId},
                        (values, versions) => values[1]);

                    return paths.Select(x=>x.Split('/')).Select(x=>new ObjectPath() {Entity = x[0],Field = x[1]});
                })
                ;
            }
            return Task.FromResult(default(IEnumerable<ObjectPath>));
        }

        public static bool TryParseDateTimeExtended(string s, out DateTime result)
        {
            if (DateTime.TryParse(s, out result))
            {
                return true;
            }
            else if (DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return true;
            }
            return false;
        }

        public static async Task<IEnumerable<RestLookupDetail>> GetResidencyPickupValues(string systemId)
        {
            var residencyPickupLookupId = await ServicesHelper.GetSystemProperty(systemId, Constants.SystemProperties.ResidencyPickupLookupIdKey);
            if (!string.IsNullOrEmpty(residencyPickupLookupId))
            {
                return await ServicesHelper.GetLookupItems(residencyPickupLookupId);
            }
            return new List<RestLookupDetail>();
        }

        private static FormField GetFieldByName(string name,IEnumerable<RenderLayoutItem> items)
        {
            foreach (var item in items)
            {
                if (item.Field != null && item.Field.Name == name)
                {
                    var field = new FormField { Name = item.Field.Name, OriginalField = item.Field };
                    return field;
                }
                var innerField =  GetFieldByName(name,item.Layout);
                if (innerField != null)
                    return innerField;
            }
            return null;
        }

        public static FormField GetFieldByName(string name,RestRenderGraph layout)
        {
            if (layout != null)
            {
                var items = layout.VerticalItems
                    .Concat(layout.CustomItems)
                    .Concat(layout.Rows.SelectMany(r => r.Items))
                    .Concat(layout.Tabs.SelectMany(r => r.Items));
                var field = GetFieldByName(name, items);
                return field;
            }
            return null;
        }

        /// <summary>
        /// Get a dictionary with list of fields name by entity name
        /// </summary>
        /// <param name="layout">The form layout</param>
        /// <returns>The dictionary of fields by entity</returns>
        public static Dictionary<string, List<FormField>> GetFormFieldByEntity(RestRenderGraph layout)
        {
            var res = new Dictionary<string, List<FormField>>();
            if (layout != null)
            {
                var items = layout.VerticalItems
                    .Concat(layout.CustomItems)
                    .Concat(layout.Rows.SelectMany(r => r.Items))
                    .Concat(layout.Tabs.SelectMany(r => r.Items));

                foreach (var item in items)
                {
                    var fieldsByEntity = GetFormFieldByEntity(item.Layout);
                    foreach (var field in fieldsByEntity)
                    {
                        var fields = new List<FormField>(field.Value);
                        if (res.ContainsKey(field.Key))
                        {
                            fields.AddRange(res[field.Key]);
                        }
                        res[field.Key] = fields;
                    }

                    if (item.Field != null && !string.IsNullOrEmpty(item.Field.Entity))
                    {
                        var field = new FormField { Name = item.Field.Name, OriginalField = item.Field };

                        // TODO: client should check this
                        // If the field is a phone number, add a dash after prefix
                        if (item.Field.Name == Constants.VisaNumber)
                        {
                            field.CustomBehavior = FieldCustomBehavior.VisaNumber;
                        }
                        else if (item.Field.FieldTypeId == "00000000000000000000000000000026")
                        {
                            field.CustomBehavior = item.Field.Mobile.HasValue && item.Field.Mobile.Value ? FieldCustomBehavior.MobilePhone : FieldCustomBehavior.Phone;
                        }
                        else if (item.Field.FieldTypeId == "00000000000000000000000000000020")
                        {
                            field.CustomBehavior = FieldCustomBehavior.Lookup;
                        }
                        else if (item.Field.FieldTypeId == "00000000000000000000000000000014")
                        {
                            field.CustomBehavior = FieldCustomBehavior.Date;
                        }

                        if (res.ContainsKey(item.Field.Entity))
                        {
                            res[item.Field.Entity].Add(field);
                        }
                        else
                        {
                            res[item.Field.Entity] = new List<FormField> { field };
                        }
                    }
                }
            }
            return res;
        }

        public static void FilterLookup(string fieldName, IEnumerable<string> values, RestRenderGraph formConfiguration)
        {
            if (!string.IsNullOrEmpty(fieldName) && values != null && formConfiguration != null)
            {
                var field = GetFieldByName(fieldName, formConfiguration);
                if (field != null)
                {
                    LOG.Debug($"{fieldName} filtered with values: {string.Join(",", values)}");
                    field.OriginalField.Filter = values.ToList();
                }
            }
        }

        const string EntryPermitNumberYearPrefix = "20";
        public static string FormatVisaNumber(bool isResidence, string value, string yearPrefix = EntryPermitNumberYearPrefix)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (isResidence)
                {
                    return value.Insert(3, "/").Insert(8, "/");
                }

                string firstPart = value.Substring(0, 3);
                int numberOne = Convert.ToInt32(value.Substring(3, 2), 10);
                string year = value.Substring(5, 2);
                string numbersRest = value.Substring(7);
                return $"{firstPart}/{yearPrefix}{year}/{numberOne}{numbersRest}";
            }
            return null;
        }

        public static string UnformatVisaNumber(bool isResidence, string value, string yearPrefix = EntryPermitNumberYearPrefix)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("/", string.Empty);
                if (isResidence)
                {
                    return value;
                }

                string firstPart = value.Substring(0, 3);
                string year = value.Substring(3 + yearPrefix.Length, 2);
                string numbers = value.Substring(7);
                string numbersRest = numbers.Substring(numbers.Length - 6);
                string numberOne = NormalizeNumber(numbers.Substring(0, numbers.Length - numbersRest.Length));

                return $"{firstPart}{numberOne}{year}{numbersRest}";
            }
            return null;
        }

        static string NormalizeNumber(string n)
        {
            return Convert.ToInt32(n, 10) < 10 ? $"0{n}" : n;
        }
    }
}