using Emaratech.Services.Channels.Contracts.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using log4net;
using Emaratech.Services.UserManagement.Model;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Contracts.Errors;
using System.Text.RegularExpressions;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService : IUserService
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(ChannelService));

        List<string> GetUnifiedSystemIds()
        {
            return new List<string>
            {
                System.Configuration.ConfigurationManager.AppSettings["UnifiedMobileSystemId"],
                System.Configuration.ConfigurationManager.AppSettings["UnifiedWebSystemId"],
            };
        }

        public async Task<bool> CheckAvailability(string username)
        {
            var availability = await ApiFactory.Default.GetUserApi()
                .AvailabilityAsync(new UserAvailability { SystemIds = GetUnifiedSystemIds(), Username = username, AuthenticationType = "local" });

            return availability != null && availability.Value;
        }

        public async Task<bool> CheckAvailabilityEmail(string email)
        {
            var availability =
                await ApiFactory.Default.GetUserApi()
                    .FindByProfileAsync(new UserProfileSearch { SystemIds = GetUnifiedSystemIds(), ProfileName = Constants.Email, ProfileValue = email,AuthenticationType = "local" });
            return availability == null;
        }

        private async Task<string> GetKeyValue(string token, string key)
        {
            var tokenInContext = await TokenUtils.VerifyTokenInContext(SystemId, token);
            return tokenInContext.Single(x => x.Name == key).Value;
        }

        void ValidateEmiratesIdPreExists(string emiratesId)
        {
            if (ApiFactory.Default.GetUserApi().FindByProfile(new UserProfileSearch(SystemIds: GetUnifiedSystemIds(),
                ProfileName: Constants.EmiratesId, ProfileValue: emiratesId,AuthenticationType: "local")) != null)
            {
                throw ChannelErrorCodes.UserAlreadyRegisteredOnThisProfile.ToWebFault($"User is already registered in {SystemId} with EmiratesId {emiratesId}.");
            }
        }

        public async Task RegisterUser(RestUser user)
        {
            string verifiedMobileNumber;

            // Check Username availability in the requested System
            if (!await CheckAvailability(user.Username))
            {
                throw ChannelErrorCodes.UsernameNotAvailable.ToWebFault($"The user {user.Username} is not available for the system {SystemId}");
            }

            // Check Email address availability in the requested System
            if (!await CheckAvailabilityEmail(user.Email))
            {
                throw ChannelErrorCodes.EmailNotAvailable.ToWebFault($"The email address or username is already registered. Please login or try a new one.");
            }

            // Check Password Complexity
            var passwordRegex =
                new Regex(System.Configuration.ConfigurationManager.AppSettings["PasswordComplexityRegex"]);

            if (!passwordRegex.IsMatch(user.Password))
                throw ChannelErrorCodes.PasswordStrengthFailed.ToWebFault($"Password should not be less than 8 characters and contains at least one letter, one digit & one special character.");

            // Check Profile (EmiratesId) in the requested System
            if (ConfigurationRepository.GetIsUsedEmiratesIdAllowed() == "true")
                ValidateEmiratesIdPreExists(user.EmiratesId);

            var individualProfile = visionApi.GetIndividualDetailedInformation(
                user.EmiratesId,
                user.DateOfBirth);

            // Optional Flow for registring a new Mobile number for user account
            if (string.IsNullOrEmpty(user.RegisterNewMobileNumberToken))
            {
                Log.Debug($"Register with existing mobile number flow.");
                ValidateUser(individualProfile);

                verifiedMobileNumber = await GetKeyValue(user.OTPToken, Constants.Mobile);
                if (
                    !individualProfile.IndividualContactDetails.Select(x => x.CONTACTDETAIL)
                        .Contains(verifiedMobileNumber))
                {
                    throw ChannelErrorCodes.IncorrectMobileNumber.ToWebFault($"Mobile number {verifiedMobileNumber} is not found in profile");
                }
            }
            else
            {
                Log.Debug($"Register with new existing mobile number flow. Request {user.RegisterNewMobileNumberToken}");
                ValidateUser(individualProfile, validateMobileNumber: false);

                verifiedMobileNumber = await GetKeyValue(user.OTPToken, Constants.Mobile);
                var emiratesId = await GetKeyValue(user.RegisterNewMobileNumberToken, Constants.EmiratesId);
                Log.Debug($"Mobile Number Token is {Newtonsoft.Json.JsonConvert.SerializeObject(user.RegisterNewMobileNumberToken)}");
                if (emiratesId != user.EmiratesId)
                {
                    Log.Debug($"Emirates Id from token {emiratesId} && user emirates id is {user.EmiratesId}");
                    throw ChannelErrorCodes.IncorrectMobileNumber.ToWebFault($"Token EmiratesId and Requested EmiratesId Didnt Match");
                }
            }

            var userId = ApiFactory.Default.GetUserApi().AddUser(new UserData
            {
                Username = user.Username,
                Password = user.Password,
                AuthenticationType = "local",
                Profile = new List<Profile>()
                {
                    {new Profile {Name = Constants.Email, Value = user.Email}},
                    {new Profile {Name = Constants.Mobile, Value = verifiedMobileNumber}},
                    {new Profile {Name = Constants.EmiratesId, Value = user.EmiratesId}},
                    {
                        new Profile
                        {
                            Name = Constants.FirstNameEn,
                            Value = individualProfile.IndividualProfileInformation.FirstNameEn
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.FirstNameAr,
                            Value = individualProfile.IndividualProfileInformation.FirstNameAr
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.PassportNo,
                            Value = individualProfile.IndividualPassportInformation?.PassportNumber
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.NationalityId,
                            Value = individualProfile.IndividualProfileInformation.NationalityId.ToString()
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.GenderId,
                            Value = individualProfile.IndividualProfileInformation.GenderId.ToString()
                        }
                    },
                    {new Profile {Name = Constants.DateOfBirth, Value = user.DateOfBirth.ToString("yyyyy-MM-dd")}},
                    {new Profile {Name = Constants.FileTypeId, Value = individualProfile.IndividualSponsorshipInformation?.SponsorshipFileTypeId?.ToString()}},
                    {
                        new Profile
                        {
                            Name = Constants.VisaNumber,
                            Value = individualProfile.IndividualResidenceInfo?.ResidenceNo
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.Ppsid,
                            Value = individualProfile.IndividualProfileInformation.PPSID.ToString()
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.JobId,
                            Value = individualProfile.IndividualProfileInformation.ProfessionId?.ToString()
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.SponsorNo,
                            Value = individualProfile.IndividualSponsorshipInformation.SponsorshipNo
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.UserType,
                            Value =  LookupHelper.GetLookupItemIdByCol1(
                                System.Configuration.ConfigurationManager.AppSettings["UserTypeLookupId"],
                                individualProfile.IndividualSponsorshipInformation.SponsorshipFileTypeId.ToString())
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.SponsorType,
                            Value = individualProfile.IndividualResidenceInfo?.SponsorTypeId?.ToString()
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.EstablishmentType,
                            Value = individualProfile.IndividualResidenceInfo?.EstablishmentTypeId?.ToString()
                        }
                    },
                    {
                        new Profile
                        {
                            Name = Constants.UDBNo,
                            Value = individualProfile.IndividualProfileInformation.UDBNo
                        }
                    },
                         {
                        new Profile
                        {
                            Name = Constants.EstablishmentCode,
                            Value = string.Empty
                        }
                    },                                                            {
                        new Profile
                        {
                            Name = Constants.EstablishmentStatusEn,
                            Value = string.Empty
                        }
                    },
                        new Profile
                        {
                            Name = Constants.EstablishmentStatusAr,
                            Value = string.Empty
                        }
                }
            });

            //TODO: Refactor Usermanagement API to accept user system assignment with null system assignments / system user profile
            ApiFactory.Default.GetUserApi().AssignUserToSystem(userId, System.Configuration.ConfigurationManager.AppSettings["UnifiedMobileSystemId"], new SystemAssignment { Profiles = new List<Profile> { } });
            ApiFactory.Default.GetUserApi().AssignUserToSystem(userId, System.Configuration.ConfigurationManager.AppSettings["UnifiedWebSystemId"], new SystemAssignment { Profiles = new List<Profile> { } });
        }

    }
}