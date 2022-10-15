
namespace Emaratech.Services.Channels.Authorization
{
    using Emaratech.Services.Channels.Contracts.Authorization;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class AuthorizationManager : IAuthorizationManager
    {
        /// <inhertdoc/>
        public bool CheckAccess(string resourceId, string action)
        {
            var permissions = GetUserPermissions();

            return permissions.Any(p => (string.IsNullOrWhiteSpace(resourceId) || 
                                         p.ResourceId.ToLower().Equals(resourceId.ToLower())) &&
                                         p.Action.ToLower().Equals(action.ToLower()));
        }

        private List<Permission> GetUserPermissions()
        {
            var userPermissions = new List<Permission>();
            var permissions = ClaimUtil.GetUserPermissionClaim();
            
            if (permissions != null && permissions.Count() > 0)
            {
                foreach (var permission in permissions)
                {
                    userPermissions.Add(
                        JsonConvert.DeserializeObject<Permission>(permission));
                }
            }

            return userPermissions;
        }
    }
}