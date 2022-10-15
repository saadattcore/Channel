using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using Emaratech.Services.Channels.Models.Enums;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public abstract class GetPassportFormData : ChannelWorkflowStep
    {
        protected JToken applicantDetails = null;
        protected JToken applicationDetails = null;
        protected JToken sponsorDetails = null;

        public ReferenceParameter<IEnumerable<string>> DocumentIds { get; set; }
        public ReferenceParameter<string> ApplicationData { get; set; }
        public ReferenceParameter<JArray> PassportApplication { get; set; }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(ApplicationData);
            CheckRequiredInput(DocumentIds);
            if (ParametersRequiringInput.Count > 1)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var passportEntryList = new JArray();
            if (PassportApplication.Value != null)
            {
                passportEntryList = PassportApplication.Value;
            }

            if (ApplicationData.Value != null)
            {
                var data = JObject.Parse(ApplicationData.Value);
                applicantDetails = data["Application"]["ApplicantDetails"];
                sponsorDetails = data["Application"]["SponsorDetails"];
                applicationDetails = data["Application"]["ApplicationDetails"];
            }

            var passportData = GetDependantData(null, applicantDetails);
            if (passportEntryList.Count == 0)
            {
                passportEntryList.Add(JObject.FromObject(passportData));
            }

            await GetDocumentsData(passportEntryList.First, DocumentIds.Value);

            SetSpecificFields(passportEntryList, 0);

            // Set values for requester info & delivery address
            if (applicantDetails != null && applicationDetails != null &&
                applicationDetails["ResidencyPickup"] != null)
            {
                foreach (var entry in passportEntryList)
                {
                    entry["requesterName"] = applicantDetails["FullName"];
                    entry["reqEmailId"] = sponsorDetails["SponsorEmail"].ToString().ToLower();
                    entry["reqMobNumber"] = applicantDetails["MobileNo"].ToString().Replace("-", string.Empty);

                    var pickupValue = applicationDetails["ResidencyPickup"].ToString();
                    if (pickupValue == Constants.PickupLookup.ZajelDelivery)
                    {
                        entry["courierPickupAddress"] = applicantDetails["Street"];
                        entry["courierPickupLandMark"] = applicantDetails["NearestLandmark"];
                        entry["courierPickupPoNumber"] = applicantDetails["POBox"];
                        entry["couierPickupCity"] = JObject.FromObject(new
                        {
                            ID = applicantDetails["CityId"]
                        });
                        entry["courierPickupEmirate"] = JObject.FromObject(new
                        {
                            ID = applicantDetails["EmirateId"]
                        });
                        entry["procModeId"] = JObject.FromObject(new
                        {
                            pmodeId = 1
                        });
                    }
                    else
                    {
                        entry["procModeId"] = JObject.FromObject(new
                        {
                            pmodeId = 2
                        });
                    }
                }
            }
            
            PassportApplication.Value = passportEntryList;
            ApplicationData.Value = null;
            DocumentIds.Value = null;
            StepState = WorkflowStepState.Done;
            return StepState;
        }

        public static async Task GetDocumentsData(JToken dependant, IEnumerable<string> documentIds)
        {
            if (documentIds != null)
            {
                await SyncDocuments(dependant, documentIds);

                var attachments = new List<JObject>();
                foreach (var docId in documentIds)
                {
                    var attachment = new JObject();
                    attachment["txnFileId"] = docId;
                    attachments.Add(attachment);
                }
                dependant["txnFileDetailsList"] = JArray.FromObject(attachments);
            }
        }
        
        public static JToken GetDependantData(JToken dependantToEdit, JToken applicantDetails)
        {
            var passportData = new JObject();
            if (dependantToEdit != null)
            {
                passportData = (JObject)dependantToEdit;
            }
            
            if (applicantDetails != null && applicantDetails["PassportFullName"] != null)
            {
                passportData["fullNameAr"] = applicantDetails["PassportFullName"]; //$"{applicantDetails["FirstNameA"]} {applicantDetails["MiddleNameA"]} {applicantDetails["LastNameA"]}";
                passportData["fullNameEn"] = applicantDetails["PassportFullName"];//$"{applicantDetails["FirstNameE"]} {applicantDetails["MiddleNameE"]} {applicantDetails["LastNameE"]}";

                passportData["issuingEmirate"] = JObject.FromObject(new
                {
                    ID = 1//applicantDetails["PassportIssuingEmirate"]
                });

                //var emirates = ServicesHelper.GetPassportEmiratesLookup();
            }
            return passportData;
        }

        public static async Task SyncDocuments(JToken passportData, IEnumerable<string> documentIds)
        {
            if (passportData["txnFileDetailsList"] != null)
            {
                var existingDocuments = (JArray)passportData["txnFileDetailsList"];
                foreach (var existingDocument in existingDocuments)
                {
                    var oldDocumentId = existingDocument["txnFileId"].ToString();
                    if (documentIds.All(x => x != oldDocumentId))
                    {
                        await ServicesHelper.DeletePassportDocument(oldDocumentId);
                    }
                }
            }
        }

        protected abstract void SetSpecificFields(JArray passportApplication, int index);
    };
}