using Emaratech.Services.Workflows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;
using log4net;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Channels.Models.Enums;
using System.Xml;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public abstract class ValidateData : ChannelWorkflowStep
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(ValidateData));

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> ApplicationData { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }
        public OutputParameter BirthDate { get; set; }
        public OutputParameter Relationship { get; set; }
        public OutputParameter PassportExpiryDate { get; set; }
        public OutputParameter YearsOfResidence { get; set; }
        public OutputParameter ApplicationDataToSave { get; set; }
        public OutputParameter CurrentNationality { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            ApplicationDataToSave = new OutputParameter(nameof(ApplicationDataToSave));
            BirthDate = new OutputParameter(nameof(BirthDate));
            Relationship = new OutputParameter(nameof(Relationship));
            PassportExpiryDate = new OutputParameter(nameof(PassportExpiryDate));
            YearsOfResidence = new OutputParameter(nameof(YearsOfResidence));
            CurrentNationality = new OutputParameter(nameof(CurrentNationality));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(ServiceId);
            CheckRequiredInput(ApplicationData);
            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var incomingData = CheckDataFormat(ApplicationData.Get());
            if (string.IsNullOrEmpty(incomingData))
            {
                throw ChannelWorkflowErrorCodes.InvalidApplicationData.ToWebFault($"Application data format is not recognized");
            }

            var matrixId = await ServicesHelper.GetSystemProperty(SystemId.Get(), Constants.SystemProperties.MappingMatrixDataContractIdKey);

            var contractsData = await ServicesHelper.ResolveMappingMatrix(
                matrixId,
                new List<string> { ServiceId.Get() },
                (values, versions) => values[1]);

            var unifiedApp = UnifiedApplication.Get();

            if (contractsData.Any())
            {
                var sourceContract = contractsData.First();
                //sourceContract = "8AF6DB5124774B5E9905D32067255A57";
                //await ServicesHelper.ValidateData(sourceContract, incomingData);
            }

            // Fill specific values
            await CompleteApplication(unifiedApp);
            ApplicationDataToSave.Set(unifiedApp);

            await SetOutputForNextSteps(unifiedApp);
            StepState = WorkflowStepState.Done;
            return StepState;
        }

        protected abstract Task CompleteApplication(JObject unifiedAppDoc);

        private async Task SetOutputForNextSteps(JObject unifiedApp)
        {
            var paramValue = await DataHelper.GetFieldValue(unifiedApp, SystemId.Get(), nameof(BirthDate));
            DateTime date;
            if (DataHelper.TryParseDateTimeExtended(paramValue, out date))
            {
                BirthDate.Set(date);
            }

            paramValue = await DataHelper.GetFieldValue(unifiedApp, SystemId.Get(), nameof(PassportExpiryDate));
            if (DataHelper.TryParseDateTimeExtended(paramValue, out date))
            {
                PassportExpiryDate.Set(date);
            }

            paramValue = await DataHelper.GetFieldValue(unifiedApp, SystemId.Get(), nameof(YearsOfResidence));
            YearsOfResidence.Set(paramValue);

            paramValue= await DataHelper.GetFieldValue(unifiedApp, SystemId.Get(), nameof(CurrentNationality));
            CurrentNationality.Set(paramValue);

            paramValue = await DataHelper.GetFieldValue(unifiedApp, SystemId.Get(), nameof(Relationship));
            if (paramValue != null)
            {
                Relationship.Set(Enum.Parse(typeof(SponsorRelationship), paramValue));
            }
        }
        
        private static string CheckDataFormat(string applicationData)
        {
            var data = applicationData.Trim();

            // If data is JSON, transforms it into XML
            if (data.StartsWith("{"))
            {
                XmlDocument doc = JsonConvert.DeserializeXmlNode(data);
                XmlElement root = doc.DocumentElement;
                ValidateNodes(root.ChildNodes);

                return doc.OuterXml;
            }
            else if (data.StartsWith("<"))
            {
                return data;
            }
            return null;
        }
        
        private static void ValidateNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                ValidateNodes(node.ChildNodes);
                node.InnerText = DataHelper.GetFormattedDate(node.InnerText);
            }
        }
    }
}