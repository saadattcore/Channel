using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.BusinessLogic.ApplicationReports.Cancellation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    public static class ReportHandlerFactory
    {
        public static IReportHandler Create(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.Application_eVisa:
                    {
                        return new ApplicationVisaReportHandler();
                    }
                case ReportType.EntryPermit_eVisa:
                    {
                        return new EntryPermitVisaReportHandler();
                    }
                case ReportType.ResidenceCancellation:
                    {
                        return new ResidenceCancellationReportHandler();
                    }
                case ReportType.EntryPermitCancellation:
                    {
                        return new EntryPermitCancellationReportHandler();
                    }
                default:
                    return null;
            }
        }
    }
}