using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Emaratech.Services.Ado;
using Emaratech.Services.Channels.Contracts.DataAccess;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.DataAccess
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ChannelDataAccessService : IChannelDataAccessService
    {
        private readonly IDbProvider provider;
        public ChannelDataAccessService(IDbProvider provider)
        {
            this.provider = provider;
        }

        public async Task<string> GetUserId(string login)
        {
            login = login.Contains("/") ? login.Split('/')[1] : login;
            return provider.RunScalar(Queries.GET_USER_ID, 
                new Dictionary<string, object> { { "login", login } })?.ToString();
        }

        public async Task<IEnumerable<string>> GetApplicableVisaTypes(string userId)
        {
            var allow_spn_id = provider.RunScalar(Queries.GET_ALLOW_SPN_ID, new Dictionary<string, object> { { "user_id", userId } })?.ToString();
            
            var visa_types = provider.RunQuery(
                Queries.GET_VISA_TYPES_FOR_SPN, new Dictionary<string, object> { { "allow_spn_id", allow_spn_id } })
                .Select(x=>x.GetString(0));

            return visa_types;
        }
    }
}