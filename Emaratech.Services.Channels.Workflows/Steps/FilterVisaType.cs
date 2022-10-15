using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class FilterVisaType : ChannelWorkflowStep
    {
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> CategoryId { get; set; }
        public InputParameter<string> UserType { get; set; }
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

            DataHelper.FilterLookup("ServiceId", GetAllowedVisaTypes(), FormConfiguration.Get());
            UpdatedFormConfiguration.Set(FormConfiguration.Get());
            return StepState = WorkflowStepState.Done;
        }
        
        private IEnumerable<string> GetAllowedVisaTypes()
        {
            return new List<string>
            {
                Constants.Services.EntryPermitNewResidenceService,
                Constants.Services.EntryPermitNewLeisureShortSingleService,
                Constants.Services.EntryPermitNewLeisureShortMultiService,
                Constants.Services.EntryPermitNewLeisureLongSingleService,
                Constants.Services.EntryPermitNewLeisureLongMultiService
            };
        }
    }
}