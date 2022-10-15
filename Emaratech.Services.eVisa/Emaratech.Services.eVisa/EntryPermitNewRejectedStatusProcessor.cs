using System.Collections.Generic;
using Emaratech.Services.Application.Model;
using Emaratech.Services.eVisa.Reports;
using System.Configuration;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    class EntryPermitNewRejectedStatusProcessor :StatusProcessor
    {
        protected override string GetSubject(RestApplicationSearchRow application)
            => ReportUtil.entryPermitNewRejectedSubject; 

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            string content = TemplateManager.GetTemplate(DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues) , "TemplateIdEntryPermitNewRejectedApplication");
            return content;
        }

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var id = ConfigurationSystem.AppSettings["EntryPermitNewCategory"];
            return ApplicationRepository.GetEntryPermitRejectedStatus(id);
        }
    }
}