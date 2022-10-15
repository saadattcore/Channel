using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SponsorFileUpdateVision : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SponsorFileUpdateVision));

        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<IList<FeeConfiguration>> WorkflowFee { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            Log.Debug($"Executing Sponsorship File update flow");

            // In case your not selected to upload document
            // TODO: Check payment status
            if (WorkflowFee?.Get() != null && WorkflowFee.Get().Count > 0)
            {
                Log.Debug("Workflow Fee - " + JsonConvert.SerializeObject(WorkflowFee.Get()));
                Log.Debug($"Sponsorship status marked starting {SponsorNo.Get()}");
                var verified = await ServicesHelper.UpdateSponsorVerifyStatus(SponsorNo.Get());
                Log.Debug($"Sponsorship status marked as verified in Vision for SponsorNo {SponsorNo.Get()} verified {verified}");
            }

            return WorkflowStepState.Done;
        }
    }
}