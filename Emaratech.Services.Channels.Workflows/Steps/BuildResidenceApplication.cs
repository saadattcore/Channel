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
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class BuildResidenceApplication : BuildApplication
    {
        public InputParameter<string> ResidenceNo { get; set; }

        public OutputParameter ResidencePPSID { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            ResidencePPSID = new OutputParameter(nameof(ResidencePPSID));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            CheckRequiredInput(ResidenceNo);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var visionData = await ServicesHelper.GetIndividualProfileByResidenceNo(ResidenceNo.Get());
            if (visionData == null)
            {
                throw ChannelWorkflowErrorCodes.ResidenceNumberNotFound.ToWebFault($"Profile not found for residence number {ResidenceNo.Get()}");
            }

            await BuildUnifiedApplicationDocument(visionData);
            ResidencePPSID.Set(visionData.IndividualUserInfo.IndividualResidenceInfo?.PPSID);
            return StepState = WorkflowStepState.Done;
        }

        protected override async Task<JObject> BuildApplicationDetails(RestIndividualUserFormInfo visionData)
        {
            var application = await base.BuildApplicationDetails(visionData);
            var residence = visionData?.IndividualUserInfo.IndividualResidenceInfo;
            application["DepartmentId"] = residence?.EmiratesDepartmentId.ToString();
            application["ResidenceType"] = residence?.ResidenceTypeId.ToString();
            application["VisaTypeId"] = residence?.VisaTypeId.ToString();
            application["FileTypeId"] = Convert.ToString((int)ServiceType.Residence);
            return application;
        }

        protected override async Task<JObject> BuildApplicantDetails(RestIndividualUserFormInfo visionData)
        {
            var applicant = await base.BuildApplicantDetails(visionData);
            var residence = visionData?.IndividualUserInfo.IndividualResidenceInfo;
            applicant["VisaNumber"] = residence.ResidenceNo;
            applicant["VisaNumberNew"] = residence.ResidenceNo;
            applicant["VisaNumberType"] = Convert.ToString((int)ServiceType.Residence);
            applicant["VisaExpiryDate"] = residence.ResidenceExpiryDate.ToString();

            // To be removed: should use a editable field in Residence Renew form
            if (ServiceId.Get() == Constants.Services.ResidenceRenew)
            {
                applicant.Add("ResidenceRequestYear", null);
            }
            else
            {
                int yearsOfResidence = Convert.ToInt32(residence.ResidenceExpiryDate?.Year - residence.ResidenceIssueDate?.Year);
                applicant.Add("ResidenceRequestYear", yearsOfResidence.ToString());
            }
            return applicant;
        }

        protected override async Task<JObject> BuildSponsorDetails(RestIndividualUserFormInfo visionData)
        {
            var sponsor = await base.BuildSponsorDetails(visionData);
            var residence = visionData?.IndividualUserInfo.IndividualResidenceInfo;
            sponsor["SponsorRelationId"] = residence?.RelationshipId.ToString();
            return sponsor;
        }
    }
}
