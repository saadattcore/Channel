using Emaratech.Services.Application.Model;
namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IReportHandler
    {
        Task<IList<ReportRecordInfo>> GetReportEntries(IList<string> ids);
        string GenerateReport(IList<ReportRecordInfo> reports);
    }
}
