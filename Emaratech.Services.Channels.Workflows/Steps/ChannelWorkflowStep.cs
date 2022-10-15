using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Common.Caching;
using Emaratech.Services.Systems.Properties;
using Emaratech.Services.Workflows.Engine;
using Emaratech.Services.Workflows.Engine.Interfaces;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public abstract class ChannelWorkflowStep : WorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChannelWorkflowStep));
        protected List<string> ParametersRequiringInput { get; set; }

        protected WorkflowStepState StepState;
        
        static ChannelWorkflowStep()
        {
            ServicesHelper.Init(new DefaultServiceFactory(),
             new SystemProperties(Cache.CacheTimeInMinutes));
        }

        public override void Initialize()
        {
            base.Initialize();

            StepState = WorkflowStepState.Init;
            ParametersRequiringInput = new List<string>();
        }

        public override Task<WorkflowStepState> Resume()
        {
            ParametersRequiringInput = new List<string>();
            return Execute();
        }

        public override Task<WorkflowStepState> Execute()
        {
            return Task.FromResult(WorkflowStepState.Executing);
        }

        protected void CheckRequiredInput<T>(ReferenceParameter<T> input)
        {
            if (input.Value==null)
            {
                ParametersRequiringInput.Add(input.Name);
            }
        }

        protected void CheckRequiredInput<T>(InputParameter<T> input)
        {
            if (!input.IsFilled())
            {
                Log.Debug($"Step {GetType().Name} requires input: {input.Name}");
                ParametersRequiringInput.Add(input.Name);
            }
        }

    }
}