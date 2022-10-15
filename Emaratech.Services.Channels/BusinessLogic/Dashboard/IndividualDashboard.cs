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

namespace Emaratech.Services.Channels.BusinessLogic.Dashboard
{
    public class IndividualDashboard
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IndividualDashboard));

        public async static Task<IList<RestApplicationSearchRow>> GetDependentApplicationSearchResult(RestDashboardSearchRequest searchCriteria)
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

        public static VisaStatus GetDependentsVisaStatus(RestIndividualDependentsInfoWrapper dependent, RestTravelInfo travelInfo)
        {
            RestDependentDashboardStatus dependentStatus = new RestDependentDashboardStatus();

            VisaStatus? enumVisaStatus = null;

            //Rules for entry permit and residence in story TEAP-1166
            if (dependent.IndividualVisaInformation.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type)
                enumVisaStatus = GetStatusForEntryPermit(dependent);
            else
                enumVisaStatus = GetStatusForResidence(dependent, travelInfo);


            ////If EP/Res cancel then for 30 days of cancel date it will be in expiring soon and after 30 days move to expire
            //if ((dependent.IndividualVisaInformation.VisaType == VisaType.Residence.GetAttribute<VisaTypeAttr>().Type && dependent.IndividualVisaInformation.VisaStatusId == (int)ResidenceStatusType.Cancelled) || (dependent.IndividualVisaInformation.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type && dependent.IndividualVisaInformation.VisaStatusId == (int)PermitStatusType.CancelledAfterEntry) || (dependent.IndividualVisaInformation.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type && dependent.IndividualVisaInformation.VisaStatusId == (int)PermitStatusType.CancelledBeforeEntry))
            //{
            //    var daysDifference = (DateTime.Now - Convert.ToDateTime(dependent.IndividualVisaInformation.CancelDate)).TotalDays;

            //    if (daysDifference <= 30)
            //        eVisaStatus = VisaStatus.ExpiringSoon;
            //    else
            //        eVisaStatus = VisaStatus.Expired;

            //    Log.Debug($"Dependent with visa number {dependent.IndividualVisaInformation.VisaNumber} has cancelled visa thats why no actions");
            //}

            if (enumVisaStatus == null)
            {
                Log.Debug($"Visa status not found for {dependent.IndividualVisaInformation.VisaNumber} thats why marked as Expired");
                enumVisaStatus = VisaStatus.Expired;
            }

            return (VisaStatus)enumVisaStatus;

        }

        private async static Task<IList<RestDependentActionsList>> GetDependentsApplicationActions(RestApplicationActionsCriteria searchCriteria)
        {
            Log.Debug("Going to get dependent application actions");

            List<RestDependentActionsList> actionsList = new List<RestDependentActionsList>();

            var items = await ApplicationHelper.GetApplicationActions(searchCriteria);

            if (items != null && items.Count > 0)
                actionsList.AddRange(items.Select(p => new RestDependentActionsList { Action = p.Action, ServiceId = searchCriteria.ServiceId, Status = true }));

            return actionsList; ;
        }

        private async static Task<List<RestDependentActionsList>> GetDependentsVisaActions(VisaStatus? visaStatus, RestIndividualDependentsInfoWrapper dependent, RestTravelInfo travelInfo, RestDashboardSearchRequest dashboardSearchCriteria)
        {
            Log.Debug($"Going to get dependent visa actions for visa number {dependent.IndividualVisaInformation.VisaNumber}");

            List<RestDependentActionsList> actionsList = GetDefaultActionsList(dependent);

            var criteria = new List<SearchVersion>
            {
                new SearchVersion("DashboardStatus", visaStatus.ToString()),
                new SearchVersion("ServiceType",  dependent.IndividualVisaInformation.VisaType),
                new SearchVersion("VisaType", Convert.ToString(dependent.IndividualVisaInformation.VisaTypeId)),
                new SearchVersion("Service", dashboardSearchCriteria?.ServiceId),
                new SearchVersion("Platform", dashboardSearchCriteria?.Platform),
                new SearchVersion("ApplicationModule", dashboardSearchCriteria?.ApplicationModule.ToString()),
            };

            Log.Debug($"Mapping Matrix input parameters are {JsonConvert.SerializeObject(criteria)}");

            var searchCriteria = new SearchCriteria
            {
                IncludeExcluded = false,
                ValuesDict = criteria
            };

            var items = await ApiFactory.Default.GetMappingMatrixApi().SearchAsync(ConfigurationManager.AppSettings["MappingMatrixDashboardVisaAction"], searchCriteria);
            Log.Debug($"Items found from mapping matrix are {JsonConvert.SerializeObject(items)}");
            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    actionsList.FirstOrDefault(p => p.Action == item.Values[4]).ServiceId = item.Values[3];
                    actionsList.FirstOrDefault(p => p.Action == item.Values[4]).Status = true;
                    actionsList.FirstOrDefault(p => p.Action == item.Values[4]).ErrorCode = string.Empty;
                    actionsList.FirstOrDefault(p => p.Action == item.Values[4]).IsInside = item.Values[5];
                }
            }

            Log.Debug($"Actions found from mapping matrix after filteration are {JsonConvert.SerializeObject(actionsList)}");

            actionsList = ValidateGeneralRulesForDependentsActions(actionsList, dependent, travelInfo);

            Log.Debug($"Actions filtered are {JsonConvert.SerializeObject(actionsList)}");

            return actionsList;
        }

        private static List<RestDependentActionsList> ValidateGeneralRulesForDependentsActions(List<RestDependentActionsList> actionsList, RestIndividualDependentsInfoWrapper dependent, RestTravelInfo travelInfo)
        {
            //If dependent has dependents then don't allow to take any action
            if (dependent.IndividualVisaInformation.NumberOfDependents > 0)
            {
                actionsList = RemoveDashboardAction(actionsList, VisaActionsType.Cancel.ToString(), ConstantMessageCodes.NoCancelDependentsExist);
                Log.Debug($"Dependent with visa number {dependent.IndividualVisaInformation.VisaNumber} has {Convert.ToString(dependent.IndividualVisaInformation.NumberOfDependents)} dependents thats why cannot cancel");
            }

            //If dependent has accompanies then don't allow to take any action
            if (dependent.IndividualVisaInformation.NumberOfAccompanied > 0)
            {
                actionsList = GetDefaultActionsList(dependent, ConstantMessageCodes.NoActionAllowedAccompaniedExist);
                Log.Debug($"Dependent with visa number {dependent.IndividualVisaInformation.VisaNumber} has {Convert.ToString(dependent.IndividualVisaInformation.NumberOfAccompanied)} accompanies thats why no actions");
            }

            if (dependent.IndividualVisaInformation.VisaType == VisaType.Residence.GetAttribute<VisaTypeAttr>().Type)
            {
                //if dependent has more than 6 months in residence expiry then don't give option to renew
                var daysDifference = (Convert.ToDateTime(dependent.IndividualVisaInformation.VisaExpiryDate) - DateTime.Now).TotalDays;

                if (daysDifference >= 180)
                {
                    actionsList = RemoveDashboardAction(actionsList, VisaActionsType.Renew.ToString(), ConstantMessageCodes.NoRenewLessThan180Days);
                    Log.Debug($"Dependent with visa number {dependent.IndividualVisaInformation.VisaNumber} is not allowed to renew because days difference in expiry and current date is {daysDifference}");
                }

                //Do not allow cancel if the person is outside country less than 180 days
                if (travelInfo != null && travelInfo.TravelType == TravelType.Exit)
                {
                    var daysOutside = (DateTime.Now - Convert.ToDateTime(travelInfo.TravelDate)).TotalDays;
                    if (daysOutside < 180)
                        if (actionsList != null && actionsList.Any(p => p.Action == VisaActionsType.Cancel.ToString()))
                            actionsList = RemoveDashboardAction(actionsList, VisaActionsType.Cancel.ToString(), ConstantMessageCodes.NoCancelLessThan180DaysOutside);
                }
            }

            //If dependent visa is not of dubai
            if (!dependent.IndividualVisaInformation.VisaNumber.StartsWith("2"))
            {
                actionsList = GetDefaultActionsList(dependent, ConstantMessageCodes.NoActionNotDubaiVisa);
                Log.Debug($"Dependent with visa number {dependent.IndividualVisaInformation.VisaNumber} visa is not from dubai thats why no actions.");
            }

            //Check for Inside/Outside and add appropriate error message (Previously it was handling in mapping matrix but changing it to add proper error messsage)
            foreach (var action in actionsList)
            {
                if (!string.IsNullOrEmpty(action.IsInside) && action.IsInside != Convert.ToString((int)travelInfo?.TravelType))
                {
                    action.Status = false;
                    action.ErrorCode = ConstantMessageCodes.NotAllowedPersonOutSideCountry;
                }
            }

            return actionsList;
        }

        private static void RemoveNotAllowedDependents(RestIndividualDependentsInfo dependents)
        {
            string dependentPermitAllowedStatuses = ConfigurationManager.AppSettings["DependentsPermitStatusAllowed"];
            string dependentResidenceAllowedStatuses = ConfigurationManager.AppSettings["DependentsResidenceStatusAllowed"];
            string allowedVisaTypes = ConfigurationManager.AppSettings["AllowedVisaTypesForDashboard"];

            foreach (var dependent in dependents.IndividualDependents.ToArray())
            {

                if (!allowedVisaTypes.Split(',').Any(p => p == Convert.ToString(dependent.IndividualVisaInformation.VisaTypeId)))
                {
                    dependents.IndividualDependents.Remove(dependent);
                    continue;
                }

                if (dependent.IndividualVisaInformation.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type)
                {
                    if (!dependentPermitAllowedStatuses.Split(',').Any(p => p == Convert.ToString(dependent.IndividualVisaInformation.VisaStatusId)))
                        dependents.IndividualDependents.Remove(dependent);
                    continue;
                }

                if (dependent.IndividualVisaInformation.VisaType == VisaType.Residence.GetAttribute<VisaTypeAttr>().Type)
                {
                    if (!dependentResidenceAllowedStatuses.Split(',').Any(p => p == Convert.ToString(dependent.IndividualVisaInformation.VisaStatusId)))
                        dependents.IndividualDependents.Remove(dependent);
                }

            }
        }

        //public static bool IsDependentValidated(RestIndividualDependentsInfoWrapper dependent)
        //{
        //    bool isDependentValid = (dependent.IndividualVisaInformation.VisaType ==
        //                             VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type &&
        //                             ConfigurationManager.AppSettings["DependentsPermitStatusAllowed"].Split(',').Any(p => p ==
        //                                         Convert.ToString(dependent.IndividualVisaInformation.VisaStatusId)))
        //                            ||
        //                            (dependent.IndividualVisaInformation.VisaType ==
        //                             VisaType.Residence.GetAttribute<VisaTypeAttr>().Type &&
        //                             ConfigurationManager.AppSettings["DependentsResidenceStatusAllowed"].Split(',').Any(p => p ==
        //                                         Convert.ToString(dependent.IndividualVisaInformation.VisaStatusId)));

        //    if (isDependentValid)
        //        isDependentValid = ConfigurationManager.AppSettings["AllowedVisaTypesForDashboard"].Split(',').Any(p => p ==
        //                                        Convert.ToString(dependent.IndividualVisaInformation.VisaTypeId));

        //    Log.Debug($"Dependent with number {dependent.IndividualVisaInformation.VisaNumber} validity is {isDependentValid.ToString()} as dependet visa type is {dependent.IndividualVisaInformation.VisaType.ToString()} and visa status is { Convert.ToString(dependent.IndividualVisaInformation.VisaStatusId)}");

        //    return isDependentValid;
        //}

        //private static VisaStatus? GetStatus(RestIndividualDependentsInfoWrapper dependent)
        //{
        //    //var lookupApi = ApiFactory.Default.GetLookupApi();

        //    //If visa expiry date has passed then expire the visa
        //    //if (DateTime.Now >= dependent.IndividualVisaInformation.VisaExpiryDate)
        //    //    return VisaStatus.Expired;

        //    VisaStatus? enumVisaStatus = null;

        //    //Rules for entry permit and residence in story TEAP-1166
        //    if (dependent.IndividualVisaInformation.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type)
        //        enumVisaStatus = GetStatusForEntryPermit(dependent);
        //    else
        //        enumVisaStatus = GetStatusForResidence(dependent);

        //    //If not found result from rules then execute the general rules in story TEAP-939
        //    //if (enumVisaStatus == null)
        //    //    enumVisaStatus = GetStatusFromGeneralRules(dependent);

        //    return enumVisaStatus;
        //}

        private static VisaStatus? GetStatusForEntryPermit(RestIndividualDependentsInfoWrapper dependent)
        {
            VisaStatus? enumVisaStatus = null;

            if (DateTime.Now > dependent.IndividualVisaInformation.ValidityDate)
                enumVisaStatus = VisaStatus.Expired;

            //If the person has entered the country
            if (dependent.IndividualVisaInformation.VisaStatusId == (int)PermitStatusType.Used)
            {
                if (DateTime.Now <= dependent.IndividualVisaInformation.ValidityDate)
                    enumVisaStatus = VisaStatus.ExpiringSoon;
            }
            else
            {
                if (DateTime.Now <= dependent.IndividualVisaInformation.ValidityDate)
                    enumVisaStatus = VisaStatus.Active;
            }

            return enumVisaStatus;
        }

        private static VisaStatus? GetStatusForResidence(RestIndividualDependentsInfoWrapper dependent, RestTravelInfo travelInfo)
        {
            //For Residence, the file will move from "Active" to "Expiring Soon" 30 days before Expiry Date.
            VisaStatus? enumVisaStatus = null;

            if (DateTime.Now < dependent.IndividualVisaInformation.VisaExpiryDate)
                enumVisaStatus = VisaStatus.Active;

            else if (DateTime.Now >= dependent.IndividualVisaInformation.VisaExpiryDate && DateTime.Now <= dependent.IndividualVisaInformation.ValidityDate)
                enumVisaStatus = VisaStatus.ExpiringSoon;

            else
                enumVisaStatus = VisaStatus.Expired;

            //If person outside country more than 6 months then move to expire
            if (travelInfo.TravelType == TravelType.Exit && travelInfo.TravelDate != null)
            {
                if ((DateTime.Now - Convert.ToDateTime(travelInfo.TravelDate)).TotalDays > 180)
                {
                    Log.Debug($"Dependent with residence no {dependent.IndividualVisaInformation.VisaNumber} because more than 6 months out of country as travel date is {Convert.ToString(travelInfo.TravelDate)}");
                    enumVisaStatus = VisaStatus.Expired;
                }
            }

            return enumVisaStatus;
        }

        public async static Task<List<RestDependentApplicationInfo>> GetDependentsApplication(IList<RestApplicationSearchRow> applicationSearchResult)
        {
            List<RestDependentApplicationInfo> dependentsApplicationInfo = new List<RestDependentApplicationInfo>();

            foreach (var application in applicationSearchResult)
            {
                dependentsApplicationInfo.Add(new RestDependentApplicationInfo
                {
                    ApplicationNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value,
                    ApplicationStatus = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "mapped_statusid")?.Value,
                    Status = LookupHelper.GetLookupIdByVisaStatus(VisaStatus.InProgress),
                    CreatedDate = Convert.ToDateTime(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "created_date")?.Value),
                    VisaType = LookupHelper.GetLookupIdByVisaTypeCol2(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "filetypeid")?.Value),
                    FirstName =
                    new RestName()
                    {
                        En = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "firstnamee")?.Value,
                        Ar = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "firstnamea")?.Value
                    },
                    LastName =
                    new RestName()
                    {
                        En = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "lastnamee")?.Value,
                        Ar = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "lastnamea")?.Value,
                    },
                    Nationality = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "currentnationalityid")?.Value,
                    Relationship = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "sponsorrelationid")?.Value,
                    VisaTypeId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visatypeid")?.Value,
                    ActionsList = await GetDependentsApplicationActions(new RestApplicationActionsCriteria
                    {
                        ApplicationStatus = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "statusid")?.Value,
                        ServiceId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "serviceid")?.Value,
                        ApplicationModule = ApplicationModules.Default
                    }),
                    VisaNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value,
                    VisaNumberType = LookupHelper.GetLookupIdByVisaTypeCol2(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumbertype")?.Value)
                });
            }
            return dependentsApplicationInfo;
        }

        public async static Task<List<RestDependentVisaInfo>> GetDependentsVisas(string sponsorNo, IList<RestApplicationSearchRow> applicationSearchResult, RestIndividualDependentsInfo dependentsProfiles, RestDashboardSearchRequest searchCriteria)
        {
            List<RestDependentVisaInfo> dependentsVisaInfo = new List<RestDependentVisaInfo>();

            //Get Sponsor Information from Vision to check whether he is investor
            RestIndividualResidenceInfo sponsorInfo = null;
            var lstResidenceNos = dependentsProfiles.IndividualDependents.Where(p => p.IndividualVisaInformation.VisaType == VisaType.Residence.GetAttribute<VisaTypeAttr>().Type).Select(c => c.IndividualVisaInformation.VisaNumber).ToList();
            Log.Debug($"Going to get travel info for dependents {JsonConvert.SerializeObject(lstResidenceNos)}");
            var residentsTravelDetail = await VisionHelper.GetTravelInfoResidenceVisas(lstResidenceNos);
            Log.Debug($"Travel info received is {JsonConvert.SerializeObject(residentsTravelDetail)}");

            RemoveNotAllowedDependents(dependentsProfiles);

            if (!string.IsNullOrEmpty(sponsorNo))
                sponsorInfo = await ApiFactory.Default.GetVisionIndividualApi().GetIndividualResidenceInfoBySponsorNoAsync(sponsorNo);

            foreach (var dependent in dependentsProfiles.IndividualDependents)
            {
                var applicationSearchRow = applicationSearchResult.FirstOrDefault(a => a.RestApplicationSearchKeyValues.Any(p => p.PropertyName.ToLower() == "visanumber" && p.Value == dependent.IndividualVisaInformation.VisaNumber));

                //If visa status is in allowed list of visas for both entry permit and residence
                if (applicationSearchRow == null)
                {
                    var travelDetail = GetInsideOutside(dependent.IndividualVisaInformation, residentsTravelDetail);

                    var visaStatus = GetDependentsVisaStatus(dependent, travelDetail);
                    Log.Debug($"Going to get actions for dependent {dependent.IndividualVisaInformation.VisaNumber}");
                    var actionsList = await GetDependentsVisaActions(visaStatus, dependent, travelDetail, searchCriteria);
                    Log.Debug($"Actions got for dependent {dependent.IndividualVisaInformation.VisaNumber}");

                    //If the sponsor is Investor then don't check for anything because he is not allowed to take any action on dashboard
                    if (sponsorInfo != null && sponsorInfo.ResidenceTypeId == (int)ResidenceType.Investor)
                    {
                        actionsList = GetDefaultActionsList(dependent, ConstantMessageCodes.NoActionInvestorSponsor);
                        Log.Debug($"Sponsor is investor with file number {sponsorInfo.ResidenceNo} thats why no actions for dependents");
                    }

                    dependentsVisaInfo.Add(new RestDependentVisaInfo
                    {
                        VisaNumber = dependent.IndividualVisaInformation.VisaNumber,
                        ExpiryDate = dependent.IndividualVisaInformation.VisaExpiryDate,
                        ValidityDate = dependent.IndividualVisaInformation.ValidityDate,
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
                        ActionsList = actionsList,
                        Relationship = Convert.ToString(dependent.IndividualVisaInformation.RelationshipId),
                        VisaTypeId = Convert.ToString(dependent.IndividualVisaInformation.VisaTypeId),
                        Status = LookupHelper.GetLookupIdByVisaStatus(visaStatus)
                    });
                }
            }

            return dependentsVisaInfo;
        }

        public static RestTravelInfo GetInsideOutside(RestIndividualVisaInfo dependentsVisaInfo, IList<RestIndividualTravelInfo> travelInfo)
        {
            var individualTravelInfo = new RestTravelInfo();

            if (dependentsVisaInfo.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type)
            {
                if (dependentsVisaInfo.VisaStatusId == (int)PermitStatusType.Used)
                    individualTravelInfo.TravelType = TravelType.Entry;
                else
                    individualTravelInfo.TravelType = TravelType.Exit;
            }
            else
            {
                var travelStatus = travelInfo?.FirstOrDefault(p => p.ResidenceNo == dependentsVisaInfo.VisaNumber);
                if (travelStatus == null)
                    individualTravelInfo.TravelType = TravelType.Entry;
                else
                {
                    individualTravelInfo.TravelType = (TravelType)Enum.Parse(typeof(TravelType), travelStatus.TravelTypeId);
                    individualTravelInfo.TravelDate = travelStatus.TravelDate;
                }
            }
            return individualTravelInfo;
        }

        private static List<RestDependentActionsList> GetDefaultActionsList(RestIndividualDependentsInfoWrapper dependent, string errorCode = null)
        {
            var dependentsActionList = new List<RestDependentActionsList>
            {
                new RestDependentActionsList(VisaActionsType.Cancel.ToString(), errorCode)
            };

            if (dependent.IndividualVisaInformation.VisaType == VisaType.EntryPermit.GetAttribute<VisaTypeAttr>().Type)
                dependentsActionList.Add(new RestDependentActionsList(VisaActionsType.New.ToString(), errorCode));
            else
            {
                dependentsActionList.Add(new RestDependentActionsList(VisaActionsType.Transfer.ToString(), errorCode));
                dependentsActionList.Add(new RestDependentActionsList(VisaActionsType.Renew.ToString(), errorCode));
            }

            return dependentsActionList;
        }

        private static List<RestDependentActionsList> RemoveDashboardAction(List<RestDependentActionsList> actionList, string actionName, string errorCode)
        {
            var action = actionList.FirstOrDefault(p => p.Action == actionName);
            if (action != null)
            {
                action.Status = false;
                action.ErrorCode = errorCode;
            }
            return actionList;
        }
    }
}