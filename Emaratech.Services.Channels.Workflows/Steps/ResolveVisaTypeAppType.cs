using System.Linq;
using Emaratech.Services.Workflows.Engine;
using Emaratech.Services.MappingMatrix.Model;
using Emaratech.Services.Channels.Workflows.Models;
using System.Collections.Generic;
using Emaratech.Services.Channels.Workflows.Errors;
using System.Threading.Tasks;
using Emaratech.Utilities;
using log4net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    //STEP ID = 045A306425A44EF2BAF9BCF6DE8C3D3D
    public class ResolveVisaTypeAppType : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ResolveVisaTypeAppType));

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> SponsorType { get; set; }
        public InputParameter<string> SponsorSponsorType { get; set; }
        public InputParameter<string> EstablishmentType { get; set; }
        public InputParameter<JObject> UnifiedApplication { get; set; }

        public OutputParameter VisaType { get; set; }
        public OutputParameter AppType { get; set; }
        public OutputParameter AppSubType { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            VisaType = new OutputParameter(nameof(VisaType));
            AppType = new OutputParameter(nameof(AppType));
            AppSubType = new OutputParameter(nameof(AppSubType));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(SystemId);
            CheckRequiredInput(ServiceId);
            CheckRequiredInput(UnifiedApplication);
            if (ParametersRequiringInput.Count > 1)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var serviceMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "ServiceMatrix");
            if (string.IsNullOrEmpty(serviceMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidServiceConfiguration.ToWebFault($"ServiceMatrix not configured in system");
            }
            var application = UnifiedApplication.Get();
            var nationalityId = application["ApplicantDetails"]["CurrentNationalityId"].Value<string>();
            var criteria = new List<SearchVersion>
            {
                new SearchVersion("Service", ServiceId?.Get()),
                new SearchVersion(nameof(UserType), UserType?.Get()),
                new SearchVersion(nameof(SponsorType), SponsorSponsorType?.Get()),
                new SearchVersion(nameof(EstablishmentType), EstablishmentType?.Get()),
                new SearchVersion("NationalityId", nationalityId)
            };

            var parameters = await DataHelper.GetMatrixParameters(UnifiedApplication?.Get(), serviceMappingMatrix, SystemId?.Get());
            foreach (var kvp in parameters)
            {
                // If there is any property with the same key, ignore this parameter
                // Input param should be hardcoded and output param is the mapping matrix output, not a search criteria
                if (!GetType().GetProperties().Any(p => p.Name == kvp.Key && p.PropertyType != typeof(OutputParameter)))
                {
                    criteria.Add(new SearchVersion(kvp.Key, kvp.Value));
                }
            }

            Log.Debug($"Resolving visa type/app type with parameters {criteria.ToJsonString()}");

            var items = await ServicesHelper.ResolveMappingMatrix(
                serviceMappingMatrix,
                criteria,
                (values, versions) => new VisaApplication(values[0], values[1], values[2], values[3]));

            // There can not be multiples services with the same visa type and visa app type
            if (items.Count() != 1)
            {
                Log.Error($"Visa Type not resolved Mapping Matrix: {serviceMappingMatrix}; Parameters = {string.Join(",", criteria.Select(x => x.Name + "-" + x.Value))}");
                throw ChannelWorkflowErrorCodes.ServiceNotResolved.ToWebFault("You are not allowed to create this service.");
            }
            else
            {
                var visaApp = items.First();
                VisaType.Set(visaApp.VisaType);
                AppType.Set(visaApp.AppType);
                AppSubType.Set(visaApp.AppSubType);
                Log.Debug($"Visa Type: {VisaType.Get()} - App Type: {AppType.Get()} - App SubType: {AppSubType.Get()}");
                
                StepState = WorkflowStepState.Done;
            }
            return StepState;
        }
    }
}