using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Models
{
    public enum ResidenceStatus
    {
        Issued = 1,
        Renewed = 2,
        SponsorTransffered = 3,
        Cancelled = 4,
        PermanentClosed = 5,
        OutpassIssued = 6,
        TemporaryFileClosed = 7,
        CancelTemporaryClosed = 9,
        CancelledbyMOL = 14
    }

    public enum ResidenceType
    {
        Employment = 1,
        EmploymentL = 2,
        Residence = 3,
        HouseMaid = 4,
        Partner = 5,
        Investor = 6
    }

    public enum SponsorType
    {
        SponsorFile = 1,
        Resident = 2,
        Establishment = 3
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    public enum PassportType
    {
        Normal = 1,
        Diplomatic = 2,
        Private = 3,
        Assignment = 5,
        TravelDoc = 7
    }

    public enum PaymentType
    {
        NoqodiNormal = 1,
        NoqodiExpress = 2,
        VirtualBank = 3
    }

    public enum MaritalStatus
    {
        Single = 1,
        Married = 2,
        Divorced = 3,
        Widow = 4,
        Deceased = 5,
        Unspecific = 6,
        Child = 7
    }

    public enum ApplicationStatus
    {
        // Quick Script to Refresh Enums: select l.status_desc_e || ' = ' || l.status_id || ',' from status_type_lk l
        Incomplete = 10,
        IncompleteDocuments = 20,
        Complete = 30,
        IncompletePayment = 40,
        Paid = 50,
        PaymentFailed = 60,
        ExportedToIntegration = 70,
        ImportedFromIntegration = 80,
        TransferredToBackOffice = 90,
        Approved = 100,
        DocumentRequired = 110,
        Rejected = 120,
        Printed = 130,
        Refunded = 140,
        Deleted = 150,
        Resubmit = 160,
        SponsorVisitRequired = 170,
    }

    public enum SponsorRelationship
    {
        Husband = 1,
        Wife = 2,
        Son = 3,
        Daughter = 4,
        NotRelated = 5,
        Mother = 6,
        Brother = 7,
        Sister = 8,
        Father = 9,
        Friend = 10,
        GrandFather = 11,
        GrandMother = 12,
        FatherInLaw = 13,
        MotherInLaw = 14,
        BrotherInLaw = 15,
        SisterInLaw = 16,
        Uncle = 17,
        Aunt = 18,
        CoBrother = 19,
        Stepfather = 21,
        Cousin = 23,
        Nephew = 25,
        Niece = 26,
        Others = 99
    }

    public enum VisaType
    {
        Residence = 3,
        LeisureShortMulti = 64,
        LeisureLongMulti = 65,
        LeisureShortSingle = 85,
        LeisureLongSingle = 86
    }
}