using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emaratech.Services.Channels;
using System.Threading.Tasks;
using VisaTypeEnum = Emaratech.Services.Channels.Contracts.Rest.Models.Enums.VisaType;
using Emaratech.Services.Channels.Workflows.Models;
using Newtonsoft.Json;
using Emaratech.Services.Lookups.Model;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetTransferResidenceDependentsView : ChannelWorkflowStep
    {
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

            //var jsonData = JsonConvert.SerializeObject(travelledDependents);
            ListTravelledDependents.Set(travelledDependents);

            return StepState = WorkflowStepState.Done;
        }

        private async Task<IEnumerable<RestDependentVisaInfo>> getDependentsInfo(string sponsorNo)
        {
            JObject travelledDependents = new JObject();

            var dependents = await ServicesHelper.GetIndividualDependentsInformation(sponsorNo);
            //dependents.IndividualDependents[0].IndividualVisaInformation.VisaType
          
            var resDependents = dependents.IndividualDependents?.Where(p => 
                p.IndividualVisaInformation.VisaType == VisaTypeEnum.Residence.GetAttribute<VisaTypeAttr>().Type && 
               (p.IndividualVisaInformation.VisaStatusId == (int)ResidenceStatusType.Issued) &&
               (p.IndividualVisaInformation.VisaExpiryDate.Value.Date > DateTime.Now.Date)).ToList();

            
            string visaTypeLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), WorkflowConstants.LookupVisaType);

            string visaStatusLookupId = await ServicesHelper.GetSystemProperty(SystemId.Get(), WorkflowConstants.LookupVisaStatus);

            var lookupItemsVisaType = await ServicesHelper.GetLookupItems(visaTypeLookupId);
            
            if (resDependents?.Count() > 0)
            {
                var mappedDependentsRes = mappObject(resDependents,lookupItemsVisaType);//ServicesHelper.ToModel<List<RestDependentVisaInfo>>(resDependents);
                return mappedDependentsRes;
            }
            return null;
        }
        private IEnumerable<RestDependentVisaInfo> mappObject(List<RestIndividualDependentsInfoWrapper> dependentList, IEnumerable<RestLookupDetail> lookupItemsVisaType)
        {
            List<RestDependentVisaInfo> dependentInfo = new List<RestDependentVisaInfo>();
            foreach (var dependentListVisa in dependentList)
            {
                var visaInfo = new RestDependentVisaInfo();
                visaInfo.ExpiryDate = dependentListVisa.IndividualVisaInformation.VisaExpiryDate;

                var visaType = lookupItemsVisaType.FirstOrDefault(p => p.Col1 == dependentListVisa.IndividualVisaInformation?.VisaType).ItemId;
                visaInfo.VisaType = visaType;//dependentListVisa.IndividualVisaInformation?.VisaType;

                visaInfo.VisaTypeId = dependentListVisa?.IndividualVisaInformation?.VisaTypeId.ToString();
                visaInfo.VisaNumber = dependentListVisa.IndividualVisaInformation?.VisaNumber;
                visaInfo.ExpiryDate = dependentListVisa.IndividualVisaInformation?.VisaExpiryDate;
                visaInfo.FirstName = new Contracts.Rest.Models.RestName();
                visaInfo.LastName = new Contracts.Rest.Models.RestName();
                visaInfo.FirstName.En = dependentListVisa?.IndividualProfileInformation?.LastNameEn.ToString();
                visaInfo.FirstName.Ar = dependentListVisa?.IndividualProfileInformation?.FirstNameAr.ToString();

                visaInfo.LastName.En = dependentListVisa?.IndividualProfileInformation?.LastNameEn.ToString();
                visaInfo.LastName.Ar = dependentListVisa?.IndividualProfileInformation?.LastNameAr.ToString();
                visaInfo.Nationality = dependentListVisa?.IndividualProfileInformation?.NationalityId.ToString();
                visaInfo.Relationship = dependentListVisa?.IndividualVisaInformation?.RelationshipId.ToString();

                //var visaStatus = lookupItemsVisaStatus.FirstOrDefault(p => p.Col1 == dependentListVisa.IndividualVisaInformation?.VisaStatusId.ToString()).ItemId;
                visaInfo.Status = visaType;//Guid.NewGuid().ToString();//dependentListVisa?.IndividualVisaInformation.VisaStatusId.ToString();
                dependentInfo.Add(visaInfo);
            }

            return dependentInfo;
        }
    }


}
