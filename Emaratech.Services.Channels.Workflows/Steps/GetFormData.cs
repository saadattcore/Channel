using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using log4net;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class GetFormData : ChannelWorkflowStep
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(ValidateData));

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ApplicationData { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public InputParameter<string> ResidenceNo { get; set; }

        public OutputParameter UnifiedApplicationWithFormData { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            UnifiedApplicationWithFormData = new OutputParameter(nameof(UnifiedApplicationWithFormData));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            
            CheckRequiredInput(ApplicationData);
            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var appData = JObject.Parse(ApplicationData.Get());
            foreach (var node in (JObject)appData["Application"])
            {
                FixDates((JObject)node.Value);
            }

            await CheckMapping(UnifiedApplication.Get(), appData);
            UnifiedApplicationWithFormData.Set(UnifiedApplication.Get());

            return StepState = WorkflowStepState.Done;
        }

        public void FixDates(JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                DateTime date;
                if (DataHelper.TryParseDateTimeExtended(property.Value?.ToString() ?? string.Empty, out date))
                {
                    property.Value = date.ToString("yyyy-MM-dd");
                }
            }
        }

        protected virtual async Task CheckMapping(JObject unifiedAppDoc, JObject data)
        {
            //TODO: Need to refactor this logic and convert this into smaller steps.

            var names = new Dictionary<string, string>
            {
                {"FirstNameE", string.Empty},
                {"MiddleNameE", string.Empty},
                {"LastNameE", string.Empty},
                {"FirstNameA", string.Empty},
                {"MiddleNameA", string.Empty},
                {"LastNameA", string.Empty}
            };
            
            unifiedAppDoc.Merge(data["Application"]);
            var applicationNode = (JObject)unifiedAppDoc["ApplicationDetails"];
            var applicantNode = (JObject)unifiedAppDoc["ApplicantDetails"];

            string isUrgent = null;
            string travelType = null;

            foreach (var p in applicantNode.Properties())
            {
                if (p.Name.ToLower().Contains("name"))
                {
                    p.Value = p.Value?.ToString().ToUpper();
                }

                if (names.ContainsKey(p.Name))
                {
                    names[p.Name] = p.Value?.ToString();
                }
                else if (p.Name == "ResidenceRequestYear")
                {
                    p.Value = await GetYearsOfResidence(p.Value?.ToString());
                }
                else if (p.Name == "IsInsideUAE")
                {
                    var value = p.Value?.ToString();
                    travelType = await GetTravelStatus(value);
                    TravelType travel = (TravelType)Convert.ToInt32(travelType);
                    WorkflowConstants.TravelTypeIsInsideMapping.TryGetValue(travel, out value);

                    //Set below for entry permit new because GetTravelStatus will not be called for entry permit new and will come from form
                    applicantNode["IsInsideUAE"] = value;
                }
                else if (p.Name == Constants.VisaNumber)
                {
                    var isResidence = !string.IsNullOrEmpty(ResidenceNo?.Get());
                    p.Value = DataHelper.UnformatVisaNumber(isResidence, p.Value?.ToString());
                }
            }
            foreach (var p in applicationNode.Properties())
            {
                if (p.Name == "ResidencyPickup")
                {
                    isUrgent = await GetUrgency(p.Value?.ToString());
                }
            }

            if (isUrgent != null)
                applicationNode["IsUrgent"] = isUrgent;
            //if (travelType != null)
            //    applicantNode["InsideOutside"] = travelType;

            applicantNode["FullNameE"] = ComputeFullName(names["FirstNameE"], names["MiddleNameE"], names["LastNameE"]);
            applicantNode["FullNameA"] = ComputeFullName(names["FirstNameA"], names["MiddleNameA"], names["LastNameA"]);
        }


        private async Task<string> GetUrgency(string value)
        {
            var resPickupItems = await DataHelper.GetResidencyPickupValues(SystemId.Get());
            return resPickupItems?.FirstOrDefault(r => r.ItemId == value)?.Col1;
        }

        private async Task<string> GetYearsOfResidence(string value)
        {
            var yearsOfResidenceLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.YearsOfResidenceLookupIdKey);
            if (!string.IsNullOrEmpty(yearsOfResidenceLookupId))
            {
                var yearsOfResidenceLookup = await ServicesHelper.GetLookupItems(yearsOfResidenceLookupId);
                return yearsOfResidenceLookup.FirstOrDefault(l => l.ItemId == value)?.ValueEn;
            }
            return string.Empty;
        }

        private async Task<string> GetTravelStatus(string value)
        {
            var insideCountryLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.InsideCountryLookupIdKey);
            if (!string.IsNullOrEmpty(insideCountryLookupId))
            {
                var isInsideCountryLookup = await ServicesHelper.GetLookupItems(insideCountryLookupId);
                LOG.Debug($"Travel status id: {value}");
                var lookup = isInsideCountryLookup.FirstOrDefault(l => l.ItemId == value);
                LOG.Debug($"Travel status lookup id: {lookup?.Col1}");
                return lookup?.Col1;
            }
            return string.Empty;
        }

        private string ComputeFullName(string firstName, string middleName, string lastName)
        {
            return $"{firstName} {middleName} {lastName}";
        }
    }
}