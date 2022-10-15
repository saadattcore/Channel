using Emaratech.Services.Application.Model;
using Emaratech.Services.Common.Configuration;
using Emaratech.Services.eVisa.Reports;
using System.Collections.Generic;
using System.Configuration;

namespace Emaratech.Services.eVisa
{
    class EntryPermitApplicationPostedStatusProcessor : StatusProcessor
    {
        protected override string GetSubject(RestApplicationSearchRow application)
      => ReportUtil.entryPermitPosted;

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            string content = TemplateManager.GetTemplate(
                DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues),
                "TemplateIdEntryPermitPosted");
            return content;
        }

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var statusId = Emaratech.Services.Common.Configuration.ConfigurationSystem.AppSettings["FeesPaidStatusId"];
            var id = ConfigurationSystem.AppSettings["EntryPermitNewCategory"];
            return ApplicationRepository.GetApplicationsByStatusId(statusId , id);
        }
    }
}
