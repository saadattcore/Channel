using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Vision.Model;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class GetEstablishmentDependentStep : ProcessingStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GetEstablishmentDependentStep));
        private string EstablishmentCode { get; }
        public Action<IEnumerable<RestIndividualDependentsInfoWrapper>> StoreAction { get; set; }

        public GetEstablishmentDependentStep(string establishmentCode, Action<IEnumerable<RestIndividualDependentsInfoWrapper>> argStore, ProcessingStep argNextStep) : base(argNextStep)
        {
            EstablishmentCode = establishmentCode;
            StoreAction = argStore;
        }

        protected override async Task Execute()
        {
            Log.Debug("Going to get establishment dependents");
            StoreAction(await ServicesHelper.GetEstablishmentDependents(EstablishmentCode));
            Log.Debug("Establishment dependents list received");
        }
    }
}
