using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Steps.Report;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Workflows.Steps.Processing;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{

    public class CreateEstablishmentSasReport : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreateEstablishmentSasReport));
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> EstablishmentCode { get; set; }
        public InputParameter<string> ReportEmailAddress { get; set; }
        public ReferenceParameter<JObject> ApplicationDataToSave { get; set; }
        public OutputParameter ReportData { get; }
        public OutputParameter EmailContent { get; }
        public OutputParameter EmailHeader { get; }

        public CreateEstablishmentSasReport()
        {
            
            ReportData = new OutputParameter(nameof(ReportData));
            EmailContent = new OutputParameter(nameof(EmailContent));
            EmailHeader = new OutputParameter(nameof(EmailHeader));
        }

        private ProcessingStep AssembleProcessAndReturnFirstStep()
        {
            var RepState = new ReportState();
            ProcessingStep startStep = new BuildEstablishmentInfoReportStep(EstablishmentCode.Get(), RepState.SetSponsorInfo,
                new GetEstablishmentDependentStep(EstablishmentCode.Get(), RepState.SetDependentsInfo,
                    new CreateResidenceDetailsListStep(RepState.GetDependentsInfo, RepState.SetDependentsResidenceDetails,
                        new CreateEntryDetailsListStep(RepState.GetDependentsInfo, RepState.SetDependentsEntryPermitDetails,
                            new SetReportContentStep(RepState.GetSponsorInfo, RepState.GetDependentsResidenceDetails, RepState.GetDependentsEntryPermitDetails, RepState.SetJsonReport,
                                new SetEmailContentStep(RepState.GetSponsorInfo, RepState.GetDependentsResidenceDetails, RepState.GetDependentsEntryPermitDetails, RepState.SetHtmlReport,
                                    new NullStep()))))));
            return startStep;
        }

        public override async Task<WorkflowStepState> Execute()
        {

            Log.Debug("Report generation start");
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(ReportEmailAddress);
            CheckRequiredInput(EstablishmentCode);

            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }


            Log.Debug("Report generation end");
            await SetOutputValues();
            return StepState = WorkflowStepState.Done;
        }

        private async Task SetOutputValues()
        {
            ApplicationDataToSave.Value["ReportDetails"]["ReportData"] = null;
            ApplicationDataToSave.Value["ReportDetails"]["Email"] = ReportEmailAddress.Get();
            ApplicationDataToSave.Value["ReportDetails"]["EstCode"] = EstablishmentCode.Get();

            var establishmentInfo = await ServicesHelper.GetEstablishmentProfile(EstablishmentCode.Get());

            var estabNameEn = establishmentInfo.EstabNameEn;
            var estabNameAr = establishmentInfo.EstabNameAr;

            ApplicationDataToSave.Value["ApplicantDetails"]["FullNameE"] =estabNameEn;//sponsorInfo.reportData["NM_E"].ToString();
            ApplicationDataToSave.Value["ApplicantDetails"]["FullNameA"] = estabNameAr;
        

            /*    Log.Debug($"Report Data for Sponsor and sponsored Report for establishment ==>  {RepState.JsonReport}")*/
            ;
            ReportData.Set(null);
            EmailContent.Set(null);
            EmailHeader.Set(null);
        }
    }
}
