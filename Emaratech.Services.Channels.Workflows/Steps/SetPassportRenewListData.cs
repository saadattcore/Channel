using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Models;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetPassportRenewListData : ChannelWorkflowStep
    {
        public InputParameter<JArray> PassportApplication { get; set; }

        public OutputParameter ListData { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            ListData = new OutputParameter(nameof(ListData));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(PassportApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }
            
            ListData.Set(GetListData(PassportApplication.Get()));
            return StepState = WorkflowStepState.Done;
        }

        public static List<RestListData> GetListData(JArray passportEntries)
        {
            var list = new List<RestListData>();
            foreach (var entry in passportEntries)
            {
                var id = entry["unifiedId"]?.ToString();
                DateTime date;
                DataHelper.TryParseDateTimeExtended(entry["dob"]?.ToString(), out date);
                var data = new RestListData
                {
                    Id = id ?? Guid.NewGuid().ToString("N"),
                    Name = new RestName
                    {
                        Ar = entry["fullNameAr"].ToString(),
                        En = entry["fullNameEn"].ToString()
                    },
                    BirthDate = date,
                    UnifiedNo = entry["udbNumber"]?.ToString(),
                    EmiratesId = entry["nidNumber"]?.ToString()
                };

                if (string.IsNullOrEmpty(id))
                {
                    entry["unifiedId"] = data.Id;
                }

                list.Add(data);
            }

            return list;
        }
    }
}