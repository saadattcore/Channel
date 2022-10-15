using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Document.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public abstract class ReusePassportFormData : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReusePassportFormData));

        public InputParameter<RestSearchInfo> SearchInfo { get; set; }
        public InputParameter<RestRenderGraph> FormConfiguration { get; set; }
        public ReferenceParameter<JArray> PassportApplication { get; set; }

        public OutputParameter Data { get; set; }
        public OutputParameter Documents { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Data = new OutputParameter(nameof(Data));
            Documents = new OutputParameter(nameof(Documents));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            if (!SearchInfo.IsFilled() && (PassportApplication.Value == null || PassportApplication.Value.First?["applciationRefNumber"] == null))
            {
                return StepState = WorkflowStepState.Done;
            }

            JArray request = PassportApplication.Value;
            if (SearchInfo.IsFilled())
            {
                request = await GetApplicationRequest(SearchInfo.Get());
            }

            if (request != null && request.First != null)
            {
                var obj = SetFormData(request.First, FormConfiguration?.Get());
                var documents = SetDocuments(request.First, null);

                if (PassportApplication.Value == null)
                {
                    PassportApplication.Value = request;
                }

                Data.Set(obj.ToString(Newtonsoft.Json.Formatting.None));
                Log.Debug(Data.Get());

                Documents.Set(documents);
                return StepState = WorkflowStepState.Done;
            }
            throw ChannelWorkflowErrorCodes.PassportRequestNotFound.ToWebFault($"Passport request not found");
        }

        public static List<RestDocument> SetDocuments(JToken dependant, IEnumerable<RestDocument> notSavedDocument)
        {
            if (dependant != null)
            {
                var attachments = (JArray)dependant["txnFileDetailsList"];
                var documents = new List<RestDocument>();

                for (int i = 0; i < attachments.Count; i++)
                {
                    var attachment = attachments[i];
                    var docType = string.Empty;
                    var docMandatory = false;
                    if (attachment["txnFileType"] == null && notSavedDocument != null)
                    {
                        var docData = notSavedDocument.ElementAt(i);
                        if (docData != null)
                        {
                            docType = docData.DocumentTypeId;
                            docMandatory = docData.Required.Value;
                        }
                    }
                    else if (attachment["txnFileType"] != null)
                    {
                        docType = attachment["txnFileType"]["fileTypeId"].ToString();
                        docMandatory = attachment["txnFileType"]["fileMandatory"].ToString() == "1";
                    }

                    if (!string.IsNullOrEmpty(docType))
                    {
                        documents.Add(new RestDocument
                        {
                            DocumentId = attachment["txnFileId"].ToString(),
                            DocumentTypeId = docType,
                            Required = docMandatory,
                            ResourceKey = GetPassportDocumentResourceKey(Convert.ToInt32(docType))
                        });
                    }
                }
                return documents;
            }
            return null;
        }

        public static JObject SetFormData(JToken applicant, RestRenderGraph formConfiguration)
        {
            if (applicant != null)
            {
                var fieldsByEntity = DataHelper.GetFormFieldByEntity(formConfiguration);

                var obj = new JObject();
                obj["ApplicantDetails"] = new JObject();
                obj["ApplicationDetails"] = new JObject();
                obj["SponsorDetails"] = new JObject();

                var applicantDetails = obj["ApplicantDetails"];

                // Second form
                if (fieldsByEntity.ContainsKey("ApplicationDetails") &&
                    fieldsByEntity["ApplicationDetails"].Any(x => x.Name == "ResidencyPickup"))
                {
                    applicantDetails["FullName"] = applicant["requesterName"];
                    applicantDetails["MobileNo"] = applicant["reqMobNumber"]?.ToString().Insert(0, "0").Insert(3, "-");
                    obj["SponsorDetails"]["SponsorEmail"] = applicant["reqEmailId"];

                    if (applicant["procModeId"] != null && applicant["procModeId"]["pmodeId"].ToString() == "1")
                    {
                        obj["ApplicationDetails"]["ResidencyPickup"] = Constants.PickupLookup.ZajelDelivery;
                        applicantDetails["Street"] = applicant["courierPickupAddress"];
                        applicantDetails["NearestLandmark"] = applicant["courierPickupLandMark"];
                        applicantDetails["POBox"] = applicant["courierPickupPoNumber"];
                        applicantDetails["CityId"] = applicant["couierPickupCity"]["ID"];
                        applicantDetails["EmirateId"] = applicant["courierPickupEmirate"]["ID"];
                    }
                    else
                    {
                        obj["ApplicationDetails"]["ResidencyPickup"] = Constants.PickupLookup.SelfCollection;
                    }
                }

                applicantDetails["PassportFullName"] = applicant["fullNameEn"];
                applicantDetails["EmiratesIdNo"] = applicant["nidNumber"];
                applicantDetails["UnifiedNo"] = applicant["udbNumber"];
                return obj;
            }
            return null;
        }

        public static string GetPassportDocumentResourceKey(int fileTypeId)
        {
            switch (fileTypeId)
            {
                case 1: return "EmiratesId";
                case 2: return "FamilyBook";
                case 3: return "PopulationRegistrationForm";
                case 4: return "PersonalPhoto";
                default: return string.Empty;
            }
        }

        protected abstract void SetSpecificFields(JObject formData, JArray passportApplication, Dictionary<string, List<FormField>> formConfiguration);
        protected abstract Task<JArray> GetApplicationRequest(RestSearchInfo searchInfo);
    }
}