using Emaratech.Services.Channels.Contracts.Rest.Models.LegalAdvice;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    public interface ILegalAdviceApi
    {
        Task<JObject> Fetch(string adviceNumber);
        Task<JObject> SaveLegalAdvice(JObject legalAdvice);
        Task<string> UploadDocument(JObject document);
        Task DeleteDocument(string documentId);
        Task<IEnumerable<NationalityLookup>> GetNationalityLookup();
        Task<IEnumerable<AdviceTypeLookup>> GetAdviceTypeLookup();
        Task<IEnumerable<ApplicantTypeLookup>> GetApplicantTypeLookup();
    }
}