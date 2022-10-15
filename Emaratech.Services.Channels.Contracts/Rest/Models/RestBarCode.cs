using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    public class RestBarCode
    {
        public string data { get; set; }
        public string height { get; set; }
        public string mode { get; set; }
        public string width { get; set; }
    }
}
