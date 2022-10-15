using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Model;
using Newtonsoft.Json.Linq;
using System.ServiceModel.Web;
using System.Configuration;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Models;
using Newtonsoft.Json;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService
    {
        public async Task<RestWorkflowState> StartWorkflow(string serviceId, RestServiceInfoRequest request)
        {
            var apiKey = WebOperationContext.Current?.IncomingRequest?.Headers["apiKey"];
            var userId = ClaimUtil.GetAuthenticatedUserId();
            var userTypeId = ClaimUtil.GetAuthenticatedUserType();
            if (!string.IsNullOrEmpty(request?.ApplicationId))
            {
                await ApplicationHelper.ValidateApplication(userId, request?.ApplicationId);
            }

            if (ClaimUtil.GetAuthenticatedUserType() != Constants.UserTypeLookup.EstablishmentUserType)
            {
                await ValidateDependent(request.PermitNumber, request.ResidenceNumber);
            }

            var categories = await serviceApi.EmaratechServicesServicesServicesServiceGetCategoriesAsync();
            bool isCategory = categories.Any(c => c.CategoryId == serviceId);
            string sId, cId;
            if (isCategory)
            {
                sId = null;
                cId = serviceId;
            }
            else
            {
                sId = serviceId;
                var service = await serviceApi.EmaratechServicesServicesServicesServiceGetServiceByKeyAsync(serviceId);
                cId = service.CategoryId;
            }

            var assemblyPath = systemSettings.GetProperty<string>(ConfigurationManager.AppSettings["StepsAssemblyPathSystemProperty"]);
            var applicationWorkflowContext = await GetApplicationWorkflowContext(request?.ApplicationId, request);
            applicationWorkflowContext.ServiceId = sId;
            applicationWorkflowContext.CategoryId = cId;
            applicationWorkflowContext.ParentCategoryId = categories.FirstOrDefault(c => c.CategoryId == cId)?.ParentCategoryId;
            applicationWorkflowContext.StepsAssemblyPath = assemblyPath;
            applicationWorkflowContext.SystemId = SystemId;
            applicationWorkflowContext.UserId = userId;
            applicationWorkflowContext.ApiKey = apiKey;
            string parameters = GetInputParameters(applicationWorkflowContext);

            Log.Debug($"Searching mapping matrix with service id {serviceId}");

            var workflows = await mappingMatrixApi.SearchAsync(systemSettings.WorkflowMappingMatrix,
                new MappingMatrix.Model.SearchCriteria
                {
                    IncludeExcluded = false,
                    Values = new List<string> { sId, string.Empty, cId, request.PlatformId, userTypeId }
                });

            Log.Debug($"Workflow row count from mapping matrix is  {workflows.Count}");

            var workflowResult = workflows.SingleOrDefault();
            if (workflowResult != null)
            {
                Log.Debug($"Starting workflows asynchronously with parameters {JsonConvert.SerializeObject(parameters)}");
                var workflowId = workflowResult.Values[1];
                var contextJson = await workflowApi.StartWorkflowWithSystemIdAsync(workflowId, "-1", SystemId, new RestWorkflowParameters(parameters));
                return contextJson;
            }
            else
            {
                throw ChannelErrorCodes.ServiceNotFound.ToWebFault($"Service not found or no existing workflow for this service");
            }
        }

        private async Task<ApplicationWorkflowContext> GetApplicationWorkflowContext(string applicationId, RestServiceInfoRequest request)
        {
            ApplicationWorkflowContext workflowContext = new ApplicationWorkflowContext
            {
                AccessToken = ClaimUtil.GetAccessToken(),
                Email = ClaimUtil.GetAuthenticatedUserEmail(),
                MobileNumber = ClaimUtil.GetAuthenticatedUseMobile(),
                AuthenticatedSystemId = ClaimUtil.GetAuthenticatedSystemId(),
                UserName = ClaimUtil.GetAuthenticatedUserName(),
                UserType = ClaimUtil.GetAuthenticatedUserType(),
                SponsorType = ClaimUtil.GetAuthenticatedSponsorFileType(),
                SponsorSponsorType = ClaimUtil.GetAuthenticatedSponsorType(),
                SponsorNo = ClaimUtil.GetAuthenticatedSponsorNo(),
                EmiratesId = ClaimUtil.GetEmiratesId(),
                BirthDate = ClaimUtil.GetBirthDate(),
                PlatformId = request.PlatformId,
                EstablishmentType = ClaimUtil.GetAuthenticatedEstablishmentType(),
                VisaNumber = ClaimUtil.GetAuthenticatedVisaNumber(),
                ApplicationId = request.ApplicationId,
                LegalAdviceNumber = request.LegalAdviceNumber,
                SearchInfo = request.SearchInfo,
                JobId = ClaimUtil.GetAuthenticatedUserJobId(),
                EstablishmentCode = ClaimUtil.GetEstablishmentCode(),
                GenderId = ClaimUtil.GetAuthenticatedUserGenderId()
            };

            if (string.IsNullOrEmpty(applicationId))
            {
                workflowContext.ResidenceNumber = request.ResidenceNumber;
                workflowContext.PermitNumber = request.PermitNumber;
            }
            else
            {
                var searchCriteria = new RestApplicationSearchCriteria
                {
                    SelectColumns = new List<RestApplicationSearchKeyValues>
                      {
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicantDetails",
                            PropertyName = "*"
                        }
                    },
                    ApplicationId = applicationId
                };

                var searchResult = (await ApplicationHelper.GetApplicationSearchResult(searchCriteria))?.FirstOrDefault()?.RestApplicationSearchKeyValues;
                var visaNumberType = searchResult?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumbertype")?.Value;
                if (!string.IsNullOrEmpty(visaNumberType))
                {
                    if (int.Parse(visaNumberType) == (int)ServiceType.EntryPermit)
                        workflowContext.PermitNumber = searchResult.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;
                    else if (int.Parse(visaNumberType) == (int)ServiceType.Residence)
                        workflowContext.ResidenceNumber = searchResult.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value;
                }
            }

            return workflowContext;
        }

        public async Task<RestWorkflowState> ResumeWorkflow(JObject data)
        {
            //await AllowAnonymousAccess(data["ServiceId"].ToString());
            //var userId = GetAuthenticatedUserId();
            //if (!string.IsNullOrEmpty(userId))
            {
                var assemblyPath = systemSettings.GetProperty<string>(ConfigurationManager.AppSettings["StepsAssemblyPathSystemProperty"]);

                var workflowInstanceId = data["workflowToken"].ToString();
                data["stepsAssemblyPath"] = assemblyPath;
                data["apiKey"] = WebOperationContext.Current?.IncomingRequest?.Headers["apiKey"];
                var contextJson = await workflowApi.ResumeWorkflowAsync(workflowInstanceId, new RestWorkflowParameters(data.ToString()));
                return contextJson;
            }
        }

        #region Private methods
        private static string GetInputParameters(ApplicationWorkflowContext context)
        {
            return JsonConvert.SerializeObject(context);
        }

        private async Task ValidateDependent(string permitNo, string residenceNo)
        {
            RestIndividualSponsorshipInfo sponsorInfo = null;

            if (string.IsNullOrEmpty(permitNo) && string.IsNullOrEmpty(residenceNo))
                return;

            if (!string.IsNullOrEmpty(permitNo))
                sponsorInfo = await visionApi.GetDependentSponsorInfoByPermitNoAsync(permitNo);

            if (!string.IsNullOrEmpty(residenceNo))
                sponsorInfo = await visionApi.GetDependentSponsorInfoByResidenceNoAsync(residenceNo);

            //If the sponsor number associated with permit and residence is not equal to logged in sponsor then throw exception
            if (sponsorInfo?.SponsorshipNo != ClaimUtil.GetAuthenticatedSponsorNo())
                throw ChannelErrorCodes.BadRequest.ToWebFault($"Sponsor no {sponsorInfo?.SponsorshipNo} mismatch with logged in sponsor no {ClaimUtil.GetAuthenticatedSponsorNo()}");
        }

        private static string ExtractInputParameters(string contextJson)
        {
            // Add the required input param names in a "params" property
            // This "params" value will be used by Channel client
            JObject jobj = JObject.Parse(contextJson);
            var inputParams = jobj.Properties()
                .Where(p => p.HasValues && p.First.Type == JTokenType.Null)
                .Select(p => p.Name).ToList();

            if (inputParams.Any())
            {
                foreach (string paramToRemove in inputParams)
                {
                    jobj.Remove(paramToRemove);
                }
                jobj.Add("params", JToken.FromObject(inputParams.ToArray()));
                contextJson = jobj.ToString(Newtonsoft.Json.Formatting.None);
            }
            return contextJson;
        }


        #endregion Private methods
    }
}