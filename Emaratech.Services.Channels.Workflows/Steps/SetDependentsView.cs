using System;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Models.Enums;
using VisaTypeEnum = Emaratech.Services.Channels.Contracts.Rest.Models.Enums.VisaType;
using Emaratech.Services.Channels.Workflows.Models;
using Newtonsoft.Json;
using Emaratech.Services.Lookups.Model;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetDependentsView : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SetDependentsView));

        public OutputParameter View { get; set; }
        public OutputParameter ListTravelledDependents { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> ParentCategoryId { get; set; }
        public InputParameter<string> UserId { get; set; }
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> Platform { get; set; }
        public InputParameter<string> ResidenceNo { get; set; }
        public InputParameter<string> PermitNo { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
            ListTravelledDependents = new OutputParameter(nameof(ListTravelledDependents));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(SponsorNo);
            CheckRequiredInput(ParentCategoryId);
            CheckRequiredInput(UserId);
            CheckRequiredInput(SystemId);
            CheckRequiredInput(Platform);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            View.Set(ViewEnum.Dependents);

            var travelledDependents = getDependentsInfo(SponsorNo.Get());
            ListTravelledDependents.Set(travelledDependents);
            return StepState = WorkflowStepState.Done;
        }

        private async Task<IEnumerable<RestDependentVisaInfo>> getDependentsInfo(string sponsorNo)
        {
            var searchCriteria = new RestDashboardSearchRequest
            {
                ApplicationStatuses = Convert.ToString(ConfigurationManager.AppSettings["StatusForDependentsInProgress"]),
                IsBatchIdMarked = true,
                UserId = UserId.Get(),
                SponsorNo = sponsorNo,
                ServiceId = ServiceId?.Get(),
                ApplicationModule = ApplicationModules.Workflows,
                Platform = Platform.Get()
            };

            if (ParentCategoryId.Get() == Constants.ParentCategories.EntryPermit || ServiceId.Get() == Constants.Services.ResidenceNewService)
                searchCriteria.ServiceType = DashboardSearchType.EntryPermit.GetAttribute<DashboardSearchTypeAttr>().Type;
            else
                searchCriteria.ServiceType = DashboardSearchType.Residence.GetAttribute<DashboardSearchTypeAttr>().Type;

            Log.Debug($"Search criteria to set dependents view is {JsonConvert.SerializeObject(searchCriteria)}");
            var result = await ServicesHelper.GetDashboardDependents(searchCriteria);
            Log.Debug($"Total dependents found are {result.DependentsVisaInfo?.Count}");

            Log.Debug($"Going to filter service ID {ServiceId.Get()} with result {JsonConvert.SerializeObject(result)}");
            return result.DependentsVisaInfo.Where(a => a.ActionsList.Any(p => p.ServiceId == ServiceId.Get() && p.Status == true)).ToList();
        }
    }
}