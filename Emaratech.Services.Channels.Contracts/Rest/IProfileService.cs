using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.WcfCommons.Faults;
using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.WcfCommons.Faults.Models;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IProfileService
    {
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Get Dependents Visa Information", "Get Dependents Visa Information for the Dashboard", "FetchDependentVisaInformation")]
        [WebGet(UriTemplate = "/users/dependents", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Task<RestIndividualDashboard> FetchDependentsVisaInformation();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Establishment")]
        [SwaggerWcfPath("Get Dependents Visa Information", "Get establishment dependents Visa Information for the Dashboard", "FetchEstablishmentDependentsVisaInformation")]
        [WebGet(UriTemplate = "/establishment/dependents", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Task<RestEstablishmentDependentsSummary> FetchEstablishmentDependentsVisaInformation();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Get User Profile", "Get User Profile", "GetUserProfile")]
        [WebInvoke(Method = "GET", UriTemplate = "/users/profile", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestUserProfile> GetUserProfile();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Get User Profile Image", "Get User Profile Image", "GetUserProfileImage")]
        [WebInvoke(Method = "GET", UriTemplate = "/users/profile/image", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Task<Stream> GetUserProfileImage();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Update User Profile Image", "Update User Profile Image", "UpdateUserProfileImage")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/profile/image", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Task UpdateUserProfileImage(RestProfileImage image);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Update user profile email", "Update user profile email", "UpdateUserProfileEmail")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/profile", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task UpdateUserProfileEmail(RestProfileEmail email);


        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Delete user profile image", "Delete user profile image", "DeleteUserProfileImage")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/profile/deleteImage", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task DeleteUserProfileImage();


        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Fetch users search result", "Fetch users search result", "FetchDashboardSearchResult")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/search", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestIndividualDashboard> FetchDashboardSearchResult(RestDashboardSearchRequest searchParams);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Fetch draft applications", "Fetch draft applications of the dependents of user", "FetchDraftApplications")]
        [WebGet(UriTemplate = "/users/applications/draft", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestDependentApplicationInfo>> FetchDraftApplications();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Get User Profile barcode", "Get User Profile", "GetUserProfileBarCode")]
        [WebInvoke(Method = "GET", UriTemplate = "/users/profile/{type}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<Stream> GetUserProfileBarCode(string type);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfPath("Get User Profile", "Get User Detailed Profile ", "GetUserDetailedProfile")]
        [WebInvoke(Method = "GET", UriTemplate = "/users/profile/detail", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestUserDetailedProfile> GetUserDetailedProfile();
    }
}
