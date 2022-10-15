using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Vision.Model;
using log4net;
using System.Configuration;
using System.Threading.Tasks;
using Emaratech.Services.Application.Api;
using Emaratech.Services.Channels.Contracts.Errors;
using Newtonsoft.Json;
using Emaratech.Services.Channels.Contracts.Extensions;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.MappingMatrix.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Establishment;

namespace Emaratech.Services.Channels.BusinessLogic.Dashboard
{
    public class EstablishmentDashboard
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IndividualDashboard));

        public async static Task<IList<RestApplicationSearchRow>> GetDependentApplicationSearchResult(RestEstablishmentSearchRequest searchCriteria)
        {
            //If Search Criteria is Entry permit or residence then remove the search criteria for application seach
            if (searchCriteria?.ServiceType == DashboardSearchType.EntryPermit.GetAttribute<DashboardSearchTypeAttr>().Type || searchCriteria?.ServiceType == DashboardSearchType.Residence.GetAttribute<DashboardSearchTypeAttr>().Type)
                searchCriteria.Criteria = "";

            var applicationSearchApi = ApiFactory.Default.GetApplicationSearchApi();
            var restApplicationSearchCriteria = new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ApplicationId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "Created_Date"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ServiceId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "FileTypeId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "VisaTypeId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "StatusId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicantDetails",
                        PropertyName = "*"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "SponsorDetails",
                        PropertyName = "SponsorRelationId"
                    }
                },
                UserId = searchCriteria?.UserId,
                StatusId = !string.IsNullOrEmpty(searchCriteria?.StatusId) ? searchCriteria?.StatusId : searchCriteria?.ApplicationStatuses,
                IsBatchIdMarked = Convert.ToString(searchCriteria?.IsBatchIdMarked),
                IsApplicationTypeNotNull = "true",
                ApplicationId = searchCriteria?.Criteria,
                CreatedFromDate = searchCriteria?.StartDate,
                CreatedToDate = searchCriteria?.EndDate,
                PassportNumber = searchCriteria?.PassportNumber,
                EstablishmentCode = searchCriteria?.EstablishmentCode
            };
            var restApplicationSearchResults = await applicationSearchApi
                .GetApplicationsByCriteriaAsync(restApplicationSearchCriteria);

            var applicationSearchResult = restApplicationSearchResults.RestApplicationSearchRow.ToList();

            return applicationSearchResult;
        }


        public async static Task<List<RestEstablishmentVisaInfo>> GetDependentsVisas(RestEstablishmentDependentsInfo dependentsProfiles, IList<RestApplicationSearchRow> applicationSearchResult)
        {
            List<RestEstablishmentVisaInfo> dependentsVisaInfo = new List<RestEstablishmentVisaInfo>();






            //Get Sponsor Information from Vision to check whether he is investor
            RestIndividualResidenceInfo sponsorInfo = null;
            var lstResidenceNos = dependentsProfiles.EstablishmentDependents.Where(p => p.IndividualVisaInformation.VisaType == VisaType.Residence.GetAttribute<VisaTypeAttr>().Type).Select(c => c.IndividualVisaInformation.VisaNumber).ToList();
            Log.Debug($"Going to get travel info for dependents {lstResidenceNos}");
            var residentsTravelDetail = await VisionHelper.GetTravelInfoResidenceVisas(lstResidenceNos);
            Log.Debug($"Travel info received is {residentsTravelDetail}");

            //RemoveNotAllowedDependents(dependentsProfiles);

            //if (!string.IsNullOrEmpty(sponsorNo))
            //    sponsorInfo = await ApiFactory.Default.GetVisionIndividualApi().GetIndividualResidenceInfoBySponsorNoAsync(sponsorNo);

            foreach (var dependent in dependentsProfiles.EstablishmentDependents)
            {
                var applicationSearchRow = applicationSearchResult.FirstOrDefault(a => a.RestApplicationSearchKeyValues.Any(p => p.PropertyName.ToLower() == "visanumber" && p.Value == dependent.IndividualVisaInformation.VisaNumber));

                //If visa status is in allowed list of visas for both entry permit and residence
                if (applicationSearchRow == null)
                {
                    var travelDetail = IndividualDashboard.GetInsideOutside(dependent.IndividualVisaInformation, residentsTravelDetail);

                    var visaStatus = IndividualDashboard.GetDependentsVisaStatus(dependent, travelDetail);
                    //Log.Debug($"Going to get actions for dependent {dependent.IndividualVisaInformation.VisaNumber}");
                    //var actionsList = await IndividualDashboard.GetDependentsVisaActions(visaStatus, dependent, travelDetail, searchCriteria);
                    //Log.Debug($"Actions got for dependent {dependent.IndividualVisaInformation.VisaNumber}");

                    //If the sponsor is Investor then don't check for anything because he is not allowed to take any action on dashboard
                    if (sponsorInfo != null && sponsorInfo.ResidenceTypeId == (int)ResidenceType.Investor)
                    {
                        //actionsList = GetDefaultActionsList(dependent, ConstantMessageCodes.NoActionInvestorSponsor);
                        //Log.Debug($"Sponsor is investor with file number {sponsorInfo.ResidenceNo} thats why no actions for dependents");
                    }

                    dependentsVisaInfo.Add(new RestEstablishmentVisaInfo
                    {
                        VisaNumber = dependent.IndividualVisaInformation.VisaNumber,
                        ExpiryDate = dependent.IndividualVisaInformation.VisaExpiryDate,
                        VisaType = LookupHelper.GetLookupIdByVisaType(dependent.IndividualVisaInformation.VisaType),
                        FirstName =
                            new RestName()
                            {
                                En = dependent.IndividualProfileInformation.FirstNameEn,
                                Ar = dependent.IndividualProfileInformation.FirstNameAr
                            },
                        LastName =
                            new RestName()
                            {
                                Ar = dependent.IndividualProfileInformation.LastNameAr,
                                En = dependent.IndividualProfileInformation.LastNameEn
                            },
                        Nationality = Convert.ToString(dependent.IndividualProfileInformation.NationalityId),
                        //ActionsList = actionsList,
                        Relationship = Convert.ToString(dependent.IndividualVisaInformation.RelationshipId),
                        VisaTypeId = Convert.ToString(dependent.IndividualVisaInformation.VisaTypeId),
                        Status = LookupHelper.GetLookupIdByVisaStatus(visaStatus)
                    });
                }
            }






            return dependentsVisaInfo;
        }
    }
}