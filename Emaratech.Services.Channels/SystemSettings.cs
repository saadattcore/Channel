using Emaratech.Services.Systems.Api;
using Emaratech.Services.Systems.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Emaratech.Services.Channels
{
    public class SystemSettings : ISystemSettings
    {
        private readonly ILog LOG = LogManager.GetLogger(typeof(SystemSettings));

        private readonly SystemInfo systemInfo;
        private readonly IList<RestSystemConfigurationProperty> systemProperties;

        public SystemSettings(string systemId, IServiceFactory factory)
        {
            SystemId = systemId;
            systemInfo = factory.GetSystemApi().GetSystem(SystemId);
            systemProperties = factory.GetSystemApi().GetAllProperties(SystemId);
        }

        void Init()
        {

        }

        public string ServiceMappingMatrix
        {
            get { return systemProperties.Single(x=>x.PropName=="ServiceMatrix").PropValue; }
        }

        public string WorkflowMappingMatrix
        {
            get { return systemProperties.Single(x => x.PropName == "WorkflowMatrix").PropValue; }
        }

        public string SystemId { get; private set; }

        public T GetProperty<T>(string name)
        {
            return (T)Convert.ChangeType(systemProperties.SingleOrDefault(x => x.PropName == name)?.PropValue,typeof(T));
        }
    }
}