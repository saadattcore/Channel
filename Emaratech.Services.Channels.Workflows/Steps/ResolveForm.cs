using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Errors;
using Emaratech.Services.Forms.Model;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ResolveForm : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ResolveForm));

        protected int matrixColumnNumber;

        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> CategoryId { get; set; }
        public InputParameter<string> Platform { get; set; }
        public InputParameter<string> UserType { get; set; }

        public OutputParameter FormId { get; set; }
        public OutputParameter FormConfiguration { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            matrixColumnNumber = 1;
            FormId = new OutputParameter(nameof(FormId));
            FormConfiguration = new OutputParameter(nameof(FormConfiguration));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(Platform);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            CheckRequiredInput(ServiceId);
            CheckRequiredInput(CategoryId);
            if (ParametersRequiringInput.Count > 1)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            var t1 = System.Environment.TickCount;
            var formMappingMatrix = await ServicesHelper.GetSystemProperty(SystemId.Get(), "FormMatrix");
            var t2 = System.Environment.TickCount;

            if (string.IsNullOrEmpty(formMappingMatrix))
            {
                throw ChannelWorkflowErrorCodes.InvalidFormConfiguration.ToWebFault($"FormMatrix not configured in system");
            }
            var formIds = await ServicesHelper.ResolveMappingMatrix(
                formMappingMatrix,
                new List<string> { ServiceId.Get(), null, null, UserType?.Get(), CategoryId.Get() },
                (values, versions) => values[matrixColumnNumber]);
            var t3 = System.Environment.TickCount;

            // There can not be multiples forms for the same service
            if (formIds.Count() != 1)
            {
                throw ChannelWorkflowErrorCodes.InvalidFormConfiguration.ToWebFault($"More than one form Ids found");
            }
            var formId = formIds.Single();
            var platform = (await ServicesHelper.GetPlatforms())
                .SingleOrDefault(p => p.Id?.ToLower() == Platform.Get()?.ToLower());

            if (platform == null)
            {
                throw ChannelWorkflowErrorCodes.PlatformNotFound.ToWebFault($"Platform {Platform.Get()} not found");
            }

            FormId.Set(formId);

            var data = await ServicesHelper.RenderFormForPlatform(formId, platform.Id);
            var t4 = System.Environment.TickCount;

            Log.Debug($"1={t2-t1}ms 2={t3-t2}ms 3={t4-t3}ms");


            //TODO
            Log.Debug($"Platformid {platform.Id}, ServiceId {ServiceId.Get()}");
            if (platform.Id == "00000000000000000000000000000001" && (ServiceId.Get() == "8C7139EFB9A84002B155C4E7651613ED" || ServiceId.Get()== "2FAED0ECF3E647C59D4B6EF7F9CD6EFB"))
            {
                Log.Debug($"Found Platformid {platform.Id}, ServiceId {ServiceId.Get()}");
                RemoveField(data.VerticalItems, "ServiceId");
            }
            FormConfiguration.Set(data);
            StepState = WorkflowStepState.Done;
            return StepState;
        }
        bool RemoveField(IList<RenderLayoutItem> items,string fieldName)
        {
            if (items == null)
            {
                Log.Debug($"fieldName not fond {fieldName}");
                return false;
            }

            for (int index = 0; index < items.Count; index++)
            {
                var item = items[index];
                if (item.Field?.Name == fieldName)
                {
                    Log.Debug($"Item deleted at {fieldName} at index number {index}");
                    items.RemoveAt(index);
                    return true;
                }
                if (item.Layout != null &&
                    RemoveField(item.Layout.VerticalItems, fieldName))
                    return true;
            }
            return false;
        }
    }

    
}