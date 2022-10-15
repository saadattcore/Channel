using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Application
{
    public class RestApplicationActionsCriteria
    {
        public string ServiceId { get; set; }
        public string ApplicationStatus { get; set; }
        public string UserType { get; set; }
        public ApplicationModules ApplicationModule { get; set; }
    }
}
