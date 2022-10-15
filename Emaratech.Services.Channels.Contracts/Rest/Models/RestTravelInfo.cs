using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;

namespace Emaratech.Services.Channels.Contracts.Rest.Models
{
    public class RestTravelInfo
    {
        public TravelType TravelType { get; set; }

        public DateTime? TravelDate { get; set; }
    }
}
