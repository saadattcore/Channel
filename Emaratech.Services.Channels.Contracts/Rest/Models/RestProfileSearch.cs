using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    [DataContract]
    public class RestProfileSearch
    {
        [DataMember(Name = "emiratesId")]
        public string EmiratesId { get; set; }
        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
    }
}
