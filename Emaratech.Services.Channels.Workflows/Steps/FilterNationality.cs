using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Forms.Api;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.MappingMatrix.Model;
using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class FilterNationality : ChannelWorkflowStep
    {
        //public InputParameter<string> SystemId { get; set; }
        //public InputParameter<string> CategoryId { get; set; }
        //public InputParameter<string> UserType { get; set; }
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

            DataHelper.FilterLookup("CurrentNationalityId", GetAllowedNationalities(), FormConfiguration.Get());
            UpdatedFormConfiguration.Set(FormConfiguration.Get());
            return StepState = WorkflowStepState.Done;
        }
        
        private IEnumerable<string> GetAllowedNationalities()
        {
            return LookupHelper.GetLookupItemsByCol4(Constants.Lookups.CountryLookupId, "1");
        }
    }
}