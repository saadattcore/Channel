using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Forms.Model;
using System;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Application.Model;
using log4net;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //STEP_ID=37B54E29B4B64D6586C77AAAF05FE00E
    public class ReuseFormData : ChannelWorkflowStep
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(ReuseFormData));


        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> SponsorEmail { get; set; }
        public InputParameter<string> MobileNo { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public InputParameter<string> ApplicationId { get; set; }
        public InputParameter<RestRenderGraph> FormConfiguration { get; set; }
        public InputParameter<string> ResidenceNo { get; set; }

        public OutputParameter Data { get; set; }
        public override void Initialize()
        {
            base.Initialize();
            Data = new OutputParameter(nameof(Data));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            JObject obj = new JObject();

            // Get all field names of the form
            var fieldsByEntity = DataHelper.GetFormFieldByEntity(FormConfiguration?.Get());

            if (!string.IsNullOrEmpty(ApplicationId?.Get()) && ApplicationId.IsFilled())
                obj = await GetFormDataFromApplication(fieldsByEntity);
            else
                obj = await GetFormDataFromVisionReuse(fieldsByEntity);

            Data.Set(obj.ToString(Newtonsoft.Json.Formatting.None));

            return StepState = WorkflowStepState.Done;

        }

        private object TransformFieldValue(FormField field, object value)
        {
            // TODO: client should check this
            // If the field is a phone number, add a dash after prefix
            if (field.CustomBehavior == FieldCustomBehavior.Phone || field.CustomBehavior == FieldCustomBehavior.MobilePhone)
            {
                int prefix = field.CustomBehavior == FieldCustomBehavior.Phone ? 2 : 3;
                var phoneNo = value?.ToString();
                if (phoneNo != null && phoneNo.Length > prefix + 1 && !phoneNo.Contains("-"))
                {
                    value = phoneNo.Insert(prefix, "-");
                }
            }
            else if (field.CustomBehavior == FieldCustomBehavior.Lookup)
            {
                return value.ToString();
            }
            else if (field.CustomBehavior == FieldCustomBehavior.Date)
            {
                DateTime date;
                if (DateTime.TryParse(value.ToString(), out date))
                {
                    return date.ToString("yyyy-MM-dd");
                }
            }
            else if (field.CustomBehavior == FieldCustomBehavior.VisaNumber)
            {
                var isResidence = !string.IsNullOrEmpty(ResidenceNo?.Get());
                return DataHelper.FormatVisaNumber(isResidence, value?.ToString());
            }
            return value;
        }

        /// <summary>
        /// Get a field value from vision data or from input parameter
        /// </summary>
        /// <param name="entity">The entity name</param>
        /// <param name="fieldName">The field name</param>
        /// <param name="unifiedApplication">The UnifiedApplication document</param>
        /// <returns></returns>
        private async Task<object> GetFieldValue(string entity, string fieldName, JObject unifiedApplication)
        {
            // First check if there is an input parameter corresponding
            var pi = GetType().GetProperty(fieldName);
            if (pi != null)
            {
                var method = pi.PropertyType.GetMethod("Get");
                var target = pi.GetValue(this, null);
                if (target != null)
                {
                    return method.Invoke(target, null);
                }
            }
            else
            {
                var value = unifiedApplication[entity][fieldName]?.Value<string>();

                // Specific case for years of residence
                if (fieldName == "ResidenceRequestYear")
                {
                    var yearsOfResidenceLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.YearsOfResidenceLookupIdKey);
                    if (!string.IsNullOrEmpty(yearsOfResidenceLookupId))
                    {
                        var yearsOfResidenceLookup = await ServicesHelper.GetLookupItems(yearsOfResidenceLookupId);
                        return yearsOfResidenceLookup.FirstOrDefault(l => l.ValueEn == value)?.ItemId;
                    }
                }
                else if (fieldName == "IsInsideUAE")
                {
                    // Set default value
                    var insideCountryLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.InsideCountryLookupIdKey);
                    if (!string.IsNullOrEmpty(insideCountryLookupId))
                    {
                        var insideCountryLookup = await ServicesHelper.GetLookupItems(insideCountryLookupId);
                        return insideCountryLookup.FirstOrDefault(l => l.ValueEn.ToLower().Contains("no"))?.ItemId;
                    }
                }
                else if (fieldName == "ResidencyPickup")
                {
                    // Set default value
                    var residencyPickupLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.ResidencyPickupLookupIdKey);
                    if (!string.IsNullOrEmpty(residencyPickupLookupId))
                    {
                        var residencyPickupLookup = await ServicesHelper.GetLookupItems(residencyPickupLookupId);
                        return residencyPickupLookup.FirstOrDefault(l => l.Col1.Contains("0"))?.ItemId;
                    }
                }
                else if (fieldName == "FetchEntryPermitDataAction")
                {
                    // Set default value
                    var fetchEntryPermitDataActionLookupIdKey = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.FetchEntryPermitDataActionLookupIdKey);
                    if (!string.IsNullOrEmpty(fetchEntryPermitDataActionLookupIdKey))
                    {
                        var fetchEntryPermitDataActionLookup = await ServicesHelper.GetLookupItems(fetchEntryPermitDataActionLookupIdKey);
                        return fetchEntryPermitDataActionLookup.FirstOrDefault(l => l.LookupId.Contains("1"))?.ItemId;
                    }
                }
                return value;
            }
            return null;
        }

        private async Task<object> GetFieldValueFromApplication(string entity, string fieldName, RestApplicationSearchRow application)
        {
            var value = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.Entity?.ToLower() == entity?.ToLower() && p.PropertyName.ToLower() == fieldName?.ToLower())?.Value;

            // Specific case for years of residence
            if (fieldName == "ResidenceRequestYear")
            {
                var yearsOfResidenceLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.YearsOfResidenceLookupIdKey);
                if (!string.IsNullOrEmpty(yearsOfResidenceLookupId))
                {
                    var yearsOfResidenceLookup = await ServicesHelper.GetLookupItems(yearsOfResidenceLookupId);
                    return yearsOfResidenceLookup.FirstOrDefault(l => l.ValueEn == value)?.ItemId;
                }
            }

            if (fieldName == "ResidencyPickup")
            {
                value = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.Entity?.ToLower() == entity?.ToLower() && p.PropertyName.ToLower() == "isurgent")?.Value;
                var resPickupItems = await DataHelper.GetResidencyPickupValues(SystemId.Get());
                return resPickupItems?.FirstOrDefault(r => r.Col1 == value)?.ItemId;
            }

            if (fieldName == "IsInsideUAE")
            {
                var insideCountryLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.InsideCountryLookupIdKey);
                if (!string.IsNullOrEmpty(insideCountryLookupId))
                {
                    var insideCountryLookup = await ServicesHelper.GetLookupItems(insideCountryLookupId);
                    var travel = (int)WorkflowConstants.TravelTypeIsInsideMapping
                        .Where(p => p.Value == value)
                        .Select(p => new { Key = p.Key, Value = p.Value })
                        .FirstOrDefault(x => x.Value == value)?.Key;

                    return insideCountryLookup.FirstOrDefault(l => l.Col1 == travel.ToString())?.ItemId;
                }
            }

            return value;
        }

        private async Task<JObject> GetFormDataFromVisionReuse(Dictionary<string, List<FormField>> fieldsByEntity)
        {
            JObject obj = new JObject();
            var doc = UnifiedApplication.Get();

            foreach (var fieldByEntity in fieldsByEntity)
            {
                // For each field, get the logical id corresponding
                foreach (var field in fieldByEntity.Value)
                {
                    object value = await GetFieldValue(fieldByEntity.Key, field.Name, doc);
                    if (value != null)
                    {
                        value = TransformFieldValue(field, value);
                        if (obj.Property(fieldByEntity.Key) == null)
                        {
                            obj[fieldByEntity.Key] = new JObject();
                        }

                        obj[fieldByEntity.Key][field.Name] = JToken.FromObject(value);
                    }
                }
            }

            return obj;
        }

        private async Task<JObject> GetFormDataFromApplication(Dictionary<string, List<FormField>> fieldsByEntity)
        {
            JObject obj = new JObject();
            var application = await ServicesHelper.SearchApplicationByApplicationId(ApplicationId.Get());

            foreach (var fieldByEntity in fieldsByEntity)
            {
                // For each field, get the logical id corresponding
                foreach (var field in fieldByEntity.Value)
                {
                    object value = await GetFieldValueFromApplication(fieldByEntity.Key, field.Name, application);

                    if (value != null)
                    {
                        value = TransformFieldValue(field, value);
                        if (obj.Property(fieldByEntity.Key) == null)
                        {
                            obj[fieldByEntity.Key] = new JObject();
                        }

                        obj[fieldByEntity.Key][field.Name] = JToken.FromObject(value);
                    }
                }
            }

            return obj;
        }
    }
}