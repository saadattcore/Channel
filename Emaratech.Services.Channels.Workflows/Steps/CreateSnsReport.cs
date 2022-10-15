using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Workflows.Steps.Report;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Workflows.Steps.Report;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using Emaratech.Services.Channels.Workflows.Steps.Processing;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class CreateSnsReport : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreateSnsReport));

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> EmiratesId { get; set; }
        public InputParameter<string> BirthDate { get; set; }
        public InputParameter<string> ReportEmailAddress { get; set; }
        public ReferenceParameter<JObject> ApplicationDataToSave { get; set; }

        public OutputParameter ReportData { get; }
        public OutputParameter EmailContent { get; }
        public OutputParameter EmailHeader { get; }
        public CreateSnsReport()
        {
            ReportData = new OutputParameter(nameof(ReportData));
            EmailContent = new OutputParameter(nameof(EmailContent));
            EmailHeader = new OutputParameter(nameof(EmailHeader));
        }

        private ProcessingStep AssembleProcessAndReturnFirstStep()
        {
            var RepState = new ReportState();
            ProcessingStep startStep = new BuildSponsorInfoReportStep(  SponsorNo.Get(), RepState.SetSponsorInfo,
                new GetDependentsStep(SponsorNo.Get(), RepState.SetDependentsInfo,
                    new CreateResidenceDetailsListStep(RepState.GetDependentsInfo, RepState.SetDependentsResidenceDetails,
                        new CreateEntryDetailsListStep(RepState.GetDependentsInfo, RepState.SetDependentsEntryPermitDetails,
                            new SetReportContentStep(RepState.GetSponsorInfo, RepState.GetDependentsResidenceDetails, RepState.GetDependentsEntryPermitDetails, RepState.SetJsonReport,
                                new SetEmailContentStep(RepState.GetSponsorInfo, RepState.GetDependentsResidenceDetails, RepState.GetDependentsEntryPermitDetails, RepState.SetHtmlReport,
                                    new NullStep()))))));
            return startStep;
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(SponsorNo);
            CheckRequiredInput(EmiratesId);
            CheckRequiredInput(BirthDate);
            CheckRequiredInput(ReportEmailAddress);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }




            await SetOutputValues();
            return StepState = WorkflowStepState.Done;
        }

        private async Task SetOutputValues()
        {
            var sponsorInfo = await ServicesHelper.GetIndividualDetailedInfoBySponsorNo(SponsorNo.Get());
            var nameEn = $"{sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.FirstNameEn} {sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.LastNameEn}";
            var nameAr = $"{sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.FirstNameAr} {sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.LastNameAr}";


            ApplicationDataToSave.Value["ReportDetails"]["ReportData"] = null;
            ApplicationDataToSave.Value["ReportDetails"]["Email"] = ReportEmailAddress.Get();
            ApplicationDataToSave.Value["ApplicantDetails"]["FullNameE"] = nameEn;//sponsorInfo.reportData["NM_E"].ToString();
            ApplicationDataToSave.Value["ApplicantDetails"]["FullNameA"] = nameAr;

            ReportData.Set(null);
            EmailContent.Set(null);
            EmailHeader.Set(null);
        }
    }
}