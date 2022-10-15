using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;


namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name = "RestKeyValueList")]
    [DataContract]
    public class RestKeyValueList
    {
        [DataMember(Name = "keyValueList")]
        public IList<RestKeyValue> KeyValueList
        {
            get;
            set;
        }
    }
}
