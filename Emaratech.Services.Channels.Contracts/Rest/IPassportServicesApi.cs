using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    public interface IPassportServicesApi
    {
        Task<JToken> SaveRenewRequest(JObject request);
        Task<JToken> SaveNewRequest(JObject request);
        Task DeleteDocument(string documentId);
        Task<JArray> FetchRenewRequest(string passportNo, DateTime birthDate);
        Task<JArray> FetchNewRequest(string emiratesId, string unifiedNumber);
        Task<JArray> GetCityLookup();
        Task<JArray> GetAreaLookup();
        Task<JArray> GetEmirateLookup();
        Task<JArray> GetRenewDocuments();
        Task<JArray> GetNewDocuments();
        Task<string> UploadDocument(JObject document);
    }
}