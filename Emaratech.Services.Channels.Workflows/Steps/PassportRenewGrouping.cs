using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Document.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using log4net;
using System.Linq;
using Emaratech.Services.Channels.Workflows.Errors;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class PassportRenewGrouping : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PassportRenewGrouping));

        public InputParameter<string> Platform { get; set; }
        public InputParameter<string> Action { get; set; }

        public ReferenceParameter<IEnumerable<string>> DocumentIds { get; set; }
        public ReferenceParameter<string> ApplicationData { get; set; }
        public ReferenceParameter<JArray> PassportApplication { get; set; }
        public ReferenceParameter<IEnumerable<RestDocument>> Documents { get; set; }
        public ReferenceParameter<IEnumerable<RestButton>> AdditionalButtons { get; set; }
        public ReferenceParameter<IList<RestListData>> ListData { get; set; }
        public ReferenceParameter<RestSearchInfo> SearchInfo { get; set; }

        public OutputParameter FormConfiguration { get; set; }
        public OutputParameter View { get; set; }
        public OutputParameter Data { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            FormConfiguration = new OutputParameter(nameof(FormConfiguration));
            View = new OutputParameter(nameof(View));
            Data = new OutputParameter(nameof(Data));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            AdditionalButtons.Value = null;
            var isNew = Action.Get()?.ToLower() == "new";
            var isEdit = Action.Get()?.StartsWith("edit") == true;
            var isDelete = Action.Get()?.StartsWith("delete") == true;

            if (!string.IsNullOrEmpty(Action.Get()) && !isNew && !isEdit && !isDelete)
            {
                return StepState = WorkflowStepState.Done;
            }
            StepState = WorkflowStepState.InputRequired;

            // In search, display the corresponding dependant
            var dependantToEdit = GetDependantToEdit(Action.Get());
            if (SearchInfo.Value != null && dependantToEdit == null)
            {
                if (PassportApplication.Value == null)
                {
                    JArray request = await ReusePassportRenewFormData.FindPassportRequest(SearchInfo.Value);
                    if (request != null && request.First != null)
                    {
                        PassportApplication.Value = request;
                    }
                    else
                    {
                        throw ChannelWorkflowErrorCodes.PassportRequestNotFound.ToWebFault($"Passport request not found");
                    }
                }

                dependantToEdit = 
                    FindDependant(PassportApplication.Value, SearchInfo.Value.EmiratesId, SearchInfo.Value.UnifiedNumber)
                    ?? PassportApplication.Value.First;
            }

            // Delete dependant
            if (dependantToEdit != null && isDelete)
            {
                if (PassportApplication.Value.Count == 1)
                {
                    throw ChannelWorkflowErrorCodes.DeletePassportInvalid.ToWebFault($"Cannot delete the only remaining passport request");
                }
                PassportApplication.Value.Remove(dependantToEdit);
                SetListView(PassportApplication.Value);
                return StepState;
            }

            // Display form
            if (ApplicationData.Value == null && DocumentIds.Value == null)
            {
                var formConfig = await ServicesHelper.RenderFormForPlatform(Constants.Forms.PassportRenewFormId, Platform.Get());
                FormConfiguration.Set(formConfig);

                var obj = ReusePassportFormData.SetFormData(dependantToEdit, formConfig);
                var documents = ReusePassportFormData.SetDocuments(dependantToEdit, Documents.Value);

                Data.Set(obj?.ToString(Newtonsoft.Json.Formatting.None));
                Log.Debug(Data.Get());
                Documents.Value = documents;

                View.Set(ViewEnum.Form);
                return StepState;
            }

            var passportEntryList = new JArray();
            if (PassportApplication.Value != null)
            {
                passportEntryList = PassportApplication.Value;
            }

            // Get form data
            if (ApplicationData.Value != null)
            {
                var data = JObject.Parse(ApplicationData.Value);
                var applicantDetails = data["Application"]["ApplicantDetails"];

                var entry = GetPassportFormData.GetDependantData(dependantToEdit, applicantDetails);
                if (dependantToEdit == null)
                {
                    passportEntryList.Add(entry);
                }

                dependantToEdit = entry;
                GetPassportRenewData.SetPassportRenewSpecificData(passportEntryList, dependantToEdit, applicantDetails);

                // Set documents view
                Documents.Value = await ResolveDocumentsPassportRenew.GetDocuments(Documents.Value);
                View.Set(ViewEnum.Documents);
            }

            // Get documents
            if (DocumentIds.Value != null)
            {
                await GetPassportFormData.GetDocumentsData(dependantToEdit == null ? passportEntryList.Last : dependantToEdit, DocumentIds.Value);

                // Set list view
                SetListView(passportEntryList);
                
                // Reset search info, only useful first time to fetch data
                SearchInfo.Value = null;
            }

            PassportApplication.Value = passportEntryList;
            ApplicationData.Value = null;
            DocumentIds.Value = null;
            return StepState;
        }

        private void SetListView(JArray passportEntryList)
        {
            ListData.Value = SetPassportRenewListData.GetListData(passportEntryList);
            View.Set(ViewEnum.List);
            var buttons = GetPassportRenewDependantsButton.GetButtons(PassportApplication.Value);
            if (buttons != null)
            {
                AdditionalButtons.Value = buttons;
            }
        }

        private JToken GetDependantToEdit(string action)
        {
            JToken result = null;
            var arr = action?.Split('-');
            var editing = ListData.Value?.FirstOrDefault(x => x.IsEdit);
            if (editing != null)
            {
                return FindDependant(PassportApplication.Value, editing.EmiratesId, editing.UnifiedNo);
            }

            if (arr != null && arr.Length > 1 && ListData.Value != null)
            {
                foreach (var data in ListData.Value)
                {
                    if (data.Id == arr[1])
                    {
                        data.IsEdit = arr[0].ToLower() == "edit";
                        result = FindDependant(PassportApplication.Value, data.EmiratesId, data.UnifiedNo);
                        break;
                    }
                }
            }
            return result;
        }

        private JToken FindDependant(JArray passportApplication, string emiratesId, string unifiedNo)
        {
            return passportApplication?.FirstOrDefault(x => x["nidNumber"].ToString() == emiratesId && x["udbNumber"].ToString() == unifiedNo);
        }
    };
}