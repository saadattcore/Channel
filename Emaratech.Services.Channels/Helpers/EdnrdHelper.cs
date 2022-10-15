using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.Helpers
{
    public class EdnrdHelper
    {
        public async static Task<RSAAuthenticationResponse> Authenticate(RSAAuthenticationData AuthenticationRequest)
        {


            string MEDIA_TYPE = "application/json";
            string url = ConfigurationManager.AppSettings["RSAAuthenticationURL"];
            var result = new RSAAuthenticationResponse();
            var requestBody = new RSAAuthenticationRequest
            {
                oldToken = AuthenticationRequest.OldToken,
                token = AuthenticationRequest.Token,
                pin = AuthenticationRequest.Pin,
                deviceId = AuthenticationRequest.DeviceId
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
                    HttpResponseMessage response = client.PostAsJsonAsync($"{AuthenticationRequest.ActionType}", requestBody).Result;
                    result = await response.Content.ReadAsAsync<RSAAuthenticationResponse>();

                }
            }
            catch (Exception ex)
            {
                result.StatusId = System.Net.HttpStatusCode.InternalServerError.ToString();
                result.StatusDesc = ex.Message + ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }


            return result;
        }
    }
}