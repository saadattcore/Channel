using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Emaratech.Services.Channels.SSO.Middleware;
using System.Linq;
using NLog;
using System.ServiceModel.Web;

namespace Emaratech.Services.Channels.SSO.Middleware.runtime.extensions
{
    public class SsoMessageInspector : IDispatchMessageInspector
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {

            Logger.Trace($"MyMessageInspector.AfterReceiveRequst");

            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                var httpRequestMessageProperty = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                if (httpRequestMessageProperty != null)
                {
                    string cookiesString = httpRequestMessageProperty.Headers[HttpRequestHeader.Cookie];
                    Logger.Trace($"Retrieved cookies from request: {cookiesString}");

                    var cookies = GetCookies(cookiesString);
                    if (!CheckCookies(cookies)
                        || GetHeaders(ConfigurationManager.AppSettings[Constants.SsoValidationUriKey], cookies) != HttpStatusCode.OK)
                    {
                        request.Properties.Add(Constants.SupressPropertyName, true);
                    }
                }
            }

            var result = Message.CreateMessage(request.Version, "http://tempuri.org/IFake/DoSomething");
            return result;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            Logger.Trace($"MyMessageInspector.BeforeSendReply");

            if (OperationContext.Current.IncomingMessageProperties.ContainsKey(Constants.SupressPropertyName))
            {
                var isSupressRaw = OperationContext.Current.IncomingMessageProperties[Constants.SupressPropertyName];
                var isSupress = (isSupressRaw is bool) ? (bool)isSupressRaw : false;

                if (isSupress)
                {
                    Message replyForRedirect = correlationState as Message;
                    PrepareReplyMessage(replyForRedirect);
                    reply = replyForRedirect;
                }
            }

            AddAccessControlHeaders(reply);

            Logger.Trace("Outgoing reply.");
        }

        private void AddAccessControlHeaders(Message reply)
        {
            var origin = WebOperationContext.Current.IncomingRequest.Headers["Origin"];
            if (!string.IsNullOrWhiteSpace(origin))
            {
                HttpResponseMessageProperty messProp = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
                if (messProp != null)
                {
                    messProp.Headers.Set("Access-Control-Allow-Origin", origin);
                    messProp.Headers.Set("Access-Control-Allow-Credentials", "true");
                }
            }
        }

        private void PrepareReplyMessage(Message replyForRedirect)
        {
            var status = HttpStatusCode.Unauthorized;

            // If this is an OPTIONS header, the client is doing preflight checking if the
            // request is allowed before sending the actual request
            // We need to seend OK code to the client in order to allow it to continue
            if (WebOperationContext.Current.IncomingRequest.Method == "OPTIONS")
            {
                status = HttpStatusCode.OK;
            }

            var httpResponseMsgProperty = new HttpResponseMessageProperty
            {
                SuppressEntityBody = false,
                SuppressPreamble = false,
                StatusCode = status,
                StatusDescription = ""
            };

            httpResponseMsgProperty.Headers.Add("Content-Type", "application/json; charset=utf-8");
            httpResponseMsgProperty.Headers.Add("X-RedirectUrl", ConfigurationManager.AppSettings[Constants.SsoRedirectUriKey]);
            httpResponseMsgProperty.Headers.Add("Access-Control-Expose-Headers", "X-RedirectUrl");
            httpResponseMsgProperty.Headers.Add("Access-Control-Allow-Headers", "Keep-Alive,User-Agent,Cache-Control,Content-Type,X-RedirectUrl");
            httpResponseMsgProperty.Headers.Add("Vary", "Origin");
            replyForRedirect.Properties.Add("WebBodyFormatMessageProperty", new WebBodyFormatMessageProperty(WebContentFormat.Json));
            replyForRedirect.Properties.Add("httpResponse", httpResponseMsgProperty);
        }

        private CookieCollection GetCookies(string cookiesString)
        {
            if (!string.IsNullOrWhiteSpace(cookiesString))
            {
                var cookies = cookiesString.Split(';');
                if (cookies != null)
                {
                    var cookiesCollection = new CookieCollection();
                    foreach (var cookieString in cookies)
                    {
                        var cookieNameValue = cookieString.Split('=');
                        var name = cookieNameValue[0].Trim();
                        var value = cookieNameValue[1].Trim();
                        var cookie = new Cookie(name, value);
                        cookiesCollection.Add(cookie);
                    }
                    return cookiesCollection;
                }
            }
            return null;
        }

        private bool CheckCookies(CookieCollection cookies)
        {
            if (cookies == null
                || cookies[Constants.SecurityCookieName] == null
                || cookies[Constants.UserIdCookieName] == null
                || string.IsNullOrWhiteSpace(cookies[Constants.UserIdCookieName].Value))
            {
                return false;
            }
            return true;
        }

        public HttpStatusCode GetHeaders(string url, CookieCollection cookies)
        {
            HttpStatusCode result = default(HttpStatusCode);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.CookieContainer = new CookieContainer();
            string domain = request.RequestUri.Host;
            
            foreach (Cookie cookie in cookies)
            {
                cookie.Domain = domain;
                request.CookieContainer.Add(cookie);
            }
            request.AllowAutoRedirect = false;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response != null)
                {
                    result = response.StatusCode;
                    response.Close();
                }
            }
            return result;
        }
    }
}