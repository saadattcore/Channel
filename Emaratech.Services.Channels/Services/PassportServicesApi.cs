using Emaratech.Services.Channels.Contracts.Rest;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Helpers;
using System.Xml.Serialization;
using Emaratech.Services.Channels.Contracts.Rest.Models.PassportService;
using System.IO;
using Emaratech.Services.WcfCommons.Faults.Models;
using System.Collections.Specialized;

namespace Emaratech.Services.Channels.Services
{
    public class PassportServicesApi : IPassportServicesApi
    {
        private readonly string baseUrl;

        public string ApiKey { get; set; }

        public PassportServicesApi(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task DeleteDocument(string documentId)
        {
            var res = await HttpHelper.Delete($"{baseUrl}/document/remove?fileId={documentId}", "{}", GetHeaders());
        }

        public async Task DeleteRenewRequest(string requestId)
        {
            var request = JObject.FromObject(new { });
            var res = await HttpHelper.Delete(baseUrl + "/txnPassport/remove", request.ToString(Newtonsoft.Json.Formatting.None), GetHeaders());
        }

        public async Task<JArray> FetchNewRequest(string emiratesId, string unifiedNumber)
        {
            var request = JObject.FromObject(new
            {
                nidNumber = emiratesId,
                udbNumber = unifiedNumber
            });
            var res = await HttpHelper.Post(baseUrl + "/txnPassport/newborn/fetch", request.ToString(Newtonsoft.Json.Formatting.None), GetHeaders());
            HandleErrors(res);
            return JArray.Parse(res);
        }

        public async Task<JArray> FetchRenewRequest(string passportNo, DateTime birthDate)
        {
            var request = JObject.FromObject(new
            {
                oldPassportNumber = passportNo,
                dob = birthDate.ToString("yyyy-MM-dd")
            });
            var res = await HttpHelper.Post(baseUrl + "/txnPassport/fetch", request.ToString(Newtonsoft.Json.Formatting.None), GetHeaders());
            HandleErrors(res);
            return JArray.Parse(res);
        }

        public async Task<JArray> GetAreaLookup()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/areas", GetHeaders());
            return JArray.Parse(res);
        }

        public async Task<JArray> GetCityLookup()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/cities", GetHeaders());
            return JArray.Parse(res);
        }

        public async Task<JArray> GetEmirateLookup()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/emirates", GetHeaders());
            return JArray.Parse(res);
        }

        public async Task<JArray> GetNewDocuments()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/newborn/requiredDocs", GetHeaders());
            return JArray.Parse(res);
        }

        public async Task<JArray> GetRenewDocuments()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/requiredDocs", GetHeaders());
            return JArray.Parse(res);
        }

        public async Task<JToken> SaveNewRequest(JObject request)
        {
            var res = await HttpHelper.Post(baseUrl + "/txnPassport/appManager/newborn", request.ToString(Newtonsoft.Json.Formatting.None), GetHeaders());
            HandleErrors(res);
            return JToken.Parse(res);
        }

        public async Task<JToken> SaveRenewRequest(JObject request)
        {
            var res = await HttpHelper.Post(baseUrl + "/txnPassport/appManager", request.ToString(Newtonsoft.Json.Formatting.None), GetHeaders());
            HandleErrors(res);
            return JToken.Parse(res);
        }

        public async Task<string> UploadDocument(JObject document)
        {
            return await HttpHelper.Post(baseUrl + "/document/add", document.ToString(Newtonsoft.Json.Formatting.None), GetHeaders());
        }

        private static void HandleErrors(string response)
        {
            if (response.StartsWith("<"))
            {
                using (var stringReader = new StringReader(response))
                {
                    var serializer = new XmlSerializer(typeof(PassportServiceValidationErrors));
                    var ex = (PassportServiceValidationErrors)serializer.Deserialize(stringReader);
                    throw ErrorCodes.BadRequest.ToWebFault(ex.Errors.FirstOrDefault()?.Message);
                }
            }
        }

        private NameValueCollection GetHeaders()
        {
            var headers = new NameValueCollection();
            headers.Add("apiKey", ApiKey);
            return headers;
        }
    }
}