using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class GetDependentsStep : ProcessingStep
    {
        private string SponsorFileNo { get; }
        public Action<IEnumerable<RestIndividualDependentsInfoWrapper>> StoreAction { get; set; }

        public GetDependentsStep(string sponsorFileNo, Action<IEnumerable<RestIndividualDependentsInfoWrapper>> argStore, ProcessingStep argNextStep) : base(argNextStep)
        {
            SponsorFileNo = sponsorFileNo;
            StoreAction = argStore;
        }

        protected override async Task Execute()
        {
            StoreAction(await ServicesHelper.GetDependents(SponsorFileNo));
        }
    }
}