using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisaTypeEnum = Emaratech.Services.Channels.Contracts.Rest.Models.Enums.VisaType;
using Emaratech.Services.Channels.Workflows.Models;
using Newtonsoft.Json;
using Emaratech.Services.Lookups.Model;
using log4net;


namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetReportDependentsView : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SetDependentsView));

        public OutputParameter View { get; set; }
        public OutputParameter ListTravelledDependents { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> SystemId { get; set; }

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
            CheckRequiredInput(SystemId);
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
            JObject travelledDependents = new JObject();

            var dependents = await ServicesHelper.GetIndividualDependentsInformation(sponsorNo);

            var resDependents = dependents.IndividualDependents?.Where(p =>
           (p.IndividualVisaInformation.VisaType == VisaTypeEnum.Residence.GetAttribute<VisaTypeAttr>().Type) &&
           (p.IndividualVisaInformation.VisaStatusId == (int)ResidenceStatusType.Issued)).ToList();

            var visaDependents = dependents.IndividualDependents?.Where(p =>
            (p.IndividualVisaInformation.VisaType == VisaTypeEnum.EntryPermit.GetAttribute<VisaTypeAttr>().Type) &&
            (p.IndividualVisaInformation.VisaStatusId == (int)PermitStatusType.Used)).ToList();

            string visaTypeLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), WorkflowConstants.LookupVisaType);
            var lookupItemsVisaType = await ServicesHelper.GetLookupItems(visaTypeLookupId);
            List<RestDependentVisaInfo> dependentsList = new List<RestDependentVisaInfo>();

            if (resDependents != null && resDependents.Any())
            {
                dependentsList.AddRange(mappObject(resDependents, lookupItemsVisaType));
            }

            if (visaDependents != null && visaDependents.Any())
            {
                dependentsList.AddRange(mappObject(visaDependents, lookupItemsVisaType));
            }

            return dependentsList.AsEnumerable();
        }

        private IEnumerable<RestDependentVisaInfo> mappObject(List<RestIndividualDependentsInfoWrapper> dependentList, IEnumerable<RestLookupDetail> lookupItemsVisaType)
        {
            List<RestDependentVisaInfo> dependentInfo = new List<RestDependentVisaInfo>();
            foreach (var dependentListVisa in dependentList)
            {
                var visaInfo = new RestDependentVisaInfo();
                visaInfo.ExpiryDate = dependentListVisa.IndividualVisaInformation.VisaExpiryDate;

                var visaType = lookupItemsVisaType.FirstOrDefault(p => p.Col1 == dependentListVisa.IndividualVisaInformation?.VisaType).ItemId;
                visaInfo.VisaType = visaType;

                visaInfo.VisaTypeId = dependentListVisa?.IndividualVisaInformation?.VisaTypeId.ToString();
                visaInfo.VisaNumber = dependentListVisa.IndividualVisaInformation?.VisaNumber;
                visaInfo.ExpiryDate = dependentListVisa.IndividualVisaInformation?.VisaExpiryDate;
                visaInfo.FirstName = new Contracts.Rest.Models.RestName();
                visaInfo.LastName = new Contracts.Rest.Models.RestName();
                visaInfo.FirstName.En = dependentListVisa?.IndividualProfileInformation?.FirstNameEn.ToString();
                visaInfo.FirstName.Ar = dependentListVisa?.IndividualProfileInformation?.FirstNameAr.ToString();

                visaInfo.LastName.En = dependentListVisa?.IndividualProfileInformation?.LastNameEn.ToString();
                visaInfo.LastName.Ar = dependentListVisa?.IndividualProfileInformation?.LastNameAr.ToString();
                visaInfo.Nationality = dependentListVisa?.IndividualProfileInformation?.NationalityId.ToString();
                visaInfo.Relationship = dependentListVisa?.IndividualVisaInformation?.RelationshipId.ToString();

                visaInfo.Status = visaType;
                dependentInfo.Add(visaInfo);
            }
            return dependentInfo;
        }
    }
}