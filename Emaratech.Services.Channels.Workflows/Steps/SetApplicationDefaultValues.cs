using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using System.Xml;
using log4net;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetApplicationDefaultValues : ChannelWorkflowStep
    {
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }

        public OutputParameter UnifiedApplicationDefaulted { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(SetApplicationDefaultValues));

        public override void Initialize()
        {
            base.Initialize();
            UnifiedApplicationDefaulted = new OutputParameter(nameof(UnifiedApplicationDefaulted));
        }

        public async override Task<WorkflowStepState> Execute()
        {
            Log.Debug("Going to set application default values");
            await base.Execute();

            CheckRequiredInput(SystemId);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var unifiedApp = UnifiedApplication.Get();

            var matrixId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.MappingMatrixApplicationDefaultValuesIdKey);
            var defaultValues = await ServicesHelper.ResolveMappingMatrix(matrixId, new List<string>(), (values, versions) => values);

            matrixId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.MappingMatrixApplicationFieldsIdKey);
            var applicationFields = await ServicesHelper.ResolveMappingMatrix(matrixId, new List<string>(), (values, versions) => values);

            foreach (var defaultValueRow in defaultValues)
            {
                var defaultValue = defaultValueRow[1];
                var logicalId = defaultValueRow[0];

                var fieldPath = applicationFields.FirstOrDefault(x => x[0] == logicalId)?.ElementAt(1);
                var fieldPathArray = fieldPath.Split('/');

                // A default value can refer to another field
                var fieldPathDefaultValue = applicationFields.FirstOrDefault(x => x[0] == defaultValue)?.ElementAt(1);
                if (!string.IsNullOrEmpty(fieldPathDefaultValue))
                {
                    var pathArray = fieldPathDefaultValue.Split('/');
                    defaultValue = unifiedApp[pathArray[0]][pathArray[1]].Value<string>();
                }

                // Set default value only if there is no value in application
                if (string.IsNullOrEmpty(unifiedApp[fieldPathArray[0]][fieldPathArray[1]]?.ToString()))
                {
                    unifiedApp[fieldPathArray[0]][fieldPathArray[1]] = defaultValue;
                }         
            }

            UnifiedApplicationDefaulted.Set(unifiedApp);
            Log.Debug("Default values are set.");
            return StepState = WorkflowStepState.Done;
        }
    }
}
