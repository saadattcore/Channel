using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.MappingMatrix.Model;
using Emaratech.Services.WcfCommons.Faults.Models;
using log4net;
using System.Configuration;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;

namespace Emaratech.Services.Channels.Helpers
{
    public class ApplicationHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationHelper));

        public async static Task<IList<RestApplicationSearchRow>> GetApplicationSearchResult(RestApplicationSearchCriteria searchCriteria)
        {
            var applicationSearchResult = (await ApiFactory.Default.GetApplicationSearchApi()
            .GetApplicationsByCriteriaAsync(searchCriteria)).RestApplicationSearchRow.ToList();

            return applicationSearchResult;
        }

        public static RestApplicationSearchCriteria GetIncompleteApplicationSearchCriteria(string userId)
        {
            return new RestApplicationSearchCriteria
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
                UserId = userId,
                StatusId = string.Concat((int)ApplicationStatus.IncompletePayment, ",", (int)ApplicationStatus.PaymentFailed)
            };
        }

        public static async Task ValidateApplication(string userId, string applicationId)
        {
            if (!string.IsNullOrEmpty(applicationId))
            {
                var searchCriteria = new RestApplicationSearchCriteria
                {
                    SelectColumns = new List<RestApplicationSearchKeyValues>
              {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "UserId"
                    }
                },

                    ApplicationId = applicationId
                };

                var searchResult = await GetApplicationSearchResult(searchCriteria);

                if (searchResult?.Count > 0)
                {
                    string applicationUserId = searchResult?.FirstOrDefault()?.RestApplicationSearchKeyValues.FirstOrDefault(p => p.PropertyName.ToLower() == "userid")?.Value;

                    if (applicationUserId != userId)
                    {
                        Log.Debug($"Unauthorized application found for user id {userId} and application id {applicationId}");
                        throw ChannelErrorCodes.Unauthorized.ToWebFault("You are not authorized to do this request");
                    }
                }
            }
        }

        /// <summary>
        /// Filter and get applications based on approved entry permit application and related to current user.
        /// </summary>
        /// <param name="applicationIds">A list of application ids to retrieve them</param>
        /// <param name="UserId">The User id</param>
        /// <returns>A list of filtered applications <see cref="RestApplicationSearchRow"/></returns>
        public static async Task<IList<RestApplicationSearchRow>> FilterAndGetEntryPermitApplications(
                                    IList<string> applicationIds,
                                    IList<string> establishmentCodes)
        {
            try
            {
                return await GetApplicationSearchResult(
                    GetApplicationEntryPermitSearchCriteria(applicationIds, establishmentCodes));
            }
            catch
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault("Not valid applications.");
            }
        }

        private static RestApplicationSearchCriteria GetApplicationEntryPermitSearchCriteria(
                                    IList<string> applicationIds,
                                    IList<string> establishmentCodes)
        {
            var applicationStatuses = ConfigurationRepository.GetApplicationPrintStatuses();
            var entryPermitApplicationType = ConfigurationRepository.GetEntryPermitApplicationType();
            var applicationIdsCommaSeprated = string.Join(",", applicationIds);
            var establishmentCodesCommaSeprated = string.Join(",", establishmentCodes);

            return new RestApplicationSearchCriteria()
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
                        PropertyName = "UserId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ServiceId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "File_Type"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicantDetails",
                        PropertyName = "Visa_Number"
                    }
                },
                StatusId = applicationStatuses,
                ApplicationIdIn = applicationIdsCommaSeprated,
                ApplicationTypeIn = entryPermitApplicationType,
                EstablishmentCode = establishmentCodesCommaSeprated
            };
        }

        public static async Task<IList<RestApplicationSearchRow>> GetApplicationsForEntryPermits(
                            IList<string> entryPermitIds,
                            IList<string> establishmentCodes)
        {
            try
            {
                return await GetApplicationSearchResult(
                    GetEntryPermitSearchCriteria(entryPermitIds, establishmentCodes));
            }
            catch
            {
                throw ChannelErrorCodes.BadRequest.ToWebFault("Not valid applications.");
            }
        }

        public static RestApplicationSearchCriteria GetEntryPermitSearchCriteria(
                            IList<string> entryPermitIds,
                            IList<string> establishmentCodes)
        {
            var entryPermitIdsCommaSeprated = string.Join(",", entryPermitIds);
            var establishmentCodesCommaSeprated = string.Join(",", establishmentCodes);
            var applicationStatuses = ConfigurationRepository.GetApplicationPrintStatuses();

            return new RestApplicationSearchCriteria()
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
                        PropertyName = "UserId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ServiceId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "File_Type"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicantDetails",
                        PropertyName = "Visa_Number"
                    }
                },
                VisaNumber = entryPermitIdsCommaSeprated,
                EstablishmentCode = establishmentCodesCommaSeprated,
                StatusId = applicationStatuses,
            };
        }

        public static async Task<IList<RestApplicationActions>> GetApplicationActions(RestApplicationActionsCriteria actionsCriteria)
        {
            IList<RestApplicationActions> actions = new List<RestApplicationActions>();

            var criteria = new Dictionary<string, string>
                        {
                            {nameof(actionsCriteria.ApplicationStatus), actionsCriteria.ApplicationStatus},
                            {nameof(actionsCriteria.ServiceId), actionsCriteria.ServiceId},
                            {nameof(actionsCriteria.UserType), actionsCriteria.UserType},
                            {nameof(actionsCriteria.ApplicationModule), actionsCriteria.ApplicationModule.ToString()}
                        };
            var items = await MappingMatrixHelper.GetItemsFromMappingMatrix(ConfigurationManager.AppSettings["MappingMatrixDashboardApplicationAction"], criteria);

            foreach (var item in items)
            {
                actions.Add(new RestApplicationActions
                {
                    Action = item.mappingMatrixItems["Action"],
                    StatusId = item.mappingMatrixItems[nameof(actionsCriteria.ApplicationStatus)],
                    ServiceId = item.mappingMatrixItems[nameof(actionsCriteria.ServiceId)],
                    UserType = item.mappingMatrixItems[nameof(actionsCriteria.UserType)]
                });
            }

            return actions;
        }

        public static IList<RestApplicationActions> RemoveAction(IList<RestApplicationActions> actions, ApplicationActions actionToRemove)
        {
            if (actions?.Count > 0)
            {
                var action = actions.FirstOrDefault(p => p.Action == actionToRemove.ToString());
                if (action != null)
                    actions.Remove(action);
            }
            return actions;
        }
    }

}