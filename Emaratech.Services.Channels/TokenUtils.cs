using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Security.KeyVault.Model;

namespace Emaratech.Services.Channels
{
    public static  class TokenUtils
    {
        public static string TokenKey(string systemId)
        {
            return ApiFactory.Default.GetSystemApi()
                .GetAllProperties(systemId).Single(x => x.PropName == Constants.KeyVaultToken)
                .PropValue;
        }

        public static Task<List<RestPayloadValue>> VerifyTokenInContext(string systemId,string token)
        {
            var key = TokenKey(systemId);

            return new DefaultServiceFactory().GetTokensApi()
                .VerifyTokenAsync(new RestToken
                {
                    SystemId = systemId,
                    KeyName = key,
                    Token = token,
                    ValidIssuers = new List<string> { "http://Channel" },
                    ValidAudiences = new List<string> { "http://Mobile" }
                });
        }
    }
}