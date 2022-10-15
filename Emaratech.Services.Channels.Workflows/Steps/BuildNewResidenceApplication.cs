using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Vision.Model;
using System.Xml;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Workflows.Engine;
using Emaratech.Services.Channels.Workflows.Errors;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class BuildNewResidenceApplication : BuildApplication
    {
        public InputParameter<string> PermitNo { get; set; }

        public OutputParameter EntryPermitPPSID { get; set; }


        public override void Initialize()
        {
            base.Initialize();
            PPSID = new OutputParameter(nameof(PPSID));
            EntryPermitPPSID = new OutputParameter(nameof(EntryPermitPPSID));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(PermitNo);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            RestIndividualUserFormInfo visionData = null;
            if (PermitNo != null && PermitNo.IsFilled())
            {
                visionData = await ServicesHelper.GetIndividualProfileByPermitNo(PermitNo.Get());
                if (visionData == null)
                {
                    throw ChannelWorkflowErrorCodes.PermitNumberNotFound.ToWebFault($"Profile not found for permit number {PermitNo.Get()}");
                }

                PPSID.Set(visionData.IndividualUserInfo.IndividualPermitInfo?.PPSID);
                EntryPermitPPSID.Set(visionData.IndividualUserInfo.IndividualPermitInfo?.PPSID);
            }

            await BuildUnifiedApplicationDocument(visionData);
            return StepState = WorkflowStepState.Done;
        }

        protected override async Task<JObject> BuildApplicationDetails(RestIndividualUserFormInfo visionData)
        {
            var application = await base.BuildApplicationDetails(visionData);
            var permitInfo = visionData?.IndividualUserInfo.IndividualPermitInfo;
            application["DepartmentId"] = permitInfo?.EmiratesDepartmentId.ToString();
            application["VisaTypeId"] = permitInfo?.VisaTypeId.ToString();
            application["FileTypeId"] = Convert.ToString((int)ServiceType.Residence);
            return application;
        }

        protected override async Task<JObject> BuildApplicantDetails(RestIndividualUserFormInfo visionData)
        {
            var applicant = await base.BuildApplicantDetails(visionData);
            var permitInfo = visionData?.IndividualUserInfo.IndividualPermitInfo;
            applicant["VisaNumber"] = permitInfo?.PermitNo;
            applicant["VisaNumberType"] = Convert.ToString((int)ServiceType.EntryPermit);
            applicant["VisaExpiryDate"] = permitInfo?.PermitExpiryDate.ToString();

            return applicant;
        }

        protected override async Task<JObject> BuildSponsorDetails(RestIndividualUserFormInfo visionData)
        {
            var sponsor = await base.BuildSponsorDetails(visionData);
            var permitInfo = visionData?.IndividualUserInfo.IndividualPermitInfo;
            sponsor["SponsorRelationId"] = permitInfo?.RelationshipId.ToString();
            return sponsor;
        }
    }
}
