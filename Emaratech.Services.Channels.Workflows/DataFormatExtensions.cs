using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Emaratech.Services.Channels.Workflows
{
    public static class DataFormatExtensions
    {
        public static XmlDocument ToUnifiedXml(this JObject unifiedApplication)
        {
            JObject obj = new JObject();
            obj[WorkflowConstants.UnifiedApplicationRootNode] = unifiedApplication;
            return JsonConvert.DeserializeXmlNode(obj.ToString());
        }
    }
}
