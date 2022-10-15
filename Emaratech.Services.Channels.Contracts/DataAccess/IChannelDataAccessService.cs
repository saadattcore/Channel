using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.DataAccess
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IChannelDataAccessService
    {
        [OperationContract]
        Task<string> GetUserId(string login);

        [OperationContract]
        Task<IEnumerable<string>> GetApplicableVisaTypes(string userId);
    }
}
