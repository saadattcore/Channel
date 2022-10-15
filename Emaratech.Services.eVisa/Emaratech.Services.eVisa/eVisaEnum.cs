using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Emaratech.Services.eVisa
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContactType
    {
        [EnumMember]
        Mobile = 1,
        [EnumMember]
        LandLine = 3,
        [EnumMember]
        Office = 4,
        [EnumMember]
        Aboard = 6
    }

    public enum AddressType
    {
        [EnumMember]
        Inside = 1,
        [EnumMember]
        OutSide = 2
    }

    public enum ApplicationStatus
    {
        [EnumMember]
        Approved = 100,
         [EnumMember]
        DocumentsRequired = 110,
        [EnumMember]
        SponsorVisitRequired = 170,
        [EnumMember]
        Resubmitted = 160,
        [EnumMember]
        Rejected = 120,
        [EnumMember]
        ReadyForRefund = 140,
    }

    public enum FileType
    {
        [EnumMember]
        EntryPermit = 7,
        [EnumMember]
        Residence = 2
    }

    public enum TravelType
    {
        Entry = 1,
        Exit = 2
    }

    public enum SponsorType
    {
        Local = 1,
        Residence = 2,
        Establishment = 3
    }
}
