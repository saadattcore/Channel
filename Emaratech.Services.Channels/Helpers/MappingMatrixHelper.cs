using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.MappingMatrix.Model;
using log4net;

namespace Emaratech.Services.Channels.Helpers
{
    public class MappingMatrixHelper
    {
        public async static Task<List<MappingMatrixResponse>> GetItemsFromMappingMatrix(string mappingMatrixId, Dictionary<string, string> criteria)
        {
            var searchVersion = criteria.Select(pair => new SearchVersion(pair.Key, pair.Value)).ToList();

            var searchCriteria = new SearchCriteria
            {
                IncludeExcluded = false,
                ValuesDict = searchVersion,
                ResolveExpressions = true
            };

            var dimensions = await ApiFactory.Default.GetMappingMatrixApi().GetMappingMatrixDimensionsAsync(mappingMatrixId, "");

            var items = await ApiFactory.Default.GetMappingMatrixApi().SearchAsync(mappingMatrixId, searchCriteria);

            var lstMappingMatrixItems = new List<MappingMatrixResponse>();

            foreach (var item in items)
            {
                var mappingMatrixResponse = new MappingMatrixResponse();
                mappingMatrixResponse.mappingMatrixItems = new Dictionary<string, string>();

                for (var i = 0; i < item.Values.Count; i++)
                {
                    mappingMatrixResponse.mappingMatrixItems.Add(dimensions[i].ColumnName, item.Values[i]);
                }
                lstMappingMatrixItems.Add(mappingMatrixResponse);
            }

            return lstMappingMatrixItems;
        }
    }

    public class MappingMatrixResponse
    {
        public Dictionary<string, string> mappingMatrixItems { get; set; }
    }
}