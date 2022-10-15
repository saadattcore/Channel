using System;
using System.Threading.Tasks;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using Emaratech.Services.Channels.Workflows.Errors;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class BuildEntryPermitApplication : BuildApplication
    {
        public InputParameter<string> PermitNo { get; set; }

        public OutputParameter EntryPermitPPSID { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            EntryPermitPPSID = new OutputParameter(nameof(EntryPermitPPSID));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            RestIndividualUserFormInfo visionData = null;
            if (PermitNo != null && PermitNo.IsFilled() && !string.IsNullOrEmpty(PermitNo?.Get()))
            {
                visionData = await ServicesHelper.GetIndividualProfileByPermitNo(PermitNo.Get());
                if (visionData == null)
                {
                    throw ChannelWorkflowErrorCodes.ResidenceNumberNotFound.ToWebFault($"Profile not found for permit number {PermitNo.Get()}");
                }
                EntryPermitPPSID.Set(visionData.IndividualUserInfo.IndividualPermitInfo?.PPSID);
            }

            await BuildUnifiedApplicationDocument(visionData);
            return StepState = WorkflowStepState.Done;
        }

        protected override async Task<JObject> BuildApplicationDetails(RestIndividualUserFormInfo visionData)
        {
            var application = await base.BuildApplicationDetails(visionData);
            var permit = visionData?.IndividualUserInfo.IndividualPermitInfo;

            application["DepartmentId"] = permit?.EmiratesDepartmentId.ToString();
            application["VisaTypeId"] = permit?.VisaTypeId.ToString();
            application["FileTypeId"] = ServiceType.EntryPermit.GetHashCode().ToString();

            return application;
        }

        protected override async Task<JObject> BuildApplicantDetails(RestIndividualUserFormInfo visionData)
        {
            // ResidenceFileNo is also used for entry permit as EntryPermitFileNo in application API
            var applicant = await base.BuildApplicantDetails(visionData);
            var permit = visionData?.IndividualUserInfo.IndividualPermitInfo;
            applicant["VisaNumber"] = permit?.PermitNo;
            applicant["VisaNumberType"] = Convert.ToString((int) ServiceType.EntryPermit);
            return applicant;
        }

        protected override async Task<JObject> BuildSponsorDetails(RestIndividualUserFormInfo visionData)
        {
            LOG.Debug($"Permit no is {PermitNo?.Get()}");
            var sponsor = await base.BuildSponsorDetails(visionData);
            var permit = visionData?.IndividualUserInfo.IndividualPermitInfo;

            sponsor["SponsorRelationId"] = permit?.RelationshipId.ToString();

            return sponsor;
        }
    }
}