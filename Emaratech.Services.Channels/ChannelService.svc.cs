using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.DataAccess;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.MappingMatrix.Api;
using Emaratech.Services.MappingMatrix.Model;
//using Emaratech.Services.Registry;
using Emaratech.Services.Security.KeyVault.Api;
using Emaratech.Services.Security.KeyVault.Model;
using Emaratech.Services.Services.Api;
using Emaratech.Services.SMS.Model;
using Emaratech.Services.Systems.Model;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Api;
using SwaggerWcf.Attributes;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Document.Api;
using Emaratech.Services.Channels.Helpers;
using Newtonsoft.Json;
using AutoMapper;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Email.Api;
using Emaratech.Services.Systems.Api;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Extensions;
using Emaratech.Services.Payment.Api;
using Emaratech.Services.WcfCommons.HealthCheck;
using Emaratech.Services.Channels.Contracts.Rest.Models.HappinessMeter;
using HappinessLib;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using Emaratech.Services.Identity.Utils;
using Emaratech.Services.Services.Model;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Channels.Contracts.Authorization;
using Emaratech.Services.Channels.BusinessLogic.ApplicationReports;

namespace Emaratech.Services.Channels
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [SwaggerWcf("/")]
    public partial class ChannelService : IChannelService, IHealthCheck
    {
        private readonly IChannelDataAccessService das;
        private readonly ISystemApi systemApi;
        private readonly IMappingMatrixApi mappingMatrixApi;
        private readonly IServiceApi serviceApi;
        private readonly IWorkflowsApi workflowApi;
        private readonly ISystemSettings systemSettings;
        protected readonly IDocumentApi documentApi;
        private readonly IVisionIndividualApi visionApi;
        private readonly IVisionEstablishmentApi visionEstabApi;
        private readonly IVisionCommonApi visionCommonApi;
        private readonly ITokensApi tokensApi;
        // private readonly IRegistry registry;
        private SystemInfo UnifiedSystemInformation;
        private readonly IUserStore userStore;
        private readonly IMapper mapper;
        private readonly IEmailApi emailApi;
        private readonly IPaymentsApi paymentApi;
        private readonly ILegalAdviceApi legalAdviceApi;
        private readonly IPassportServicesApi passportServicesApi;
        private readonly ConfigurationRepository configurationRepository;
        private readonly IAuthorizationManager authorizationManager;

        public ChannelService(
            ISystemSettings systemSettings,
            IUserStore userStore,
            IMapper mapper,
            ConfigurationRepository configurationRepository,
            IServiceFactory factory,
            IAuthorizationManager authorizationManager)
        {
            this.systemSettings = systemSettings;
            this.systemApi = factory.GetSystemApi();
            this.mappingMatrixApi = factory.GetMappingMatrixApi();
            this.serviceApi = factory.GetServiceApi();
            this.workflowApi = factory.GetWorkflowsApi();
            this.documentApi = factory.GetDocumentApi();
            this.visionApi = factory.GetVisionApi();
            this.visionEstabApi = factory.GetVisionEstablishmentApi();
            this.visionCommonApi = factory.GetVisionCommonApi();
            this.tokensApi = factory.GetTokensApi();
            this.userStore = userStore;
            this.mapper = mapper;
            this.emailApi = factory.GetEmailApi();
            this.paymentApi = factory.GetPaymentApi();
            this.legalAdviceApi = factory.GetLegalAdviceApi();
            this.passportServicesApi = factory.GetPassportServicesApi();
            this.configurationRepository = configurationRepository;
            this.authorizationManager = authorizationManager;
        }

        protected ChannelService()
        {
        }

        private string SystemId => ConfigurationManager.AppSettings["SystemId"];
        public static string ApiHost => ConfigurationManager.AppSettings["ApiHost"];

        public void OptionsHandler()
        {
        }

        public async Task<IEnumerable<RestService>> GetServices()
        {
            bool isUserAnonymous = IsUserAnonymous();

            var serviceAccess = systemSettings.GetProperty<string>("Configuration.ServiceAccess");
            var platformId = await GetAuthenticatedPlatformId();
            Log.Debug($"Mapping Matrix Parameters are User Type {ClaimUtil.GetAuthenticatedUserType()} and platform {platformId}");

            var items = await mappingMatrixApi.SearchAsync(serviceAccess,
            new SearchCriteria
            {
                IncludeExcluded = false,
                Values = new List<string> { string.Empty, ClaimUtil.GetAuthenticatedUserType(), string.Empty, platformId },
                Versions = Enumerable.Repeat<long?>(null, 2).ToList()
            });

            Log.Debug($"Items found from mapping matrix are {JsonConvert.SerializeObject(items)}");

            var serviceAndCategoryIds = items.Select(x => x.Values[0] == null ? x.Values[2] : x.Values[0]).ToList();
            var services = (await serviceApi.GetSystemServicesAsync(new Emaratech.Services.Services.Model.Systems { SystemIds = new List<string> { systemSettings.SystemId } }, "1", short.MaxValue.ToString())).Data;
            List<Category> categories;

            if (isUserAnonymous) //If anonymous user then no need to fetch category as it also fetches entry permit new
                categories = new List<Category>();
            else
                categories = await serviceApi.EmaratechServicesServicesServicesServiceGetCategoriesAsync();

            var availableServices = services.Where(x => serviceAndCategoryIds.Contains(x.ServiceId)).Select(x => new RestService
            {
                Id = x.ServiceId,
                ResourceKey = x.ResourceKey,
                CategoryId = x.CategoryInfo?.Parent?.Id,
                CategoryResourceKey = x.CategoryInfo?.Parent?.ResourceKey,
                SubCategoryId = x.CategoryInfo?.Id,
                SubCategoryResourceKey = x.CategoryInfo?.ResourceKey
            });

            Log.Debug($"Available Services {JsonConvert.SerializeObject(availableServices)}");

            var availableCategories = categories.Where(x => serviceAndCategoryIds.Contains(x.CategoryId)).Select(x => new RestService
            {
                Id = x.CategoryId,
                ResourceKey = x.ResourceKey,
                CategoryId = x.ParentCategoryId,
                SubCategoryResourceKey = x.ResourceKey
            });

            Log.Debug($"Available Categories {JsonConvert.SerializeObject(availableCategories)}");

            Log.Debug($"Final services are {JsonConvert.SerializeObject(availableServices.Concat(availableCategories).ToList())}");
            return availableServices.Concat(availableCategories);//.Where(x => x.CategoryId != ConfigurationManager.AppSettings["5B166CC8029540B09B022C6E0D8D535E"]);
        }

        public async Task<IEnumerable<RestService>> GetAllServices()
        {
            var services = (await serviceApi.GetSystemServicesAsync(new Emaratech.Services.Services.Model.Systems { SystemIds = new List<string> { systemSettings.SystemId } }, "1", short.MaxValue.ToString())).Data;

            var allServices = services.Select(x => new RestService
            {
                Id = x.ServiceId,
                ResourceKey = x.ResourceKey,
                CategoryId = x.CategoryInfo?.Parent?.Id,
                CategoryResourceKey = x.CategoryInfo?.Parent?.ResourceKey,
                SubCategoryId = x.CategoryInfo?.Id,
                SubCategoryResourceKey = x.CategoryInfo?.ResourceKey
            }).ToList();

            return allServices;
        }

        private async Task<string> GetAuthenticatedPlatformId()
        {
            var props = await systemApi.GetPropertyByNameAsync(ClaimUtil.GetAuthenticatedSystemId(), "PlatformId");
            return props.FirstOrDefault()?.PropValue;
        }


        public async Task<string> SendOtpCode(VerificationInitRequest request)
        {
            return await SendOtpCode(request.MobileNumber, new Dictionary<object, object>() { { Constants.Mobile, request.MobileNumber } });
        }

        private async Task<string> SendOtpCode(string mobileNo, Dictionary<object, object> nameValuePairs)
        {
            await ApiFactory.Default.GetSmsApi()
                .EmaratechServicesSmsSmsServiceValidateAsync(
                    new EmaratechServicesSmsContractsRestModelRestVerificationInput()
                    {
                        MobileNo = mobileNo,
                        SystemId = SystemId
                    });

            var token = await PayloadBuilder.New()
                .Key(SystemId, TokenUtils.TokenKey(SystemId))
                .Expiry(TimeSpan.FromHours(1))
                .Add(nameValuePairs, x => x.Key, x => x.Value)
                .Issue(tokensApi);

            return token;
        }


        public async Task<string> VerifyOtpCode(RestOtpVerificationRequest request)
        {
            var tokenData = await TokenUtils.VerifyTokenInContext(SystemId, request.OtpToken);
            var mobileNo = tokenData.Single(x => x.Name == Constants.Mobile).Value;

            await VerifyOtpCodeAgainstRemoteService(request, mobileNo, SystemId);

            var token = await PayloadBuilder.New()
                .Key(SystemId, TokenUtils.TokenKey(SystemId))
                .Expiry(TimeSpan.FromHours(1))
                .Add(Constants.Mobile, mobileNo)
                .Issue(tokensApi);

            return token;
        }

        private async Task VerifyOtpCodeAgainstRemoteService(RestOtpVerificationRequest request, string mobileNo, string argSystemId)
        {
            await ApiFactory
                .Default.GetSmsApi()
                .EmaratechServicesSmsSmsServiceValidateContactAsync(request.otpCode,
                    new EmaratechServicesSmsContractsRestModelRestVerificationRequest()
                    {
                        MobileNo = mobileNo,
                        SystemId = argSystemId
                    });
        }


        private static void ValidateUser(RestIndividualInfo individualInformation, bool validateMobileNumber = true)
        {
            // Check if user is found
            if (individualInformation?.IndividualProfileInformation == null)
                throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"User profile data not found");

            if (individualInformation.IndividualProfileInformation?.LegalityStatusId == null)
                throw ChannelErrorCodes.IndividualUserNotSupported.ToWebFault($"Legality status not found of user");

            //if User is GCC National
            //if (individualInformation.IndividualProfileInformation?.LegalityStatusId == (int)LegalityStatusType.GCCLocal)
            //    throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"User is GCC National");

            //Check if sponsorship file type is not null
            if (individualInformation?.IndividualSponsorshipInformation?.SponsorshipFileTypeId == null)
                throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"User sponsor file type id not found");

            //Check if the sponsor department is dubai department
            if (!(Convert.ToString(individualInformation?.IndividualSponsorshipInformation?.SponsorshipNo).StartsWith(Constants.DubaiDepartmentStart.ToString())))
                throw ChannelErrorCodes.IndividualUserDepartmentNotSupported.ToWebFault($"Department is not a dubai department");


            //Check if file is valid and passport no is not empty
            //if (individualInformation.IndividualProfileInformation?.LegalityStatusId == null ||
            //    individualInformation.IndividualProfileInformation?.LegalityStatusId !=
            //    (int)LegalityStatusType.Citizen)
            //{
            //    if (string.IsNullOrEmpty(individualInformation.IndividualPassportInformation.PassportNumber))
            //        throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"Sponsor no is not active or passport number is empty")
            //    ;
            //}


            if (individualInformation.IndividualProfileInformation?.LegalityStatusId == (int)LegalityStatusType.Resident)
            {
                //If residence number or expiry is empty then return error
                if (individualInformation.IndividualResidenceInfo == null ||
                    string.IsNullOrEmpty(individualInformation.IndividualResidenceInfo?.ResidenceNo) ||
                    string.IsNullOrEmpty(Convert.ToString(individualInformation.IndividualResidenceInfo?.ResidenceExpiryDate)))
                {
                    Log.Debug($"Residence No is {individualInformation.IndividualResidenceInfo.ResidenceNo} and expiry is {Convert.ToString(individualInformation?.IndividualResidenceInfo?.ResidenceExpiryDate)}");
                    throw ChannelErrorCodes.IndividualUserResInfoNotFound.ToWebFault($"Residence number or expiry is empty");

                }

                // Check if user is Partner
                if (individualInformation?.IndividualResidenceInfo?.ResidenceTypeId == (int)ResidenceType.Partner)
                    throw ChannelErrorCodes.IndividualUserPartner.ToWebFault($"Resident Type partner is not allowed to register");

                //If resident department is not dubai department
                //if (!Convert.ToString(individualInformation.IndividualResidenceInfo.EmiratesDepartmentId).StartsWith(Constants.DubaiDepartmentStart.ToString()))
                //    throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"Department is not a dubai department");

                //if (string.IsNullOrEmpty(Convert.ToString(individualInformation?.IndividualResidenceInfo?.SponsorTypeId)))
                //    throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"User sponsor type id not found");

                //if (DateTime.Now > (Convert.ToDateTime(individualInformation.IndividualPassportInformation.ExpiryDate)))
                //    throw ChannelErrorCodes.IndividualUserNotFound.ToWebFault($"Passport is expired");

                if (DateTime.Now > (Convert.ToDateTime(individualInformation?.IndividualResidenceInfo?.ValidityDate)))
                    throw ChannelErrorCodes.IndividualUserVisaExpired.ToWebFault($"User visa has expired");
            }
            else if (individualInformation.IndividualProfileInformation?.LegalityStatusId == (int)LegalityStatusType.Citizen)
            {
                //If User selected as citizen but passport is not from UAE
                if (individualInformation.IndividualPassportInformation != null && individualInformation.IndividualPassportInformation.IssueCountryId != Constants.UaeCountryId)
                    throw ChannelErrorCodes.IndividualLocalUserPassportNotFromUae.ToWebFault($"User passport country id is not UAE");
            }
            else
                throw ChannelErrorCodes.IndividualUserNotSupported.ToWebFault($"User Legality Status not supported {individualInformation.IndividualProfileInformation?.LegalityStatusId}");

            // If mobile number not found
            if (validateMobileNumber)
            {
                if (individualInformation.IndividualContactDetails == null ||
                    individualInformation.IndividualContactDetails.Count == 0 ||
                    !individualInformation.IndividualContactDetails.Any(x => x.CONTACTDETAIL.StartsWith("05")))
                    throw ChannelErrorCodes.IndividualUserMobileNotFound.ToWebFault($"No mobile number found for the requested profile")
                ;
            }
        }

        private static T GetResponse<T>(IDictionary<string, object> values)
        {
            var response = (T)Activator.CreateInstance(typeof(T));
            var properties = response.GetType().GetProperties();
            foreach (var property in properties)
            {
                object value;
                values.TryGetValue(property.Name, out value);
                if (value != null)
                {
                    if (property.PropertyType == typeof(IEnumerable<RestDocumentInfo>))
                    {
                        var docs = new List<RestDocumentInfo>();
                        JArray obj = value as JArray;
                        foreach (var child in obj.Children())
                        {
                            bool required = false;
                            bool.TryParse(child["required"]?.ToString(), out required);
                            docs.Add(new RestDocumentInfo
                            {
                                DocumentTypeId = child["documentTypeId"]?.ToString(),
                                DocumentId = child["documentId"]?.ToString(),
                                MimeType = child["mimeType"]?.ToString(),
                                Size = Convert.ToInt32(child["maxSize"]),
                                ResourceKey = child["resourceKey"]?.ToString(),
                                Required = required,
                                Name = child["name"]?.ToString()
                            });
                        }

                        property.SetValue(response, docs);
                    }
                    else if (property.PropertyType == typeof(IEnumerable<RestFeeInfo>))
                    {
                        var fees = new List<RestFeeInfo>();
                        JArray obj = value as JArray;
                        foreach (var child in obj.Children())
                        {
                            fees.Add(new RestFeeInfo
                            {
                                Amount = Convert.ToUInt64(child["amount"]),
                                FeeTypeId = child["feeTypeId"]?.ToString(),
                                ResourceKey = child["resourceKey"]?.ToString()
                            });
                        }

                        property.SetValue(response, fees);
                    }
                    else if (property.PropertyType == typeof(IEnumerable<RestDependentVisaInfo>))
                    {
                        var dependents = new List<RestDependentVisaInfo>();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RestDependentVisaInfo>>(value.ToJObject().SelectToken("Result").ToString());
                        if (result != null)
                        {
                            foreach (var child in result)
                            {
                                dependents.Add(new RestDependentVisaInfo
                                {
                                    VisaType = child.VisaType,
                                    VisaTypeId = child.VisaTypeId,
                                    VisaNumber = child.VisaNumber,
                                    ExpiryDate = child.ExpiryDate,
                                    Nationality = child.Nationality,
                                    Relationship = child.Relationship,

                                    FirstName = new RestName
                                    {
                                        En = child.FirstName?.En,
                                        Ar = child.FirstName?.Ar
                                    },

                                    LastName = new RestName
                                    {
                                        En = child.LastName?.En,
                                        Ar = child.LastName?.Ar
                                    }
                                });
                            }
                        }

                        property.SetValue(response, dependents);
                    }
                    else if (property.PropertyType == typeof(IEnumerable<RestListData>))
                    {
                        JArray array = value as JArray;
                        property.SetValue(response, array.ToObject<IEnumerable<RestListData>>());
                    }
                    else if (property.PropertyType == typeof(IEnumerable<RestButton>))
                    {
                        JArray array = value as JArray;
                        property.SetValue(response, array.ToObject<IEnumerable<RestButton>>());
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        if (value is string)
                        {
                            property.SetValue(response, value.ToString());
                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(value, Formatting.None);
                            property.SetValue(response, json);
                        }
                    }
                    else
                    {
                        property.SetValue(response, value);
                    }
                }
            }
            return response;
        }

        public HealthcheckResponse PerformHealthcheck()
        {
            return new HealthcheckResponse()
            {
                HealthcheckSucceeded = true
            };
        }

        public System.IO.Stream GetHappinessMeter(string lang, string themeColor)
        {
            //if(string.Equals(request.Type, "transaction", StringComparison.OrdinalIgnoreCase))
            //    throw new NotImplementedException();

            var beatValues = Happiness.GetNonTransactionalBeats(
                themeColor,
                ConfigurationManager.AppSettings["serviceProvider"].ToString(),
                ConfigurationManager.AppSettings["applicationID"].ToString(),
                ConfigurationManager.AppSettings["Applicationtype"].ToString(),
                ConfigurationManager.AppSettings["platform"].ToString(),
                ConfigurationManager.AppSettings["Applicationurl"].ToString(),
                ConfigurationManager.AppSettings["version"].ToString(),
                ConfigurationManager.AppSettings["source"].ToString(),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                ConfigurationManager.AppSettings["SecretKey"].ToString(),
                lang);

            var response = new HappinessMeterResponse
            {
                PostURL = ConfigurationManager.AppSettings["PostURL"].ToString(),
                ClientId = System.Configuration.ConfigurationManager.AppSettings["client_id"],
                JsonRequest = beatValues.json_payload,
                Lang = beatValues.lang,
                Nonce = beatValues.nonce,
                Random = beatValues.random,
                Signature = beatValues.signature,
                Timestamp = beatValues.timestamp
            };

            var htmlResponseBuilder = new StringBuilder();

            htmlResponseBuilder.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            htmlResponseBuilder.Append("<head runat=\"server\">");
            htmlResponseBuilder.Append("<title></title>");
            htmlResponseBuilder.Append("</head>");
            htmlResponseBuilder.Append("<body>");
            htmlResponseBuilder.Append($"<form action=\"{response.PostURL}\" method=\"post\">");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"json_payload\" value=\"{response.JsonRequest}\" />");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"client_id\" value=\"{response.ClientId}\"/>");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"signature\" value=\"{response.Signature}\"/>");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"lang\" value=\"{response.Lang}\"/>");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"timestamp\" value=\"{response.Timestamp}\"/>");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"random\" value=\"{response.Random}\"/>");
            htmlResponseBuilder.Append($"<input type=\"hidden\" name=\"nonce\" value=\"{response.Nonce}\"/>");
            htmlResponseBuilder.Append("</form>");
            htmlResponseBuilder.Append("</body>");
            htmlResponseBuilder.Append("<script type=\"text/javascript\">");
            htmlResponseBuilder.Append("document.forms[0].submit();");
            htmlResponseBuilder.Append("</script>");
            htmlResponseBuilder.Append("</html>");

            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return new MemoryStream(Encoding.UTF8.GetBytes(htmlResponseBuilder.ToString()));
        }

        private bool IsUserAnonymous()
        {
            try
            {
                bool isAnonymous = false;
                if (!ClaimUtil.IsLoggedIn())
                {
                    ClaimUtil.ImpersonateAnonymousUser();
                    isAnonymous = true;
                }
                return isAnonymous;
            }
            catch (Exception exp)
            {
                throw;
            }
        }
    }
}