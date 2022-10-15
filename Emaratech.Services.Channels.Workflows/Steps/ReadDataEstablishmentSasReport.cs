using Emaratech.Services.Workflows.Engine;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class ReadDataEstablishmentSasReport : ReadEmailAddressForReport
    {
        public OutputParameter EstablishmentCode { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            EstablishmentCode = new OutputParameter(nameof(EstablishmentCode));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();
            var estCode = UnifiedApplication.Get()["SponsorDetails"]["EstablishmentId"];
            EstablishmentCode.Set(estCode);
            return StepState = WorkflowStepState.Done;
        }
    }
}