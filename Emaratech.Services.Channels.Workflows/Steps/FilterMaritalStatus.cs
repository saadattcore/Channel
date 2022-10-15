using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class FilterMaritalStatus : ChannelWorkflowStep
    {
        public ReferenceParameter<RestRenderGraph> FormConfiguration { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            if (FormConfiguration.Value == null)
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            
            DataHelper.FilterLookup("MaritalStatusId", GetAllowedMaritalStatus(), FormConfiguration.Value);
            return StepState = WorkflowStepState.Done;
        }

        private IEnumerable<string> GetAllowedMaritalStatus()
        {
            var values = new List<MaritalStatus>
            {
                MaritalStatus.Single,
                MaritalStatus.Married,
                MaritalStatus.Divorced,
                MaritalStatus.Widow
            };
            return values.Select(s => Convert.ToInt32(s).ToString());
        }
    }
}