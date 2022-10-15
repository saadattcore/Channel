using System;
using System.Collections.Generic;
using Emaratech.Services.Channels.Reports.Models.Sns;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class SetEmailContentStep : SetReportContentBase
    {
        public SetEmailContentStep(
            Func<SponsorInfo> argGetSponsorInfo,
            Func<IEnumerable<ResidenceDetails>> argGetResidenceDetailsFunc,
            Func<IEnumerable<EntryPermitDetails>> argGetEntryPermitDetailsFunc,
            Action<string> argSetReportOutput,
            ProcessingStep argNextStep) : base(argGetSponsorInfo, argGetResidenceDetailsFunc, argGetEntryPermitDetailsFunc, argSetReportOutput, argNextStep)
        {
        }

        protected override string SerializeReport(object data)
        {
            return string.Empty;
        }
    }
}