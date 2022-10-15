using System.ServiceModel;
using System.ServiceModel.Web;
using SwaggerWcf.Attributes;
using Emaratech.Services.WcfCommons.Faults.Models;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IEstablishmentService
    {
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Establishment")]
        [SwaggerWcfPath("Get Establishment user Information", "Get Establishment user Information for the logged in user", "FetchEstablishmentUserInfo")]
        [WebInvoke(Method = "GET", UriTemplate = "/establishment/user/{username}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        RestIndividualUserFormInfo FetchEstablishmentUserInfo(string username);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Establishment")]
        [SwaggerWcfPath("Fetch establishment profiles search result", "Fetch establishment profiles search result", "FetchEstablishmentDashboardSearchResult")]
        [WebInvoke(Method = "POST", UriTemplate = "/establishment/search", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestEstablishmentDashboard> FetchEstablishmentDashboardSearchResult(RestEstablishmentSearchRequest searchParams);
    }
}
