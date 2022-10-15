using Emaratech.Services.Channels.Contracts.Rest.Models;
using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.WcfCommons.Faults.Models;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IUserService
    {

        [SwaggerWcfTag("Registration")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfPath("Send OTP code for verification", "Send OTP code for verification", "SendOtpCode")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/mobile/verification/init", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<string> SendOtpCode(VerificationInitRequest request);

        [SwaggerWcfTag("Registration")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfPath("Verify OTP code sent on mobile number", "Verify OTP code sent on mobile number", "VerifyOtpCode")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/mobile/verification/check", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<string> VerifyOtpCode(RestOtpVerificationRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Registration")]
        [SwaggerWcfPath("Fetch User Mobiles", "Fetch User Mobile Numbers From Vision", "FetchUserMobiles")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/mobile", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Task<RestProfile> FetchUserMobiles(RestProfileSearch request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Registration")]
        [SwaggerWcfPath("Fetch Security Questions for Verification", "Fetch Security Questions for Verification", "FetchUserVerificationQuestions")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/question/verification/init", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Task<RestProfileSecurityQuestions> FetchVerificationQuestions(RestProfileSearch request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Registration")]
        [SwaggerWcfPath("Verify Security Answers", "Verify Security Answers", "VerifySecurityAnswers")]
        [WebInvoke(Method = "POST", UriTemplate = "/users/question/verification/check", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Task<string> VerifySecurityAnswers(RestProfileSecurityAnswers request);


        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Registration")]
        [SwaggerWcfPath("Check Username Availability", "API to check if the username is already registered in the system", "CheckUsernameAvailability")]
        [WebInvoke(Method = "GET", UriTemplate = "/users/username/{username}/available", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<bool> CheckAvailability(string username);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Registration")]
        [SwaggerWcfPath("Check Email Availability", "API to check if the email is already registered in the system", "CheckAvailabilityEmail")]
        [WebInvoke(Method = "GET", UriTemplate = "/users/email/{email}/available", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<bool> CheckAvailabilityEmail(string email);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("User")]
        [SwaggerWcfTag("Registration")]
        [SwaggerWcfPath("Register User", "API to register a Unified Channel user", "RegisterUser")]
        [WebInvoke(Method = "POST", UriTemplate = "/users", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task RegisterUser(RestUser user);
        
    }
}
