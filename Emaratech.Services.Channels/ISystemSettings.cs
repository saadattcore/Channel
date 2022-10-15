using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels
{
    public interface ISystemSettings
    {
        string SystemId { get; }

        string ServiceMappingMatrix { get; }

        string WorkflowMappingMatrix { get; }

        T GetProperty<T>(string name);
            
    }
}
