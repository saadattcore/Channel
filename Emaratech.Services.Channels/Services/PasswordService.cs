using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.SMS.Model;
using Emaratech.Services.UserManagement.Model;
using Emaratech.Services.WcfCommons.Faults.Models;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService : IRenewPasswordService
    {
        public async Task<string> ForgetPasswordInitiate(RestUsernameAndEmiratesId request)
        {
            Log.Debug($"Request for forgot password with data {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            string retValue = null;
            string cstUserTypeLocalLookupName = configurationRepository.GetUserTypeLocalLookupResult(); // should be "230AAC5F9259481F95A92AAC60F5D705";

            var systemUser = await FetchSystemUser(request.Username,"local");
            var userProfileData = await GetUserProfileDataFromSystemUser(systemUser);
            Log.Debug($"User profile data is {Newtonsoft.Json.JsonConvert.SerializeObject(userProfileData)}");

            var isLocalUser = string.Equals(cstUserTypeLocalLookupName, userProfileData.UserType);
            var hasMobile = !string.IsNullOrEmpty(userProfileData.Mobile);
            var isSameEmiratesId = string.Equals(request.EmiratesId, userProfileData.EmiratesId);

            Log.Debug($"Is Local User {isLocalUser} aand emirate is is same {isSameEmiratesId} and has mobile {hasMobile}");
            if (isSameEmiratesId && hasMobile)
            {
                retValue = await SendOtpCode(userProfileData.Mobile, new Dictionary<object, object>()
                {
                    { Constants.Mobile, userProfileData.Mobile },
                    { Constants.Username, request.Username }
                });
            }
            else
            {
                throw ChannelErrorCodes.InvalidArgumentsForPasswordReset.ToWebFault($"Invalid arguments for password reset.");
            }
            return retValue;
        }

        public async Task<string> ForgetPasswordComplete(RestOtpVerificationPasswordChangeRequest request)
        {
            var tokenData = await TokenUtils.VerifyTokenInContext(SystemId, request.OtpToken);
            var mobileNo = tokenData.Single(x => x.Name == Constants.Mobile).Value;
            var username = tokenData.Single(x => x.Name == Constants.Username).Value;

            await VerifyOtpCodeAgainstRemoteService(request, mobileNo, SystemId); // This will throw exception if invalid.
            var systemUser = await FetchSystemUser(username,"local");

            var version = ApiFactory
                .Default.GetUserApi()
                .ResetPassword(systemUser.UserId, new PasswordInfo() { Password = request.NewPassword });

            return $"{version}";
        }

        public async Task<string> ForgetUsernameInitiate(RestEmiratesId request)
        {
            var user = await GetUserWithEmiratesId(request.EmiratesId);
            if (user == null)
            {
                throw ChannelErrorCodes.PleaseTryAgain.ToWebFault($"Emirates Id {request.EmiratesId} not found");
            }

            var mobile = user.Profile.SingleOrDefault(x => x.Name == Constants.Mobile);
            if (mobile == null)
            {
                throw ChannelErrorCodes.IndividualUserMobileNotFound.ToWebFault($"Mobile number not found for user {user.UserId}, {user.Username} , Emirates Id {request.EmiratesId} not found")
                ;
            }
            await ApiFactory
                .Default.GetSmsApi().SendSMSAsync(new RestSmsRequest()
                {
                    Message = $"Your Username is:  {user.Username}",
                    MobileNo = mobile.Value,
                    SystemId = SystemId
                });



            return mobile.Value;

        }

        private async Task<SystemUser> FetchSystemUser(string username,string authenticationType)
        {
            string retValue = string.Empty;
            return await ApiFactory.Default.GetUserApi().GetSystemUserByUsernameAsync(new UserAvailability()
            {
                SystemIds = GetUnifiedSystemIds(),
                Username = username,
                AuthenticationType = authenticationType
            });
        }

        private async Task<UserProfileData> GetUserProfileDataFromSystemUser(SystemUser argSystemUser)
        {
            UserProfileData retValue = new UserProfileData();

            if (argSystemUser != null)
            {
                var user = await ApiFactory.Default.GetUserApi().GetUserAsync(argSystemUser.UserId);
                retValue.Mobile = user.Profile.FirstOrDefault(x => string.Equals(Constants.Mobile, x.Name))?.Value;
                retValue.EmiratesId = user.Profile.FirstOrDefault(x => string.Equals(Constants.EmiratesId, x.Name))?.Value;
                retValue.UserType = user.Profile.FirstOrDefault(x => string.Equals(Constants.UserType, x.Name))?.Value;
            }

            return retValue;
        }

        private async Task<UserAll> GetUserWithEmiratesId(string emiratesId)
        {
            var user = await  ApiFactory.Default.GetUserApi().FindByProfileAsync(new UserProfileSearch()
            {
                SystemIds = GetUnifiedSystemIds(),
                ProfileName = Constants.EmiratesId,
                ProfileValue = emiratesId,
                AuthenticationType = "local"
            });
            return user;
        }
        struct UserProfileData
        {
            public string Mobile { get; set; }
            public string EmiratesId { get; set; }
            public string UserType { get; set; }
        }
    }
}