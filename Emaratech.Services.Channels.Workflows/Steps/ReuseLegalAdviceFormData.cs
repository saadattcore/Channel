using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Document.Model;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ReuseLegalAdviceFormData : ChannelWorkflowStep
    {
        public InputParameter<string> LegalAdviceNumber { get; set; }

        public OutputParameter Data { get; set; }
        public OutputParameter Documents { get; set; }
        public OutputParameter LegalAdvice { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Data = new OutputParameter(nameof(Data));
            Documents = new OutputParameter(nameof(Documents));
            LegalAdvice = new OutputParameter(nameof(LegalAdvice));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            if (!LegalAdviceNumber.IsFilled())
            {
                return StepState = WorkflowStepState.Done;
            }
            
            var response = await ServicesHelper.FetchLegalAdvice(LegalAdviceNumber.Get());
            var legalAdvice = response["legalRequest"];
            if (legalAdvice != null)
            {
                var obj = new JObject();
                obj["ApplicantDetails"] = new JObject();
                obj["ApplicationDetails"] = new JObject();
                obj["SponsorDetails"] = new JObject();

                var applicantDetails = obj["ApplicantDetails"];
                applicantDetails["FullName"] = legalAdvice["reqName"];
                applicantDetails["SexId"] = legalAdvice["gender"].ToString().ToLower() == "male" ? "1" : "2";
                applicantDetails["BirthDate"] = DataHelper.GetFormattedDate(legalAdvice["dob"].ToString());
                applicantDetails["PassportNo"] = legalAdvice["passport"];
                applicantDetails["AddressOutsideTelNo"] = legalAdvice["mobile"];
                applicantDetails["PreferredLanguage"] = legalAdvice["acceptLang"].ToString().ToLower() == "ar" ? Constants.LanguagesLookup.ArabicLanguage : Constants.LanguagesLookup.EnglishLanguage;
                applicantDetails["NationalityId"] = legalAdvice["nationality"]["nationalityId"];
                applicantDetails["ApplicantType"] = legalAdvice["applicantType"]["applicantTypeId"];

                obj["SponsorDetails"]["SponsorEmail"] = legalAdvice["email"];
                obj["ApplicationDetails"]["Comment"] = legalAdvice["adviceSubject"];
                obj["ApplicationDetails"]["AdviceType"] = legalAdvice["adviceType"]["adviceTypeId"];

                var attachments = (JArray)response["attachments"];
                var documents = new List<RestDocument>();
                foreach (var attachment in attachments)
                {
                    documents.Add(new RestDocument
                    {
                        DocumentId = attachment["attachmentId"].ToString(),
                        DocumentTypeId = attachment["attachmentName"].ToString(),
                        Name = attachment["attachmentName"].ToString()
                    });
                }

                Data.Set(obj.ToString(Newtonsoft.Json.Formatting.None));
                Documents.Set(documents);
                LegalAdvice.Set(response);
                return StepState = WorkflowStepState.Done;
            }
            throw ChannelWorkflowErrorCodes.LegalAdviceNotFound.ToWebFault($"Legal advice not found");
        }
    }
}