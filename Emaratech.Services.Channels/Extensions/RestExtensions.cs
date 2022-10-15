using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Emaratech.Services.Workflows.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace Emaratech.Services.Channels.Extensions
{
    public static class RestExtensions
    {
        public static JObject ToJObject(this object obj)
        {
            JObject o = JObject.FromObject(obj);
            return o;
        }

        public static Stream ToJsonStream(this object obj)
        {
            JObject o = JObject.FromObject(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(o.ToString());
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        public static Dictionary<string, object> ToDictionary(this RestWorkflowState state)
        {
            var jObject = JObject.Parse(state.Context);
            var objects = jObject.ToObject<IDictionary<string, object>>();
            var dictionary = new Dictionary<string, object>(objects, StringComparer.CurrentCultureIgnoreCase);
            dictionary["WorkflowToken"] = state.InstanceId;
            return dictionary;
        }

        public static Dictionary<string, object> ToDictionary(this Stream response)
        {
            if (response != null)
            {
                var serializer = new JsonSerializer();
                using (var sr = new StreamReader(response))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var o = serializer.Deserialize(jsonTextReader) as JObject;
                        if (o != null)
                        {
                            return new Dictionary<string, object>(o.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
                        }
                    }
                }
            }
            return new Dictionary<string, object>();
        }

    }
}