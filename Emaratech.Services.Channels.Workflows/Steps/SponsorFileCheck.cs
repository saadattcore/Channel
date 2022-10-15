using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SponsorFileCheck : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SponsorFileCheck));

        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> PlatformId { get; set; }
        public OutputParameter View { get; set; }
        public OutputParameter FormId { get; set; }
        public OutputParameter FormConfiguration { get; set; }
        public OutputParameter IsSponsorFileOpen { get; set; }
        public ReferenceParameter<string> ApplicationData { get; set; }
        public ReferenceParameter<IList<FeeConfiguration>> WorkflowFee { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            View = new OutputParameter(nameof(View));
            FormId = new OutputParameter(nameof(FormId));
            FormConfiguration = new OutputParameter(nameof(FormConfiguration));
            IsSponsorFileOpen = new OutputParameter(nameof(IsSponsorFileOpen));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            try
            {
                var sponsorNo = SponsorNo.Get();

                Log.Debug($"Starting Verification in Vision for {sponsorNo}");
                var sponsorshipInfo = await ServicesHelper.GetSponsorInfoBySponsorNo(sponsorNo);

                if (sponsorshipInfo == null)
                    throw ChannelWorkflowErrorCodes.InvalidSponsorFileNumber.ToWebFault($"The sponsor file number is inactive for sponsor no {sponsorNo}");

                IsSponsorFileOpen.Set(sponsorshipInfo.SponsorVerifyStatusId);
                if (sponsorshipInfo.FileCreatedDate < new DateTime(2014, 8, 1))
                {
                    Log.Debug($"Sponsor Verified - Sponsorship File Create Date < 2014,08,01. Created Date Received: {sponsorshipInfo.FileCreatedDate}");
                    return WorkflowStepState.Done;
                }

                if (sponsorshipInfo.SponsorVerifyStatusId == null || sponsorshipInfo.SponsorVerifyStatusId == 2)
                {
                    Log.Debug($"Sponsor Verified - Vision Flag {sponsorNo}. Received value is {sponsorshipInfo.SponsorVerifyStatusId}");
                    return WorkflowStepState.Done;
                }

                Log.Debug($"Sponsor Not Verified - Vision Flag {sponsorNo}. Received value is {sponsorshipInfo.SponsorVerifyStatusId}");
                CheckRequiredInput(ApplicationData);
                if (ParametersRequiringInput.Any())
                {
                    View.Set(ViewEnum.Form);
                    FormId.Set(Constants.Forms.SponsorFileFormId);

                    var data = await ServicesHelper.RenderFormForPlatform(Constants.Forms.SponsorFileFormId, PlatformId.Get());
                    FormConfiguration.Set(data);

                    Log.Debug($"Input Required {nameof(ApplicationData)}");
                    return WorkflowStepState.InputRequired;
                }

                var formData = JObject.Parse(ApplicationData.Value);
                var sponsorFileSelectionLookup = await ServicesHelper.GetLookupItems(Constants.Lookups.SponsorFileLookupId);
                var selectedOption = sponsorFileSelectionLookup
                    .Where(x => x.ItemId == formData["Application"]["SponsorDetails"]["IsSponsorFileOpen"].Value<string>())
                    .Single()
                    .Col1;
                IsSponsorFileOpen.Set(selectedOption);

                // User selected to open a new File (Pay Fee)
                if (IsSponsorFileOpen.Get()?.ToString() == "2")
                {
                    WorkflowFee.Value = new List<FeeConfiguration>
                    {
                        //SponsorshipFileApplicationFee
                        new FeeConfiguration { FeeTypeId = "1EC291CA95D54E0A9C03DF6FC02561BC", Amount = "100", Name="Sponsorship File Application Fee", ResourceKey = "SponsorshipFileApplicationFee" }
                        //SponsorshipFileIssuanceFee
                        ,new FeeConfiguration { FeeTypeId = "4295818E3F7D4CD59504C30148CCD681", Amount = "100", Name="Sponsorship File Issuance Fee", ResourceKey = "SponsorshipFileIssuanceFee"  }

                    };
                }

                Log.Debug($"Selected the option: {IsSponsorFileOpen.Get()}");
                FormId.Set(null);
                FormConfiguration.Set(null);
                ApplicationData.Value = null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
            Log.Debug($"END - All Inputs Provided.");
            return WorkflowStepState.Done;
        }
    }
}
