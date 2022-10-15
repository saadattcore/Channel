using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Channels.Workflows.Steps.SubmitZajelApplicationNs;
using Emaratech.Services.Workflows.Engine;
using Emaratech.Services.Zajel.Model;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Workflows.Models;
using log4net;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SubmitZajelApplication : ChannelWorkflowStep
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(SubmitZajelApplication));

        #region Lookups
        private ILookup emirateLookup => LookupEmirate.Instance;
        private ILookup cityLookup => LookupCity.Instance;
        private ILookup areaLookup => LookupArea.Instance;
        #endregion

        #region Properties
        public string ApplicationType { get; set; }
        public string OdrStatus { get; set; }
        public string DeliveryMode { get; set; }
        public string ProductType { get; set; }
        #endregion

        #region Input/output arguments
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public InputParameter<string> MobileNumber { get; set; }
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<IEnumerable<FeeConfiguration>> Fees { get; set; }
        public OutputParameter UniqueId { get; }
        public InputParameter<string> ApplicationId { get; set; }
        #endregion

        public SubmitZajelApplication()
        {
            UniqueId = new OutputParameter(nameof(UniqueId));

            ApplicationType = "Standard";
            DeliveryMode = "Dt";
            ProductType = "Residence";
            OdrStatus = "Y";
        }

        public override async Task<WorkflowStepState> Execute()
        {
            CheckRequiredInput(ApplicationId);

            string uniqueId = string.Empty;
            Logger.Debug("Executing step...About to check if submit application to zajel is required.");
            if (IsZajelFeePresent(Fees))
            {
                Logger.Debug($"Submit application to zajel is required.");
                var unifiedApplication = UnifiedApplication.Get();

                Logger.Debug($"Application information received from context is {unifiedApplication}");

                var mobileNumber = MobileNumber.Get();
                var formApplicantMobile = unifiedApplication["ApplicantDetails"]["MobileNo"]?.ToString();
                var formSponsorMobile = unifiedApplication["SponsorDetails"]["MobileNo"]?.ToString();
                if (!string.IsNullOrWhiteSpace(formApplicantMobile))
                {
                    mobileNumber = formApplicantMobile;
                }
                else if (!string.IsNullOrWhiteSpace(formSponsorMobile))
                {
                    mobileNumber = formSponsorMobile;
                }

                var appInfo = new ApplicationInfo()
                {
                    SystemId = SystemId.Get(),
                    ApplicationId = this.ApplicationId.Get(),
                    ContactNo = mobileNumber,
                    Landline = unifiedApplication["ApplicantDetails"]["ResidenceTelNo"]?.Value<string>(),
                    SponsorName = unifiedApplication["SponsorDetails"]["SponsorFullNameE"]?.Value<string>(),
                    Area = GetArea(unifiedApplication),
                    Address = GetAddress(unifiedApplication),
                    PoBox = unifiedApplication["ApplicantDetails"]["POBox"]?.Value<string>(),
                    OdrStatus = this.OdrStatus,
                    DeliveryMode = this.DeliveryMode.ToDeliveryModeEnum(),
                    ApplicationType = ApplicationType,
                    FileNo = unifiedApplication["SponsorDetails"]["NewSponsorFileNo"]?.Value<string>(),
                    ProductType = this.ProductType.ToProductTypeEnum()
                };
                Logger.Debug($"About to store zajel record with with AppId {appInfo.ApplicationId}, SponsorName {appInfo.SponsorName}");

                uniqueId = await ServicesHelper.PutApplicationToZajelSubmitQueue(appInfo);
                Logger.Debug($"Zajel record stored with unique id {appInfo.ApplicationId}.");
            }
            else
            {
                Logger.Debug("Submit application to zajel is not required.");
            }

            UniqueId.Set(uniqueId);
            return WorkflowStepState.Done;
        }

        private string GetArea(JObject unifiedApplication)
        {
            var areaId = unifiedApplication["ApplicantDetails"]["AreaId"]?.Value<string>();
            return this.areaLookup.GetEn(areaId);
        }

        private string GetAddress(JObject unifiedApplication)
        {
            var street = unifiedApplication["ApplicantDetails"]["Street"]?.Value<string>();
            var cityId = unifiedApplication["ApplicantDetails"]["CityId"]?.Value<string>();
            var building = unifiedApplication["ApplicantDetails"]["Building"]?.Value<string>();
            var city = this.cityLookup.GetEn(cityId);
            var emirateId = unifiedApplication["ApplicantDetails"]["EmirateId"]?.Value<string>();
            var emirate = this.emirateLookup.GetEn(emirateId);

            var address = !string.IsNullOrEmpty(building) ? $"{street} (building {building}) {city} {emirate}" : $"{street} {city} {emirate}";
            return address;
        }

        private bool IsZajelFeePresent(InputParameter<IEnumerable<FeeConfiguration>> Fees)
        {
            bool retValue = false;
            if (Fees != null)
            {
                Logger.Debug("Fees input parameter is not null, continue check.");
                if (Fees.Get() != null)
                {
                    Logger.Debug("Value of Fees input parameter is not null, continue check.");
                    if (Fees.Get().Any(x => string.Equals(x.FeeTypeId, Constants.Fees.DeliveryFeeId)))
                    {
                        Logger.Debug("Found delivery fee in the list of incoming fees.");
                        retValue = true;
                    }
                    else
                    {
                        Logger.Debug("Delivery fee not found in the list of incoming fees.");
                        Fees.Get().ToList().ForEach(x => Logger.Debug($"Fee type {x.FeeTypeId}"));
                    }
                }
            }

            return retValue;
        }
    }
}