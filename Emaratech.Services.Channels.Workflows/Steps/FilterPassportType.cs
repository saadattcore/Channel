using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class FilterPassportType : ChannelWorkflowStep
    {
        public InputParameter<RestRenderGraph> FormConfiguration { get; set; }

        public OutputParameter UpdatedFormConfiguration { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            UpdatedFormConfiguration = new OutputParameter(nameof(UpdatedFormConfiguration));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(FormConfiguration);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            
            DataHelper.FilterLookup("PassportTypeId", GetAllowedPassportType(), FormConfiguration.Get());

            UpdatedFormConfiguration.Set(FormConfiguration.Get());
            return StepState = WorkflowStepState.Done;
        }

        private IEnumerable<string> GetAllowedPassportType()
        {
            var values = new List<PassportType>
            {
                PassportType.Normal
            };
            return values.Select(s => Convert.ToInt32(s).ToString());
        }
    }
}