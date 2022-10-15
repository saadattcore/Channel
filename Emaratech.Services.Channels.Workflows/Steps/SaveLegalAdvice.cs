using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Document.Model;
using System;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SaveLegalAdvice : ChannelWorkflowStep
    {
        public InputParameter<string> ApplicationData { get; set; }
        public InputParameter<string> Action { get; set; }
        public InputParameter<IEnumerable<string>> DocumentIds { get; set; }
        public InputParameter<JObject> LegalAdvice { get; set; }

        public ReferenceParameter<string> ApplicationId { get; set; }
        public ReferenceParameter<string> LegalRequestId { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(ApplicationData);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var data = JObject.Parse(ApplicationData.Get());
            var applicantDetails = data["Application"]["ApplicantDetails"];
            var request = new JObject();
            var legalAdvice = new JObject();

            // We are in update
            if (LegalAdvice.IsFilled())
            {
                await SyncDocuments();

                legalAdvice.Merge(LegalAdvice.Get()["legalRequest"]);

                if (Action.IsFilled() && Action.Get()?.ToLower() == "submit")
                    request["requestMode"] = "UPDATE_SUBMIT";
                else
                    request["requestMode"] = "UPDATE";
            }
            else
            {
                if (Action.IsFilled() && Action.Get()?.ToLower() == "submit")
                    request["requestMode"] = "SUBMIT";
                else
                    request["requestMode"] = "SAVE";
            }

            legalAdvice["reqName"] = applicantDetails["FullName"];
            legalAdvice["gender"] = applicantDetails["SexId"].ToString() == "1" ? "MALE" : "FEMALE";
            legalAdvice["dob"] = DataHelper.GetFormattedDate(applicantDetails["BirthDate"].ToString());
            legalAdvice["passport"] = applicantDetails["PassportNo"];
            legalAdvice["mobile"] = applicantDetails["AddressOutsideTelNo"];
            legalAdvice["email"] = data["Application"]["SponsorDetails"]["SponsorEmail"].ToString().ToLower();
            legalAdvice["adviceSubject"] = data["Application"]["ApplicationDetails"]["Comment"];
            legalAdvice["acceptLang"] = applicantDetails["PreferredLanguage"].ToString() == Constants.LanguagesLookup.ArabicLanguage ? "ar" : "en";

            var adviceTypes = await ServicesHelper.GetLegalAdviceAdviceTypes();
            var adviceType = adviceTypes.FirstOrDefault(x => x.AdviceTypeId == data["Application"]["ApplicationDetails"]["AdviceType"].ToString());

            var nationalities = await ServicesHelper.GetLegalAdviceNationalities();
            var nationality = nationalities.FirstOrDefault(x => x.NationalityId == applicantDetails["NationalityId"].ToString());

            var applicantTypes = await ServicesHelper.GetLegalAdviceApplicantTypes();
            var applicantType = applicantTypes.FirstOrDefault(x => x.ApplicantTypeId == applicantDetails["ApplicantType"].ToString());

            legalAdvice["adviceType"] = JObject.FromObject(adviceType);
            legalAdvice["nationality"] = JObject.FromObject(nationality);
            legalAdvice["applicantType"] = JObject.FromObject(applicantType);

            request["requestAdvice"] = legalAdvice;

            if (DocumentIds.IsFilled())
            {
                var attachments = new List<JObject>();
                foreach (var docId in DocumentIds.Get())
                {
                    var attachment = new JObject();
                    attachment["attachmentId"] = docId;
                    attachments.Add(attachment);
                }
                request["attachments"] = JArray.FromObject(attachments);
            }
            else
            {
                request["attachments"] = JArray.FromObject(new string[0]);
            }

            if (Action.IsFilled())
            {
                // Remove this properties which are not accepted by the api for update
                legalAdvice.Remove("modifiedDate");
                legalAdvice.Remove("adviceStatus");

                var reponse = await ServicesHelper.SaveLegalAdvice(request);
                ApplicationId.Value = reponse["adviceNumber"].ToString();
                LegalRequestId.Value = reponse["legalReqId"].ToString();
            }

            StepState = WorkflowStepState.Done;
            return StepState;
        }

        private async Task SyncDocuments()
        {
            var existingDocuments = (JArray)LegalAdvice.Get()["attachments"];
            var currentLegalAdviceResponse = await ServicesHelper.FetchLegalAdvice(Convert.ToString(LegalAdvice.Get()["legalRequest"]["adviceNumber"]));
            foreach (var existingDocument in existingDocuments)
            {
                var oldDocumentId = existingDocument["attachmentId"].ToString();
                if (!DocumentIds.IsFilled() ||
                    !DocumentIds.Get().Contains(oldDocumentId))
                {
                    if (isDocumentExist(currentLegalAdviceResponse, oldDocumentId)) //Only try to delete document if it exists in smart amer otherwise don't
                        await ServicesHelper.DeleteLegalAdviceDocument(oldDocumentId);
                }
            }
        }
        private bool isDocumentExist(JObject request, string documentId)
        {
            bool response = false;
            if (!string.IsNullOrEmpty(documentId))
            {
                JArray attachments = (JArray)request["attachments"];
                foreach (var attachment in attachments)
                    if (Convert.ToString(attachment["attachmentId"]) == documentId)
                        response = true;
            }
            return response;
        }
    };
}