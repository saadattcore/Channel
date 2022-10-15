using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.Vision.Model;
using Newtonsoft.Json.Linq;
using Emaratech.Services.eVisa.Lookups;
using Emaratech.Services.Application.Model;
using System.IO;
using Newtonsoft.Json;
using Emaratech.Services.eVisa.Reports;
using Emaratech.Services.Services.Api;
using Emaratech.Services.Template.Model;
using RazorEngine;
using log4net;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    public class ApplicationRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationRepository));

        public static List<RestApplicationSearchRow> GetEntryPermitRejectedStatus(string categoryID)
        {
            string rejectedStatus = ConfigurationSystem.AppSettings["RejectedStatusId"];

            Services.Api.IServiceApi api = new ServiceApi(ConfigurationSystem.AppSettings["ServiceApi"]);
            var allServices = api.EmaratechServicesServicesServicesServiceGetServices();
            var entryPermitNewIds = allServices.Where(x => x.CategoryId == categoryID).Select(x => x.ServiceId).ToList();

            return GetStatus(rejectedStatus, entryPermitNewIds);
        }

        public static List<RestApplicationSearchRow> GetApprovedStatus(string categoryID)
        {
            string approvedStatus = ConfigurationSystem.AppSettings["ApprovedStatusId"];

            Services.Api.IServiceApi api = new ServiceApi(ConfigurationSystem.AppSettings["ServiceApi"]);
            var allServices = api.EmaratechServicesServicesServicesServiceGetServices();
            var entryPermitNewIds = allServices.Where(x => x.CategoryId == categoryID).Select(x => x.ServiceId).ToList();

            return GetStatus(approvedStatus, entryPermitNewIds);
        }

        public static List<RestApplicationSearchRow> GetApplicationsByStatusId(string statusId, string categoryID)
        {
            Services.Api.IServiceApi api = new ServiceApi(ConfigurationSystem.AppSettings["ServiceApi"]);
            var allServices = api.EmaratechServicesServicesServicesServiceGetServices();
            var entryPermitNewIds = allServices.Where(x => x.CategoryId == categoryID).Select(x => x.ServiceId).ToList();            
            return GetStatusForPostedApps(statusId , entryPermitNewIds);
        }


        private static List<RestApplicationSearchRow> GetStatus(string approvedStatus, List<string> serviceIds = null)
        {
            var criteria = new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                    {
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicantDetails",
                            PropertyName = "*"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "SponsorDetails",
                            PropertyName = "*"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "*"
                        }


                    },
                StatusId = approvedStatus, //ApplicationStatus.Approved.GetHashCode().ToString(),
                IsPaid = "true",
                IsApplicationTypeNotNull = "true",
                SelectedFlag = 1,
                SelectedFlagValue = 0,
                ServiceIds = serviceIds


                //ApplicationId = searchCriteria?.Criteria,
                //CreatedFromDate = searchCriteria?.StartDate,
                //CreatedToDate = searchCriteria?.EndDate
            };

            var searchPayload = Newtonsoft.Json.JsonConvert.SerializeObject(criteria);

            var applicationSearchResult =
                (ServicesHelper.GetApplicationSearchApi.GetApplicationsByCriteria(criteria)).RestApplicationSearchRow.ToList();

            string servicesId = serviceIds != null ? string.Join(",", serviceIds) : string.Empty;
            Log.Debug($"Approved Application Status Ids === { approvedStatus} and services id ==== {servicesId}")
            ;
            // Log.Debug($"Approved Application for eVisa :  ====  {JsonConvert.SerializeObject(applicationSearchResult)}");
            return applicationSearchResult;
        }

        private static List<RestApplicationSearchRow> GetStatusForPostedApps(string approvedStatus, List<string> serviceIds = null)
        {
            var criteria = new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                    {
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicantDetails",
                            PropertyName = "*"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "SponsorDetails",
                            PropertyName = "*"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "*"
                        }


                    },
                StatusId = approvedStatus, //ApplicationStatus.Approved.GetHashCode().ToString(),
                IsPaid = "true",
                IsApplicationTypeNotNull = "true",
                SelectedFlag = 4,
                SelectedFlagValue = 0,
                ServiceIds = serviceIds


                //ApplicationId = searchCriteria?.Criteria,
                //CreatedFromDate = searchCriteria?.StartDate,
                //CreatedToDate = searchCriteria?.EndDate
            };

            var searchPayload = Newtonsoft.Json.JsonConvert.SerializeObject(criteria);

            var applicationSearchResult =
                (ServicesHelper.GetApplicationSearchApi.GetApplicationsByCriteria(criteria)).RestApplicationSearchRow.ToList();

            string servicesId = serviceIds != null ? string.Join(",", serviceIds) : string.Empty;
            Log.Debug($"Approved Application Status Ids === { approvedStatus} and services id ==== {servicesId}")
            ;
            // Log.Debug($"Approved Application for eVisa :  ====  {JsonConvert.SerializeObject(applicationSearchResult)}");
            return applicationSearchResult;
        }

    }
}
