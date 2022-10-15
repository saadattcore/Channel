using Emaratech.Services.Channels.Contracts.Rest;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Contracts.Rest.Models.LegalAdvice;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Emaratech.Services.Channels.Services
{
    public class LegalAdviceApi : ILegalAdviceApi
    {
        private readonly string baseUrl;

        public string ApiKey { get; set; }

        public LegalAdviceApi(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<IEnumerable<AdviceTypeLookup>> GetAdviceTypeLookup()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/advice_types", GetHeaders());
            return JsonConvert.DeserializeObject<IEnumerable<AdviceTypeLookup>>(res);
        }

        public async Task<IEnumerable<ApplicantTypeLookup>> GetApplicantTypeLookup()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/applicant_types", GetHeaders());
            return JsonConvert.DeserializeObject<IEnumerable<ApplicantTypeLookup>>(res);
        }

        public async Task<IEnumerable<NationalityLookup>> GetNationalityLookup()
        {
            var res = await HttpHelper.Get(baseUrl + "/lookup/nationality", GetHeaders());
            return JsonConvert.DeserializeObject<IEnumerable<NationalityLookup>>(res);
        }

        public async Task<JObject> SaveLegalAdvice(JObject legalAdvice)
        {
            var res = await HttpHelper.Post(baseUrl + "/advice/advice_manager", legalAdvice.ToString(Formatting.None), GetHeaders());
            return JObject.Parse(res);
        }

        public async Task<JObject> Fetch(string adviceNumber)
        {
            var json = JObject.FromObject(new
            {
                adviceNumber = adviceNumber
            });
            var res = await HttpHelper.Post(baseUrl + "/advice/advice_manager/fetchAdvice", json.ToString(Formatting.None), GetHeaders());
            return JObject.Parse(res);
        }

        public async Task<string> UploadDocument(JObject document)
        {
            var json = document.ToString();
            return await HttpHelper.Post(baseUrl + "/advice/legal_attachment", json, GetHeaders());
        }

        public async Task DeleteDocument(string documentId)
        {
            var attachment = JObject.FromObject(new
            {
                attachmentId = documentId
            });
            var res = await HttpHelper.Delete(baseUrl + "/advice/legal_attachment", attachment.ToString(Formatting.None), GetHeaders());
        }

        private NameValueCollection GetHeaders()
        {
            var headers = new NameValueCollection();
            headers.Add("apiKey", ApiKey);
            return headers;
        }
    }
}