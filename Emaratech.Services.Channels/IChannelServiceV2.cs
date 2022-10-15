using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Emaratech.Services.Channels
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IChannelServiceV2" in both code and config file together.
    [ServiceContract]
    public interface IChannelServiceV2
    {
        [OperationContract]
        void DoWork();
    }
}
