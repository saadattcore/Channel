using System.Collections.Generic;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using SponsorType = Emaratech.Services.Channels.Workflows.Models.SponsorType;
using static Emaratech.Services.Channels.Models.Enums.Constants;

namespace Emaratech.Services.Channels.Workflows
{
    internal static class WorkflowConstants
    {
        public class WorkflowParameterKeys
        {
            public const string SystemId = "SystemId";
            public const string ApplicationId = "ApplicationId";
            public const string ContactNo = "ContactNo";
            public const string Landline = "Landline";
            public const string SponsorName = "SponsorName";
            public const string Area = "Area";
            public const string Address = "Address";
            public const string PoBox = "PoBox";
            public const string OdrStatus = "OdrStatus";
            public const string DeliveryMode = "DeliveryMode";
            public const string ApplicationType = "ApplicationType";
            public const string FileNo = "FileNo";
            public const string ProductType = "ProductType";
            public const string UniqueId = "UniqueId";
        }

        public class Statuses
        {
            public static string ResidenceExistingApplicationStatuses = string.Concat(ApplicationStatus.Paid.GetHashCode(), ",", ApplicationStatus.ExportedToIntegration.GetHashCode(), ",",
                ApplicationStatus.ImportedFromIntegration.GetHashCode(), ",", ApplicationStatus.SubmittedToBackOffice.GetHashCode(), ",", ApplicationStatus.DocumentRequired.GetHashCode(), ",",
                ApplicationStatus.Resubmitted.GetHashCode(), ",", ApplicationStatus.SponsorVisitRequired.GetHashCode(), ",", ApplicationStatus.InProcessInBackOffice.GetHashCode());

            public static string EntryPermitExistingApplicationStatuses = string.Concat(ApplicationStatus.Paid.GetHashCode(), ",", ApplicationStatus.ExportedToIntegration.GetHashCode(), ",",
                ApplicationStatus.ImportedFromIntegration.GetHashCode(), ",", ApplicationStatus.SubmittedToBackOffice.GetHashCode(), ",", ApplicationStatus.DocumentRequired.GetHashCode(), ",",
                ApplicationStatus.Resubmitted.GetHashCode(), ",", ApplicationStatus.SponsorVisitRequired.GetHashCode(), ",", ApplicationStatus.InProcessInBackOffice.GetHashCode());
        }

        public class WorkflowParameterJsonKeys
        {
            public const string SystemId = "systemId";
            public const string ApplicationId = "applicationId";
            public const string ContactNo = "contactNo";
            public const string Landline = "landline";
            public const string SponsorName = "sponsorName";
            public const string Area = "area";
            public const string Address = "address";
            public const string PoBox = "poBox";
            public const string OdrStatus = "odrStatus";
            public const string DeliveryMode = "deliveryMode";
            public const string ApplicationType = "ApplicationType";
            public const string FileNo = "FileNo";
            public const string ProductType = "ProductType";
            public const string UniqueId = "uniqueId";
        }

        public const string CancellationReasonEntry = "Sponsor Request";
        public const string CancellationReasonExit = "Outside Country";
        public const string UnifiedApplicationRootNode = "UnifiedApplication";

        public const string LookupVisaType = "VisaType";
        public const string LookupVisaStatus = "VisaStatus";

        public static Dictionary<int?, int?> LegalitySponsorTypeMapping = new Dictionary<int?, int?>();
        public static Dictionary<string, SponsorType> UserSponsorMapping = new Dictionary<string, SponsorType>();
        public static Dictionary<TravelType, string> TravelTypeIsInsideMapping = new Dictionary<TravelType, string>();

        static WorkflowConstants()
        {
            // Mapping of Legality and Sponsor Type
            LegalitySponsorTypeMapping.Add(1, 1); // UAE Citizen to SponsorFile
            LegalitySponsorTypeMapping.Add(6, 2); // Resident to Residence
            LegalitySponsorTypeMapping.Add(8, 99); //  GCC Local to GCC National

            UserSponsorMapping.Add(Emaratech.Services.Channels.Models.Enums.Constants.UserTypeLookup.CitizenUserType, SponsorType.SponsorFile);
            UserSponsorMapping.Add(Emaratech.Services.Channels.Models.Enums.Constants.UserTypeLookup.ResidentUserType, SponsorType.Resident);
            UserSponsorMapping.Add(Emaratech.Services.Channels.Models.Enums.Constants.UserTypeLookup.EstablishmentUserType, SponsorType.Establishment);

            TravelTypeIsInsideMapping.Add(TravelType.Entry, "1");
            TravelTypeIsInsideMapping.Add(TravelType.Exit, "0");
        }
    }
}