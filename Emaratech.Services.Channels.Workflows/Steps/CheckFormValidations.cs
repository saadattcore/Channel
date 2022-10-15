using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Forms.Validation;
using Emaratech.Services.Forms.Validation.Models;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class CheckFormValidation : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CheckFormValidation));

        public InputParameter<RestRenderGraph> FormConfiguration { get; set; }
        public InputParameter<string> ApplicationData { get; set; }
        
        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(ApplicationData);
            CheckRequiredInput(FormConfiguration);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var appData = JObject.Parse(ApplicationData.Get())["Application"];
            var fieldsByEntity = DataHelper.GetFormFieldByEntity(FormConfiguration.Get());
            foreach (var fieldByEntity in fieldsByEntity)
            {
                foreach (var field in fieldByEntity.Value)
                {
                    if (field.OriginalField.Validations != null)
                    {
                        foreach (var validation in field.OriginalField.Validations)
                        {
                            var fieldValue = appData[fieldByEntity.Key][field.Name]?.ToString();
                            var validationType = new ValidationType
                            {
                                CustomTypeName = null,
                                ValidationTypeId = validation.ValidationTypeId
                            };

                            var fieldName = new FullName
                            {
                                Entity = fieldByEntity.Key,
                                Name = field.Name
                            };

                            var validator = new FieldValidationExecutor();
                            var isValid = validator.IsValid(validationType, fieldName, fieldValue, validation.Data2, GetFormData(appData));
                            if (!isValid)
                            {
                                var message = $"Validation '{validation.ResourceKey} failed for field '{field.Name}'";
                                Log.Error(message);
                                throw ChannelWorkflowErrorCodes.FormFieldNotValid.ToWebFault(message);
                            }
                        }
                    }
                }
            }

            return StepState = WorkflowStepState.Done;
        }

        private static IDictionary<FullName, string> GetFormData(JToken applicationData)
        {
            var formData = new Dictionary<FullName, string>();
            return formData;
        }
    }
}