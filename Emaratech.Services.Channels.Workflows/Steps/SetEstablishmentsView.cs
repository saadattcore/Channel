using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Workflows.Engine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Models;
using System.Linq;
using log4net;
using Emaratech.Services.Channels.Contracts.Rest.Models;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SetEstablishmentsView : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SetEstablishmentsView));

        public OutputParameter View { get; set; }
        public OutputParameter EstablishmentList { get; set; }
        public InputParameter<string> UserName { get; set; }
        public InputParameter<string> EstablishmentCode { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            View = new OutputParameter(nameof(View));
            EstablishmentList = new OutputParameter(nameof(EstablishmentList));
        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

            CheckRequiredInput(UserName);
            CheckRequiredInput(EstablishmentCode);
            if (ParametersRequiringInput.Count > 0)
            {
                return StepState = WorkflowStepState.InputRequired;
            }

            View.Set(ViewEnum.Dependents);

            var establishments = GetEstablishmentsInfo(EstablishmentCode.Get());
            EstablishmentList.Set(establishments);
            return StepState = WorkflowStepState.Done;
        }

        private async Task<IEnumerable<RestDependentVisaInfo>> GetEstablishmentsInfo(string establishmentCode)
        {
            var establishments = await ServicesHelper.GetUserEstablishments(UserName.Get());
            return establishments.Select(x => new RestDependentVisaInfo
            {
                FirstName = new RestName
                {
                    Ar = x.EstabNameAr,
                    En = x.EstabNameEn
                },
                VisaNumber = x.EstabCode
            });
        }
    }
}