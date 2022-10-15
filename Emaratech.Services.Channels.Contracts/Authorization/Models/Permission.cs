namespace Emaratech.Services.Channels.Contracts.Authorization
{
    using Newtonsoft.Json;

    public class Permission
    {
        [JsonProperty(PropertyName = "res_grp")]
        public string ResourceGroup { get; set; }
        [JsonProperty(PropertyName = "res_id")]
        public string ResourceId { get; set; }
        [JsonProperty(PropertyName = "act")]
        public string Action { get; set; }
        [JsonProperty(PropertyName = "sys_id")]
        public string SystemId { get; set; }
    }
}
