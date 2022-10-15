using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IRenewPasswordService
    {
        [SwaggerWcfTag("RenewPassword")]
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfPath("Initiate renew password service", "Initate sending of otp code to allow setting of new password", "InitiateChange")]
        [WebInvoke(Method = "POST", UriTemplate = "/password/renewstart", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<string> ForgetPasswordInitiate(RestUsernameAndEmiratesId request);

        [SwaggerWcfTag("RenewPassword")]
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfPath("Complete password change", "Complete password change if otp correct.", "CompleteChange2")]
        [WebInvoke(Method = "POST", UriTemplate = "/password/renewcomplete", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<string> ForgetPasswordComplete(RestOtpVerificationPasswordChangeRequest request);


        [SwaggerWcfTag("RenewPassword")]
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfPath("Initiate forget username service", "Initiate forget username service", "ForgetUsernameInitiate")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/username/forget/init", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<string> ForgetUsernameInitiate(RestEmiratesId request);



    }
}