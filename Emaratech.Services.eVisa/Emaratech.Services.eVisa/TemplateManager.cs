using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Template.Model;
using Newtonsoft.Json;
using Emaratech.Services.Common.Configuration;

namespace Emaratech.Services.eVisa
{
    public class TemplateManager
    {
        public static string GetTemplate(object data, string configTemplateKey)
        {
            Emaratech.Services.Template.Api.CRUDApi templateApi = new Template.Api.CRUDApi(ConfigurationSystem.AppSettings["TemplateApi"]);
            var templateId = ConfigurationSystem.AppSettings[configTemplateKey];
            var json = JsonConvert.SerializeObject(data);
            return templateApi.RenderTemplate(templateId, new Context(json));
        }   
    }
}
