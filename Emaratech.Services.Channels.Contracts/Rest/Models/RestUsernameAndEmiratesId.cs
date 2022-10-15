using System.Runtime.Serialization;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestUsernameAndEmiratesId
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }


    }
}