using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Enums
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LegalityStatusType
    {
        [EnumMember]
        Citizen = 1,
        [EnumMember]
        Resident = 6,
        [EnumMember]
        GCCLocal = 8
    }
    public enum SponsorType
    {
        [EnumMember]
        Citizen = 1,
        [EnumMember]
        Resident = 2
    }

    public enum EstablishmentStatus
    {
        [EnumMember]
        Banned = 3
    }


    public enum ServiceType
    {
        [EnumMember]
        EntryPermit = 7,
        [EnumMember]
        Residence = 2
    }

    public enum TravelType
    {
        [EnumMember]
        Entry = 1,
        [EnumMember]
        Exit = 2,
    }

    public enum VisaTypeId
    {
        [EnumMember]
        ShortTermVisit = 37,
        [EnumMember]
        ShortTermLeisure = 99,
        [EnumMember]
        SinglyEntryShortTermLeisure = 85,
        [EnumMember]
        MultiEntryShortTermLeisure = 64,
        [EnumMember]
        MultiEntryShortTermLeisure2 = 61,
        [EnumMember]
        MultiEntryShortTermTourist = 76,
        [EnumMember]
        LongTermVisit = 36,
        [EnumMember]
        LongTermLeisure = 97,
        [EnumMember]
        SingleEntryLongTermLeisure = 86,
        [EnumMember]
        SingleEntryLongTermTourist = 77,
        [EnumMember]
        MultiEntryLongTermLeisure = 65,
        [EnumMember]
        MultiEntryLongTermTourist = 78,
        [EnumMember]
        Residence = 3,
    }

    public enum PermitStatusType
    {
        [EnumMember]
        Issued = 1,
        [EnumMember]
        Used = 2,
        [EnumMember]
        PermanentClosed = 3,
        [EnumMember]
        CancelledBeforeEntry = 8,
        [EnumMember]
        CancelledAfterEntry = 9,
        [EnumMember]
        ResidenceTransffered = 10,
        [EnumMember]
        RenewedOnce = 4,
        [EnumMember]
        RenewedTwice = 5,
        [EnumMember]
        ExtendedOnce = 4,
        [EnumMember]
        ExtendedTwice = 5,
        [EnumMember]
        TemporaryFileClosed = 12,
        [EnumMember]
        CancelledByMOL = 14,
    }

    public enum ResidenceStatusType
    {
        [EnumMember]
        Issued = 1,
        [EnumMember]
        Renewed = 2,
        [EnumMember]
        SponsorTransferred = 3,
        [EnumMember]
        Cancelled = 4,
        [EnumMember]
        PermanentClosed = 5,
        [EnumMember]
        TemporaryFileClosed = 7,
        [EnumMember]
        OutpassIssued = 6,
        [EnumMember]
        CancelTemporaryClosed = 9,
        [EnumMember]
        CancelledByMOL = 14,
    }


    public enum ApplicationStatus
    {
        [EnumMember]
        DocumentRequired = 110,
        [EnumMember]
        SponsorVisitRequired = 170,
        [EnumMember]
        Resubmitted = 160,
        [EnumMember]
        Rejected = 120,
        [EnumMember]
        RefundRequested = 140,
        [EnumMember]
        Refunded = 510,
        [EnumMember]
        RefundFailed = 530,
        [EnumMember]
        RefundInitiated = 550,
        [EnumMember]
        TechnicalRefunded = 560,
        [EnumMember]
        Approved = 100,
        [EnumMember]
        IncompletePayment = 40,
        [EnumMember]
        Paid = 50,
        [EnumMember]
        PaymentFailed = 60,
        [EnumMember]
        ExportedToIntegration = 70,
        [EnumMember]
        ImportedFromIntegration = 80,
        [EnumMember]
        SubmittedToBackOffice = 90,
        [EnumMember]
        InProcessInBackOffice = 180,
    }

    public enum ResidenceType
    {
        [EnumMember]
        Employment = 1,
        [EnumMember]
        Residence = 3,
        [EnumMember]
        EmploymentGovt = 2,
        [EnumMember]
        HouseMaid = 4,
        [EnumMember]
        Partner = 5,
        [EnumMember]
        Investor = 6,
    }

    public enum VisaActionsType
    {
        [EnumMember]
        Renew,
        [EnumMember]
        New,
        [EnumMember]
        Cancel,
        [EnumMember]
        Transfer
    }

    public enum ApplicationActions
    {
        [EnumMember]
        NewRefund
    }

    public enum ApplicationModules
    {
        [EnumMember]
        Default,
        [EnumMember]
        Refund,
        [EnumMember]
        Dashboard,
        [EnumMember]
        Workflows
    }

    //To be uncommented later
    //[DataContract]
    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum VisaStatus
    //{
    //    [EnumMember]
    //    Active,
    //    [EnumMember]
    //    InProgress,
    //    [EnumMember]
    //    ExpiringSoon,
    //    [EnumMember]
    //    Expired
    //}

    //public class VisaTypeAttr : Attribute
    //{
    //    internal VisaTypeAttr(string type)
    //    {
    //        this.Type = type;
    //    }
    //    public string Type { get; private set; }
    //}

    //public enum VisaType
    //{
    //    [VisaTypeAttr("R")]
    //    Residence,
    //    [VisaTypeAttr("P")]
    //    EntryPermit,
    //}

    //public class VisaStatusAttr : Attribute
    //{
    //    internal VisaStatusAttr(string lookupId)
    //    {
    //        this.LookupId = lookupId;
    //    }
    //    public string LookupId { get; private set; }
    //}

    public class DashboardSearchTypeAttr : Attribute
    {
        internal DashboardSearchTypeAttr(string type)
        {
            this.Type = type;
        }
        public string Type { get; private set; }
    }

    public enum DashboardSearchType
    {
        [DashboardSearchTypeAttr("R")]
        Residence = 2,
        [DashboardSearchTypeAttr("P")]
        EntryPermit = 7,
        [DashboardSearchTypeAttr("A")]
        Application
    }


    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VisaStatus
    {
        [EnumMember]
        Active,
        [EnumMember]
        InProgress,
        [EnumMember]
        ExpiringSoon,
        [EnumMember]
        Expired
    }

    public enum VisaType
    {
        [VisaTypeAttr("R")]
        Residence,
        [VisaTypeAttr("P")]
        EntryPermit,
    }

    public enum RefundType
    {
        [EnumMember]
        ApplicationRefund = 1,

        [EnumMember]
        WarrantyRefund = 2
    }
    public enum ContactType
    {
        [EnumMember]
        Mobile = 1,
        [EnumMember]
        Email = 2,
        [EnumMember]
        LandLine = 3,
        [EnumMember]
        Office = 4,
        [EnumMember]
        Aboard = 6
    }

    public enum Nationality
    {
        [EnumMember]
        Syria = 119
    }

    public enum AddressType
    {
        [EnumMember]
        Inside = 1,
        [EnumMember]
        OutSide = 2
    }

    public class VisaStatusAttr : Attribute
    {
        internal VisaStatusAttr(string lookupId)
        {
            this.LookupId = lookupId;
        }
        public string LookupId { get; private set; }
    }

    public class VisaTypeAttr : Attribute
    {
        internal VisaTypeAttr(string type)
        {
            this.Type = type;
        }
        public string Type { get; private set; }
    }

    public enum BarCodeType
    {
        [BarCodeAttr("passport")]
        passport,
        [BarCodeAttr("eida")]
        eida,
    }

    public class BarCodeAttr : Attribute
    {
        internal BarCodeAttr(string type)
        {
            this.Type = type;
        }
        public string Type { get; private set; }
    }
}