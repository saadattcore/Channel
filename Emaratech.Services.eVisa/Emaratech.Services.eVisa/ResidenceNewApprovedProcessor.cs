﻿using Emaratech.Services.Application.Model;
using Emaratech.Services.Common.Configuration;
using Emaratech.Services.eVisa.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa
{
    class ResidenceNewApprovedProcessor : StatusProcessor
    {
        protected override string GetSubject(RestApplicationSearchRow application)
 => ReportUtil.residenceNewApprovedSubject;

        protected override string GetTemplateContent(RestApplicationSearchRow application)
        {
            string content = TemplateManager.GetTemplate(DataUtil.ModelToDictionary(application.RestApplicationSearchKeyValues) , "TemplateIdApprovedResidence");
            return content;
        }

        protected override IList<RestApplicationSearchRow> GetApplications()
        {
            var id = ConfigurationSystem.AppSettings["ResidenceNewCategory"];
            return ApplicationRepository.GetApprovedStatus(id);
        }
    }
}
