using Emaratech.Services.Channels.BusinessLogic.Dashboard;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Vision.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService : IEstablishmentService
    {
        public RestIndividualUserFormInfo FetchEstablishmentUserInfo(string username)
        {
            var estabUserFileInfo = ApiFactory.Default.GetEdnrdIntegrationApi().RetrieveEstablishmentUserInfoByUserName(username);
            if (!string.IsNullOrEmpty(estabUserFileInfo.FileNo))
            {
                RestIndividualUserFormInfo individualInfo = visionApi.GetIndividualDetailedInformationBySponsorNo(estabUserFileInfo.FileNo);
                return individualInfo;
            }
            return null;
        }


        public async Task<RestEstablishmentDashboard> FetchEstablishmentDashboardSearchResult(RestEstablishmentSearchRequest searchParams)
        {
            searchParams.ApplicationStatuses = Convert.ToString(ConfigurationManager.AppSettings["StatusForDependentsInSearch"]);
            searchParams.UserId = ClaimUtil.GetAuthenticatedUserId();
            searchParams.SponsorNo = ClaimUtil.GetAuthenticatedSponsorNo();
            searchParams.ResidenceNo = ClaimUtil.GetAuthenticatedVisaNumber();

            return await SearchDependents(searchParams);
        }
        private async Task<RestEstablishmentDashboard> SearchDependents(RestEstablishmentSearchRequest searchCriteria)
        {
            searchCriteria.ApplicationModule = ApplicationModules.Dashboard;

            Log.Debug($"Going to perform establishment search with criteria {JsonConvert.SerializeObject(searchCriteria)}");

            var applicationSearchResult = await EstablishmentDashboard.GetDependentApplicationSearchResult(searchCriteria);

            var establishmentDashboard = new RestEstablishmentDashboard();

            //Get applications of this user if there is no search criteria or criteria is application
            if (string.IsNullOrEmpty(searchCriteria?.ServiceType) || searchCriteria?.ServiceType == DashboardSearchType.Application.GetAttribute<DashboardSearchTypeAttr>().Type)
            {
                establishmentDashboard.DependentsApplicationInfo = await IndividualDashboard.GetDependentsApplication(applicationSearchResult);
                Log.Debug($"Total establishment dependents applications filtered are {establishmentDashboard.DependentsApplicationInfo.Count}");
            }

            //Get visas of dependents of this user if there is no search criteria or criteria is entry permit permit or residence
            if (string.IsNullOrEmpty(searchCriteria?.ServiceType) || searchCriteria?.ServiceType == DashboardSearchType.EntryPermit.GetAttribute<DashboardSearchTypeAttr>().Type || searchCriteria?.ServiceType == DashboardSearchType.Residence.GetAttribute<DashboardSearchTypeAttr>().Type)
            {
                Log.Debug($"Going to get establishment dependents from vision for {searchCriteria.UserId}");
                var dependentsProfiles = await visionEstabApi.GetEstablishmentDependentsFilteredInformationAsync(mapper.Map<RestEstablishmentSearchCriteria>(searchCriteria));
                Log.Info("Establishment dependent information found from vision " + dependentsProfiles?.EstablishmentDependents?.Count + " for sponsor no " + searchCriteria.SponsorNo);

                if (dependentsProfiles?.EstablishmentDependents == null)
                    establishmentDashboard.DependentsVisaInfo = new List<RestEstablishmentVisaInfo>();
                else
                    establishmentDashboard.DependentsVisaInfo = await EstablishmentDashboard.GetDependentsVisas(dependentsProfiles, applicationSearchResult);

                Log.Debug($"Total establishment dependents profiles filtered are {establishmentDashboard.DependentsVisaInfo.Count}");
            }

            Log.Debug($"Establishment dependents successfully filtered for {searchCriteria.UserId}");

            return establishmentDashboard;
        }
    }
}