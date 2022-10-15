using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Identity.Utils;
using Emaratech.Services.WcfCommons.Faults.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Emaratech.Services.Channels
{

    public class ClaimUtil
    {
        private static string profileClaim = ConfigurationManager.AppSettings.Get("profileClaim");
        private static string permissionClaim = ConfigurationManager.AppSettings.Get("permissionClaim");

        public static string GetAuthenticatedUserId()
        {
            if (ConfigurationManager.AppSettings["AllowAnonymousAccess"] == "true")
                return "5FC039517FA04F028BEA4D0EA96FC5DB";
            return GetUserClaim("sub");
        }

        public static string GetEstablishmentCode()
        {
            if (GetAuthenticatedUserType() == Constants.UserTypeLookup.EstablishmentUserType)
            {
                return GetUserClaim("estab_code", true);
            }
            else
            {
                return GetUserClaim("EstablishmentCode", false);
            }
        }

        public static string GetEstablishmentStatusEn()
        {
            bool throwException = GetAuthenticatedUserType() == Constants.UserTypeLookup.EstablishmentUserType;
            return GetUserClaim("estab_statusEn", throwException);
        }

        public static string GetEstablishmentStatusAr()
        {
            bool throwException = GetAuthenticatedUserType() == Constants.UserTypeLookup.EstablishmentUserType
            ;
            return GetUserClaim("estab_statusAr", throwException);
        }

        public static string GetAccessToken()
        {
            return GetUserClaim("jwt");
        }

        public static string GetAuthenticatedUserType()
        {
            if (ConfigurationManager.AppSettings["AllowAnonymousAccess"] == "true")
                return "230AAC5F9259481F95A92AAC60F5D705";

            return GetUserClaim("UserType");
        }

        public static string GetAuthenticatedSponsorType()
        {
            bool throwException = GetAuthenticatedUserType() == Constants.UserTypeLookup.ResidentUserType;
            return GetUserClaim("SponsorType", throwException);
        }

        public static string GetAuthenticatedEstablishmentType()
        {
            //bool throwException = GetAuthenticatedUserType() != Constants.UserTypeLookup.ResidentUserType;
            return GetUserClaim("EstablishmentType", false);
        }

        public static string GetAuthenticatedVisaNumber()
        {
            if (ConfigurationManager.AppSettings["AllowAnonymousAccess"] == "true")
                return "20120137018043";

            bool throwException = GetAuthenticatedUserType() == Constants.UserTypeLookup.ResidentUserType;
            return GetUserClaim("VisaNumber", throwException);
        }

        public static string GetAuthenticatedSponsorFileType()
        {
            bool throwException = GetAuthenticatedUserType() != Constants.UserTypeLookup.EstablishmentUserType;
            return GetUserClaim("FileTypeId",throwException);
        }

        public static string GetAuthenticatedUserEmail()
        {
            return GetUserClaim("email", false);
        }

        public static string GetAuthenticatedUseMobile()
        {
            return GetUserClaim("Mobile", false);
        }

        public static string GetAuthenticatedSponsorNo()
        {
            if (ConfigurationManager.AppSettings["AllowAnonymousAccess"] == "true")
                return "20120137018043";
            return GetUserClaim("SponsorNo", false);
        }

        public static string GetEmiratesId()
        {
            return GetUserClaim("EmiratesId", false);
        }

        public static string GetBirthDate()
        {
            return GetUserClaim("DateOfBirth", false);
        }

        public static string GetAuthenticatedSystemId()
        {
            if (ConfigurationManager.AppSettings["AllowAnonymousAccess"] == "true")
                return "08867038D3934BCA804CD4074735B260";

            return GetUserClaim("sys_id");
        }

        public static string GetAuthenticatedUserJobId()
        {
            bool throwException = GetAuthenticatedUserType() == Constants.UserTypeLookup.ResidentUserType;
            return GetUserClaim("JobId", throwException);
        }

        public static string GetAuthenticatedUserGenderId()
        {
            return GetUserClaim("GenderId", false);
        }

        public static bool IsLoggedIn()
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            return principal?.Identities.Any(x => x.IsAuthenticated) == true;
        }

        public static List<string> GetUserPermissionClaim()
        {
            return GetUserClaimAsList(permissionClaim, string.Empty) as List<string>;
        }
        public static string GetAuthenticatedUserName()
        {
            return GetUserClaim("name", false);
        }

        public static void ImpersonateAnonymousUser()
        {
            Impersonator.Impersonate("08867038D3934BCA804CD4074735B260", "secret", "channelanonymous", "P@ss1234");
        }

        private static string GetUserClaim(string name, bool throwException = true)
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal?.Identities.Any(x => x.IsAuthenticated) == true)
            {
                var cl = principal.FindFirst(c => c.Type == name
                            || c.Type == $"{profileClaim}/{name}");
                if (cl != null)
                {
                    return cl.Value;
                }

            }

            if (throwException)
            {
                throw ErrorCodes.Unauthorized.ToWebFault($"Not allowed, claim {name} not found.");
            }
            return null;
        }

        private static List<string> GetUserClaimAsList(string name, string defaultValue)
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal?.Identities.Any(x => x.IsAuthenticated) == true)
            {
                var claims = principal.FindAll(c => c.Type == name);

                if (claims != null && claims.Count() > 0)
                {
                    return claims.Select(p => p.Value).ToList();
                }

                foreach (var identity in principal.Identities)
                {
                    var claim = identity.Claims.Where(c => c.Type == profileClaim)
                        .Select(v => JObject.Parse(v.Value))
                        .Where(v => v[name] != null)
                        .Select(u => u[name].ToString()).FirstOrDefault();

                    if (claim != null)
                    {
                        return new List<string>() { claim };
                    }
                }
            }

            if (ConfigurationManager.AppSettings["AllowAnonymousAccess"] == "true")
            {
                return new List<string>() { defaultValue };
            }

            return null;
        }
    }
}