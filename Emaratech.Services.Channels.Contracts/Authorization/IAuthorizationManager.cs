using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Authorization
{
    public interface IAuthorizationManager
    {
        /// <summary>
        /// Check if user has access for specific resource and action based on user permissions
        /// </summary>
        /// <param name="resourceId">The resource should be accessed, if no resource required pass null</param>
        /// <returns>True if user has access for given resource and action else false</returns>
        bool CheckAccess(string resourceId, string action);
    }
}
