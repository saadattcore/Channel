using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Helpers
{
    public class HttpHelper
    {
        private static string ContentType = "application/json";
        private static string ApiKey = System.Configuration.ConfigurationManager.AppSettings["APIKey"];

        public static async Task<string> Post(string url, string json, NameValueCollection headers = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    if (headers != null)
                    {
                        client.Headers.Add(headers);
                    }

                    CheckApiKey(client);
                    client.Headers[HttpRequestHeader.ContentType] = ContentType;
                    var result = await client.UploadStringTaskAsync(url, json);
                    return result;
                }
            }
            catch (WebException exception)
            {
                string responseText;
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }
                return responseText;
            }
        }

        public static async Task<string> Get(string url, NameValueCollection headers = null)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                if (headers != null)
                {
                    client.Headers.Add(headers);
                }

                CheckApiKey(client);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = await client.DownloadStringTaskAsync(url);
                return result;
            }
        }

        public static async Task<string> Delete(string url, string json, NameValueCollection headers = null)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                if (headers != null)
                {
                    client.Headers.Add(headers);
                }

                CheckApiKey(client);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = await client.UploadStringTaskAsync(url, "DELETE", json);
                return result;
            }
        }

        private static void CheckApiKey(WebClient client)
        {
            if (client.Headers.Get("apikey") == null || string.IsNullOrEmpty(client.Headers["apikey"]))
            {
                client.Headers["apikey"] = ApiKey;
            }
        }
    }
}