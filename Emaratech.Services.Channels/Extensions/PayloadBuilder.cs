using Emaratech.Services.Channels.Contracts.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.WcfCommons.Faults;
using Newtonsoft.Json.Linq;
using System.Net;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using Emaratech.Services.Security.KeyVault.Model;
using Emaratech.Services.Channels.Contracts.Errors;
using System.ServiceModel;
using Emaratech.Services.Security.KeyVault.Api;

namespace Emaratech.Services.Channels.Extensions
{
    public class PayloadBuilder
    {
        private readonly RestTokenPayload payload = new RestTokenPayload() { Payload = new List<RestPayloadValue>() };

        public static PayloadBuilder New()
        {
            return new PayloadBuilder();
        }

        public PayloadBuilder Key(string systemId, string key)
        {
            payload.SystemId = systemId;
            payload.KeyName = key;

            return this;
        }

        public PayloadBuilder Expiry(TimeSpan expiryTime)
        {
            payload.ValidDuration = (int)expiryTime.TotalMinutes;
            return this;
        }

        public PayloadBuilder Add(object name, object value)
        {
            payload.Payload.Add(new RestPayloadValue { Name = name.ToString(), Value = value.ToString() });
            return this;
        }

        public PayloadBuilder Add<T>(IEnumerable<T> list, Func<T, object> nameFunc, Func<T, object> valueFunc)
        {
            foreach (var item in list)
            {
                payload.Payload.Add(new RestPayloadValue { Name = nameFunc(item).ToString(), Value = valueFunc(item).ToString() });
            }
            return this;
        }

        public Task<string> Issue(ITokensApi tokenApi)
        {
            payload.Issuer = "http://Channel";
            payload.Audience = "http://Mobile";
            return tokenApi.IssueTokenAsync(payload);
        }
    }
}