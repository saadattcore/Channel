using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [SwaggerWcfDefinition(Name="ServiceInfo")]
    [DataContract]
    public class RestService
    {
        [DataMember(Name = "categoryId")]
        public string CategoryId { get; set; }
        [DataMember(Name = "categoryResourceKey")]
        public string CategoryResourceKey { get; set; }
        [DataMember(Name = "subCategoryId")]
        public string SubCategoryId { get; set; }
        [DataMember(Name = "subCategoryResourceKey")]
        public string SubCategoryResourceKey { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "resourceKey")]
        public string ResourceKey { get; set; }
    }
}
