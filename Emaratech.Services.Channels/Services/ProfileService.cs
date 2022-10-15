using Emaratech.Services.Channels.Contracts.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Contracts.Errors;
using System.ServiceModel;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Models.Enums;
using System.Configuration;
using System.IO;
using System.ServiceModel.Web;
using Emaratech.Services.Channels.BusinessLogic.Dashboard;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Newtonsoft.Json;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Extensions;
using RestSharp;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService : IProfileService
    {
        public async Task<RestProfile> FetchUserMobiles(RestProfileSearch request)
        {
            ValidateEmiratesIdPreExists(request.EmiratesId);
            var result = await FetchProfileAndValidate(request.EmiratesId, request.DateOfBirth);
            return TransformMobileNumberDTO(result);

        }

        private async Task<RestIndividualInfo> FetchProfileAndValidate(string emiratesId, DateTime dateOfBirth, bool validateMobileNumber = true)
        {
            Log.Debug("Going to fetch profile from vision.");
            var result = await visionApi.GetIndividualDetailedInformationAsync(emiratesId, dateOfBirth);
            Log.Debug($"Profile successfully fetched from vision for emiratesId {emiratesId}");
            ValidateUser(result, validateMobileNumber);
            return result;
        }

        private List<QuestionCode> GetRandomQuestions(int count)
        {
            var random = new Random();
            var randomQuestions = new HashSet<QuestionCode>();

            var allQuestions = typeof(QuestionCode).GetEnumValues();

            while (randomQuestions.Count < count)
            {
                var question = (QuestionCode)allQuestions.GetValue(random.Next(0, allQuestions.Length));
                randomQuestions.Add(question);
            }
            return randomQuestions.ToList();
        }

        public async Task<RestProfileSecurityQuestions> FetchVerificationQuestions(RestProfileSearch request)
        {
            try
            {
                await FetchProfileAndValidate(request.EmiratesId, request.DateOfBirth, validateMobileNumber: false);
                var noOfRandomQuestions =
                    Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["NoOfRandomQuestions"]);
                var randomQuestions = GetRandomQuestions(noOfRandomQuestions);

                var token = await PayloadBuilder.New()
                    .Key(SystemId, TokenUtils.TokenKey(SystemId))
                    .Expiry(TimeSpan.FromHours(1))
                    .Add(Constants.EmiratesId, request.EmiratesId)
                    .Add(Constants.DateOfBirth, request.DateOfBirth)
                    .Add(randomQuestions, x => "QuestionType" + x, y => y)
                    .Issue(tokensApi);

                var questions = new RestProfileSecurityQuestions
                {
                    Token = token,
                    Questions =
                        randomQuestions.Select(x => new RestProfileSecurityQuestion
                        {
                            Code = x,
                            Question = x.ToString(),
                            Type = x.GetAttribute<QuestionTypeAttr>().Type.ToString(),
                            LookupId = x.GetAttribute<QuestionTypeAttr>().LookupId
                        })
                };

                return questions;
            }
            catch (FaultException e)
            {
                Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw ChannelErrorCodes.InternalServerError.ToWebFault(e.Message);
            }
        }

        private void AssertAnswer<T>(QuestionCode question, string emiratesId, DateTime dateOfBirth, T expectedValue, T providedValue)
        {
            if (!string.Equals(expectedValue.ToString(), providedValue.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Log.Info($"Invalid Answer Supplied: EmiratesId: {emiratesId}; DOB: {dateOfBirth}; Question: {question}; ExpectedAnswer: {expectedValue.ToString()}; ProvidedAnswer: {providedValue.ToString()}");

                throw ChannelErrorCodes.IncorrectAnswer.ToWebFault("Incorrect Answer: " + question);
            }
        }

        private void LogInvalidAnswers()
        {

        }

        public async Task<string> VerifySecurityAnswers(RestProfileSecurityAnswers request)
        {
            try
            {
                var questionToken = await TokenUtils.VerifyTokenInContext(SystemId, request.QuestionToken);

                var questions = questionToken
                    .Where(x => x.Name.StartsWith("QuestionType"))
                    .Select(
                        x => (QuestionCode)Enum.Parse(typeof(QuestionCode), x.Name.Substring("QuestionType".Length)))
                    .ToList();

                if (request.Answers.Select(x => x.Code).Distinct().Count() < request.Answers.Count() ||
                    request.Answers.Count != questions.Count ||
                    request.Answers.Select(x => x.Code).Except(questions).Any())
                {
                    throw ChannelErrorCodes.IncorrectAnswer.ToWebFault("Invalid answers request against asked questions");
                }

                var emiratesId = questionToken.Single(x => x.Name == Constants.EmiratesId).Value;
                var dateOfBirth = DateTime.Parse(questionToken.Single(x => x.Name == Constants.DateOfBirth).Value);

                var profile = await FetchProfileAndValidate(
                    emiratesId: emiratesId,
                    dateOfBirth: dateOfBirth,
                    validateMobileNumber: false);

                //var searchCriteria = new RestDashboardSearchRequest
                //{
                //    SponsorNo = profile.IndividualSponsorshipInformation?.SponsorshipNo,
                //    ResidenceNo = profile.IndividualResidenceInfo?.ResidenceNo,
                //    ServiceType = DashboardSearchType.EntryPermit.GetAttribute<DashboardSearchTypeAttr>().Type
                //};

                var dependents = (await visionApi.GetIndividualDependentsFilteredInformationAsync(profile.IndividualSponsorshipInformation?.SponsorshipNo, new RestIndividualDependentSearchCriteria())).IndividualDependents;

                if (dependents == null)
                    dependents = new List<RestIndividualDependentsInfoWrapper>();

                foreach (var answer in request.Answers)
                {
                    switch (answer.Code)
                    {
                        case QuestionCode.LastCountryEntryDate:
                            AssertAnswer<string>(answer.Code, emiratesId, dateOfBirth,
                                visionApi.GetIndividualTravelLastEntry(emiratesId, dateOfBirth)
                                    ?.TravelDate?.ToString("d"),
                                DateTime.Parse(answer.Answer).ToString("d"));
                            break;
                        case QuestionCode.NumberOfSponsoredResidents:
                            AssertAnswer<int>(answer.Code, emiratesId, dateOfBirth,
                                dependents.Count(
                                    x =>
                                        x.IndividualVisaInformation.VisaType ==
                                        VisaType.Residence.GetAttribute<VisaTypeAttr>().Type),
                                int.Parse(answer.Answer));
                            break;
                        case QuestionCode.NumberOfSponsoredVisas:
                            AssertAnswer<int>(answer.Code, emiratesId, dateOfBirth,
                                dependents.Count(
                                    x =>
                                       x.IndividualVisaInformation.VisaType ==
                                        VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type),
                                int.Parse(answer.Answer));
                            break;
                        case QuestionCode.PassportNumber:
                            AssertAnswer<string>(answer.Code, emiratesId, dateOfBirth,
                                profile.IndividualPassportInformation.PassportNumber,
                                answer.Answer);
                            break;
                        case QuestionCode.UnifiedNumber:
                            AssertAnswer<string>(answer.Code, emiratesId, dateOfBirth,
                                profile.IndividualProfileInformation.UDBNo,
                                answer.Answer);
                            break;
                    }
                }

                var answerToken = await PayloadBuilder.New()
                    .Key(SystemId, TokenUtils.TokenKey(SystemId))
                    .Expiry(TimeSpan.FromHours(1))
                    .Add(request.Answers, x => x.Code, y => y.Answer)
                    .Add(Constants.EmiratesId, emiratesId)
                    .Add(Constants.DateOfBirth, dateOfBirth)
                    .Issue(tokensApi);

                return answerToken;
            }
            catch (FaultException e)
            {
                Log.Error(e);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw ChannelErrorCodes.InternalServerError.ToWebFault(e.Message);
            }
        }

        private RestProfile TransformMobileNumberDTO(RestIndividualInfo individualInformation)
        {
            RestProfile restProfile = new RestProfile();
            restProfile.MobileNumbers = new List<string>();

            foreach (var individualContact in individualInformation.IndividualContactDetails)
                restProfile.MobileNumbers.Add(individualContact.CONTACTDETAIL);

            return restProfile;
        }

        public async Task<RestIndividualDashboard> FetchDependentsVisaInformation()
        {
            var searchCriteria = new RestDashboardSearchRequest
            {
                ApplicationStatuses = Convert.ToString(ConfigurationManager.AppSettings["StatusForDependentsInProgress"]),
                IsBatchIdMarked = true,
                UserId = ClaimUtil.GetAuthenticatedUserId(),
                SponsorNo = ClaimUtil.GetAuthenticatedSponsorNo(),
                ResidenceNo = ClaimUtil.GetAuthenticatedVisaNumber()
            };

            return await SearchDependents(searchCriteria);
        }

        public async Task<RestEstablishmentDependentsSummary> FetchEstablishmentDependentsVisaInformation()
        {
            var establishmentCode = ClaimUtil.GetEstablishmentCode();

            var establishmentSummary = await visionEstabApi.GetEstablishmentSummaryAsync(establishmentCode);

            if (establishmentSummary == null)
                return null;

            var establishmentDependents = mapper.Map<RestEstablishmentDependentsSummary>(establishmentSummary);

            establishmentDependents.EntryPermitCount = establishmentSummary.PermitDependents.Count;
            establishmentDependents.ResidenceCount = establishmentSummary.ResidenceDependents.Count;
            establishmentDependents.ExpiringSoonCount = establishmentSummary.PermitDependents.Count(p => (Convert.ToDateTime(p.ValidityDate) - DateTime.Now).TotalDays < 30) +
                                                       establishmentSummary.ResidenceDependents.Count(p => (Convert.ToDateTime(p.ValidityDate) - DateTime.Now).TotalDays < 30);

            establishmentDependents.OverstayEntryPermitCount = establishmentSummary.DependentsViolation?.PermitViolatorCount;
            establishmentDependents.OverstayResidenceCount = establishmentSummary.DependentsViolation?.ResidenceViolatorCount;

            establishmentDependents.UserStatus = new RestName
            {
                En = ClaimUtil.GetEstablishmentStatusEn(),
                Ar = ClaimUtil.GetEstablishmentStatusAr()
            };

            return establishmentDependents;
        }

        private async Task<RestIndividualDashboard> SearchDependents(RestDashboardSearchRequest searchCriteria)
        {
            searchCriteria.ApplicationModule = ApplicationModules.Dashboard;

            Log.Debug($"Going to perform search with criteria {JsonConvert.SerializeObject(searchCriteria)}");

            var applicationSearchResult = await IndividualDashboard.GetDependentApplicationSearchResult(searchCriteria);

            var individualDashboard = new RestIndividualDashboard();

            //Get applications of this user if there is no search criteria or criteria is application
            if (string.IsNullOrEmpty(searchCriteria?.ServiceType) || searchCriteria?.ServiceType == DashboardSearchType.Application.GetAttribute<DashboardSearchTypeAttr>().Type)
            {
                individualDashboard.DependentsApplicationInfo = await IndividualDashboard.GetDependentsApplication(applicationSearchResult);
                Log.Debug($"Total dependents applications filtered are {individualDashboard.DependentsApplicationInfo.Count}");
            }

            //Get visas of dependents of this user if there is no search criteria or criteria is entry permit permit or residence
            if (string.IsNullOrEmpty(searchCriteria?.ServiceType) || searchCriteria?.ServiceType == DashboardSearchType.EntryPermit.GetAttribute<DashboardSearchTypeAttr>().Type || searchCriteria?.ServiceType == DashboardSearchType.Residence.GetAttribute<DashboardSearchTypeAttr>().Type)
            {
                Log.Debug($"Going to get dependents from vision for {searchCriteria.UserId}");
                var dependentsProfiles = await visionApi.GetIndividualDependentsFilteredInformationAsync(searchCriteria.SponsorNo, mapper.Map<RestIndividualDependentSearchCriteria>(searchCriteria) ?? new RestIndividualDependentSearchCriteria());
                Log.Info("Dependent information found from vision " + dependentsProfiles?.IndividualDependents?.Count + " for sponsor no " + searchCriteria.SponsorNo);

                if (dependentsProfiles?.IndividualDependents == null)
                    individualDashboard.DependentsVisaInfo = new List<RestDependentVisaInfo>();
                else
                    individualDashboard.DependentsVisaInfo = await IndividualDashboard.GetDependentsVisas(searchCriteria.SponsorNo, applicationSearchResult, dependentsProfiles, searchCriteria);

                Log.Debug($"Total dependents profiles filtered are {individualDashboard.DependentsVisaInfo.Count}");
            }

            Log.Debug($"Dependents successfully filtered for {searchCriteria.UserId}");

            return individualDashboard;
        }

        public Task<RestUserProfile> GetUserProfile()
        {
            var userId = ClaimUtil.GetAuthenticatedUserId();

            var userObj = ApiFactory.Default.GetUserApi().GetUserAsync(userId).Result;

            var mappedProfile = Mapper.MapUserProfile(userObj);

            var result = visionApi.GetIndividualDetailedInformationBySponsorNo(mappedProfile.SponsorNo);

            var userType = Convert.ToInt16(LookupHelper.GetLookupCol1ById(ConfigurationManager.AppSettings["UserTypeLookupId"], mappedProfile.UserType));
            
            mappedProfile.name.ar = mappedProfile.FirstNameAr = result.IndividualUserInfo.IndividualProfileInformation.FullNameAr;

            mappedProfile.name.en = mappedProfile.FirstNameEn = result.IndividualUserInfo.IndividualProfileInformation.FullNameEn;

            mappedProfile.PassportExpiryDate = result?.IndividualUserInfo?.IndividualPassportInformation?.ExpiryDate;

            if (userType == (int)SponsorType.Resident)
            {
                mappedProfile.VisaExpiryDate = result?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceExpiryDate;
            }

            return Task.FromResult(mappedProfile);

        }

        public Task<Stream> GetUserProfileImage()
        {
            var userId = ClaimUtil.GetAuthenticatedUserId();

            var userProfile = ApiFactory.Default.GetUserApi().GetUserAsync(userId).Result.Profile;

            var docId = Mapper.GetUserProfileDocId(userProfile);

            var docDetail = ApiFactory.Default.GetDocumentApi().GetDocument(docId);

            MemoryStream documentStream = new MemoryStream(Convert.FromBase64String(docDetail.DocumentStream));
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";

            return Task.FromResult((Stream)documentStream);

        }

        public Task UpdateUserProfileImage(RestProfileImage image)
        {
            var newDoc = new Document.Model.UploadDocument()
            {
                DocumentTypeId = ConfigurationManager.AppSettings["ProfileImageDocTypeId"],
                DocumentStream = image.ProfileImage,
                SystemId = ConfigurationManager.AppSettings["UnifiedMobileSystemId"]
            };

            var docId = ApiFactory.Default.GetDocumentApi().AddDocument(newDoc);
            var userId = ClaimUtil.GetAuthenticatedUserId(); //"7E220C175A7F47EDA29123F5AF426E09";//

            var profile = new UserManagement.Model.UserProfile();

            profile.Profile = new List<UserManagement.Model.Profile>()
             {
                 {new UserManagement.Model.Profile {Name="Image" , Value =docId } }
            };
            ApiFactory.Default.GetUserApi().AddOrUpdateProfile(userId, profile);

            return Task.CompletedTask;
        }

        public Task UpdateUserProfileEmail(RestProfileEmail email)
        {
            var userId = ClaimUtil.GetAuthenticatedUserId(); //"7E220C175A7F47EDA29123F5AF426E09";// 

            var profile = new UserManagement.Model.UserProfile();

            profile.Profile = new List<UserManagement.Model.Profile>()
             {
                 {new UserManagement.Model.Profile {Name="Email" , Value =email.Email } }
            };
            ApiFactory.Default.GetUserApi().AddOrUpdateProfile(userId, profile);
            return Task.CompletedTask;
        }

        public Task DeleteUserProfileImage()
        {
            var userId = ClaimUtil.GetAuthenticatedUserId(); //"7E220C175A7F47EDA29123F5AF426E09";// 

            var userProfile = ApiFactory.Default.GetUserApi().GetUserAsync(userId).Result.Profile;

            var docId = Mapper.GetUserProfileDocId(userProfile);

            var profile = new UserManagement.Model.UserProfile();

            profile.Profile = new List<UserManagement.Model.Profile>()
             {
                 {new UserManagement.Model.Profile {Name="Image" , Value = string.Empty } }
            };
            ApiFactory.Default.GetUserApi().AddOrUpdateProfile(userId, profile);

            return Task.CompletedTask;
        }

        public async Task<RestIndividualDashboard> FetchDashboardSearchResult(RestDashboardSearchRequest searchParams)
        {
            searchParams.ApplicationStatuses = Convert.ToString(ConfigurationManager.AppSettings["StatusForDependentsInSearch"]);
            searchParams.UserId = ClaimUtil.GetAuthenticatedUserId();
            searchParams.SponsorNo = ClaimUtil.GetAuthenticatedSponsorNo();
            searchParams.ResidenceNo = ClaimUtil.GetAuthenticatedVisaNumber();

            return await SearchDependents(searchParams);
        }

        public async Task<IEnumerable<RestDependentApplicationInfo>> FetchDraftApplications()
        {
            Log.Debug("Going to fetch draft applications.");

            var searchCriteria = new RestDashboardSearchRequest
            {
                ApplicationStatuses = Convert.ToString(ConfigurationManager.AppSettings["StatusForDraftApplications"]),
                UserId = ClaimUtil.GetAuthenticatedUserId(),
            };

            var applicationSearchResult = await IndividualDashboard.GetDependentApplicationSearchResult(searchCriteria);

            var draftApplications = await IndividualDashboard.GetDependentsApplication(applicationSearchResult);
            Log.Debug($"Total draft applications filtered are {draftApplications.Count}");

            return draftApplications.AsEnumerable();
        }

        public Task<Stream> GetUserProfileBarCode(string type)
        {
            Log.Debug($"Trying to get profile barcode {type}");
            var userId = ClaimUtil.GetAuthenticatedUserId();

            var userObj = ApiFactory.Default.GetUserApi().GetUserAsync(userId).Result;

            var mappedProfile = Mapper.MapUserProfile(userObj);

            RestIndividualUserFormInfo info = null;

            if (mappedProfile.FileTypeId == ((int)SponsorType.Citizen).ToString())
            {
                info = visionApi.GetIndividualDetailedInformationBySponsorNo(mappedProfile.SponsorNo);
            }
            else
            {
                info = visionApi.GetIndividualDetailedInformationByResNo(mappedProfile.VisaNumber);

            }
            var passportNo = string.Empty;
            var emiratesId = string.Empty;
            var data = string.Empty;
            var countryISOCode = string.Empty;
            var dob = string.Empty;

            if (info != null && info.IndividualUserInfo != null)
            {
                if (BarCodeType.passport.GetAttribute<BarCodeAttr>().Type.Equals(type) && info.IndividualUserInfo.IndividualPassportInformation != null)
                {
                    passportNo = info.IndividualUserInfo.IndividualPassportInformation.PassportNumber;
                    string issueCountry = Convert.ToString(info.IndividualUserInfo.IndividualPassportInformation.IssueCountryId);
                    countryISOCode = LookupHelper.GetLookupCol1ById(ConfigurationManager.AppSettings["eCountryLookupId"], issueCountry);
                    dob = String.Format("{0:dd-MM-yyyy}", info.IndividualUserInfo.IndividualProfileInformation.BirthDate);
                    data = "{"
                          + "\"Type\":\"Passport\","
                          + "\"No\":\"" + passportNo + "\","
                          + "\"Nat\":\"" + countryISOCode + "\","
                          + "\"DOB\":\"" + dob
                          +
                         "\"}";

                }
                else if (BarCodeType.eida.GetAttribute<BarCodeAttr>().Type.Equals(type) && info.IndividualUserInfo.IndividualProfileInformation != null)
                {
                    emiratesId = info.IndividualUserInfo.IndividualProfileInformation.EmiratesIdNo;
                    var emiratesExpDate = String.Format("{0:dd-MM-yyyy}", Convert.ToDateTime(info.IndividualUserInfo.IndividualProfileInformation.EmiratesIdExpiryDate));
                    data = "{"
                           + "\"Type\":\"EIDA\","
                           + "\"No\":\"" + emiratesId + "\","
                           + "\"Expiry Date of Card\":\"" + emiratesExpDate
                           +
                          "\"}";
                }
                Log.Debug($"Barcode profile found {data}");

            }
            MemoryStream documentStream = new MemoryStream(Convert.FromBase64String(BuildBarcode(data)));
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";

            Log.Debug("Finished generating barcode");
            return Task.FromResult((Stream)documentStream);
        }

        private string BuildBarcode(string jsonData)
        {
            string SmartGateServiceUrl = ConfigurationManager.AppSettings["SmartGateApi"];
            RestSharp.RestClient client = new RestSharp.RestClient(SmartGateServiceUrl);
            RestSharp.RestRequest request = new RestSharp.RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Accept", "image/png");
            request.AddJsonBody(new RestBarCode
            {
                data = jsonData,
                height = "50",
                mode = "PDF",
                width = "640"
            }).AddHeader("Content-Type", "application/json;charset=UTF-8");

            var response = client.ExecuteAsPost(request, "POST");
            var imageByteArray = response.RawBytes;
            var strImageContent = System.Convert.ToBase64String(imageByteArray);
            return strImageContent;
        }

        public Task<RestUserDetailedProfile> GetUserDetailedProfile()
        {
            var userId =  ClaimUtil.GetAuthenticatedUserId();

            var userObj = ApiFactory.Default.GetUserApi().GetUserAsync(userId).Result;

            RestUserDetailedProfile mappedProfile = mapper.Map<RestUserDetailedProfile>(Mapper.MapUserProfile(userObj));

            var result = visionApi.GetIndividualDetailedInformationBySponsorNo(mappedProfile.SponsorNo);

            var userType = Convert.ToInt16(LookupHelper.GetLookupCol1ById(Constants.Lookups.UserTypeLookupId, mappedProfile.UserType));


            mappedProfile.PassportExpiryDate = result?.IndividualUserInfo?.IndividualPassportInformation?.ExpiryDate;

            if (userType == (int)SponsorType.Resident)
            {
                mappedProfile.VisaExpiryDate = result?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceExpiryDate;
                mappedProfile.VisaIssueDate = result?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceIssueDate;

                mappedProfile.VisaStatus = LookupHelper.GetEn(LookupHelper.GetLookupItems(ConstantsUtil.LookupIds.ResidenceStatus).Result,
                    result?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceStatusId.ToString());

                mappedProfile.ResidenceAccompanied = Convert.ToString(result?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceAccompanyCount);

                var emirateId = result?.IndividualUserInfo?.IndividualResidenceInfo?.EmiratesDepartmentId?.ToString().Substring(0, 1);

                mappedProfile.PlaceOfIssueEn = LookupHelper.GetEn(LookupHelper.GetLookupItems(ConstantsUtil.LookupIds.Emirate).Result,emirateId);

                mappedProfile.PlaceOfIssueAr = LookupHelper.GetAr(LookupHelper.GetLookupItems(ConstantsUtil.LookupIds.Emirate).Result, emirateId);
                  
                var estabCode = result?.IndividualUserInfo?.IndividualResidenceInfo?.EstCode;
                if (result.IndividualSponsorInfo != null)
                {
                    mappedProfile.SponsorNameEn = result.IndividualSponsorInfo.ProfileInfo?.FullNameEn;
                    mappedProfile.SponsorNameAr = result.IndividualSponsorInfo.ProfileInfo?.FullNameAr;
                }else
                {
                    if(estabCode != null)
                    {
                        var establishmentProfile = visionEstabApi.GetEstablishmentProfile(estabCode);
                        mappedProfile.SponsorNameEn = establishmentProfile.EstabNameEn;
                        mappedProfile.SponsorNameAr = establishmentProfile.EstabNameAr;
                    }               

                }
            }
            mappedProfile.UDBNumber = result?.IndividualUserInfo?.IndividualProfileInformation?.UDBNo;

            if (result?.IndividualUserInfo?.IndividualPassportInformation != null)
            {
                string issueCountry = Convert.ToString(result?.IndividualUserInfo?.IndividualPassportInformation?.IssueCountryId);
                var countryISOCode = (issueCountry != "") ? LookupHelper.GetLookupCol1ById(Constants.Lookups.CountryLookupId, issueCountry) : issueCountry;
                mappedProfile.NationalityId = countryISOCode;

                mappedProfile.NationalityEn = (issueCountry != "") ? LookupHelper.GetEn(LookupHelper.NationalityLookuptDetails,
                    result?.IndividualUserInfo?.IndividualPassportInformation.IssueCountryId.ToString()) : issueCountry;

                mappedProfile.NationalityAr = (issueCountry != "") ? LookupHelper.GetAr(LookupHelper.NationalityLookuptDetails,
                    result?.IndividualUserInfo?.IndividualPassportInformation.IssueCountryId.ToString()) : issueCountry;                

                var passportType = result?.IndividualUserInfo?.IndividualPassportInformation?.PassportTypeId?.ToString();

                mappedProfile.PassportType = (passportType != null) ? LookupHelper.GetEn(LookupHelper.PassportTypeLookupDetails,
                    result?.IndividualUserInfo?.IndividualPassportInformation.PassportTypeId.ToString()) : "";

                var gender = result?.IndividualUserInfo?.IndividualPassportInformation?.GenderId?.ToString();
                mappedProfile.GenderEn = (gender != null)?LookupHelper.GetEn(LookupHelper.
                    GetLookupItems(ConstantsUtil.LookupIds.Gender).Result, gender):"";

                mappedProfile.GenderAr = (gender != null) ? LookupHelper.GetAr(LookupHelper.
                    GetLookupItems(ConstantsUtil.LookupIds.Gender).Result,gender):"";

                mappedProfile.BirthPlaceEn = result?.IndividualUserInfo?.IndividualPassportInformation?.BirthPlaceEn;

                mappedProfile.BirthPlaceAr = result?.IndividualUserInfo?.IndividualPassportInformation?.BirthPlaceAr;

                mappedProfile.name.en = result?.IndividualUserInfo?.IndividualPassportInformation?.NameEn;

                mappedProfile.name.ar = result?.IndividualUserInfo?.IndividualPassportInformation?.NameAr;

                mappedProfile.PassportIssueDate = result?.IndividualUserInfo?.IndividualPassportInformation?.IssueDate;

                var passportIssueGov = result?.IndividualUserInfo?.IndividualPassportInformation?.IssueGovCountryId?.ToString();

                mappedProfile.PassportIssueAuthorityEn =(passportIssueGov != null)? LookupHelper.
                    GetEn(ServicesHelper.GetLookupItems(ConstantsUtil.LookupIds.Nationality).Result,passportIssueGov):"";

                mappedProfile.PassportIssueAuthorityAr = (passportIssueGov != null) ? LookupHelper.
                    GetAr(ServicesHelper.GetLookupItems(ConstantsUtil.LookupIds.Nationality).Result,passportIssueGov):"";
            }
            
            
            mappedProfile.ProfessionEn = LookupHelper.GetEn(ServicesHelper.GetLookupItems(ConstantsUtil.LookupIds.Profession).Result,
                result?.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            mappedProfile.ProfessionAr = LookupHelper.GetAr(ServicesHelper.GetLookupItems(ConstantsUtil.LookupIds.Profession).Result,
                result?.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            return Task.FromResult(mappedProfile);
        }
    }

}