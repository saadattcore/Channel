using System.Collections.Generic;

namespace Emaratech.Services.Channels.Models.Enums
{
    public static class Constants
    {
        internal static class ConfigurationKeys
        {
            public const string UserTypeLocalLookupResult = "userTypeLocalLookupResult";
            public const string ApplicationRefundStatuses = nameof(ApplicationRefundStatuses);
            public const string WarrantyRefundStatuses = nameof(WarrantyRefundStatuses);
            public const string RefundListStatuses = nameof(RefundListStatuses);
            public const string IsUsedEmiratesIdAllowed = nameof(IsUsedEmiratesIdAllowed);
            public const string EntryPermitApplicationType = "Print_eVisa_EntryPermitApplicationType";
            public const string ApplicationPrintStatuses = "Print_eVisa_ApplicationStatuses";
            public const string EntryPermitPrintStatuses = "Print_eVisa_PermitStatuses";
            public const string PrintPermissions = "Print_eVisa_Permissions";
            public const string PrintPhotoDocumnetId = "Print_eVisa_SponsoredPhotoDocId";

            public const string Print_PermitCancellation_Statuses = "Print_PermitCancellation_Statuses";
            public const string Print_ResidenceCancellation_Statuses = "Print_ResidenceCancellation_Statuses";
        }

        public const string EmiratesId = "EmiratesId";
        public const string Mobile = "Mobile";
        public const string FileTypeId = "FileTypeId";
        public const string Email = "Email";
        public const string FirstNameEn = "FirstNameEn";
        public const string FirstNameAr = "FirstNameAr";
        public const string PassportNo = "PassportNo";
        public const string NationalityId = "NationalityId";
        public const string GenderId = "GenderId";
        public const string DateOfBirth = "DateOfBirth";
        public const string VisaNumber = "VisaNumber";
        public static readonly IList<int> LstGccCountries = new[] { 103, 105, 107, 109, 111 };
        public const int UaeCountryId = 101;
        public const int DubaiDepartmentStart = 2;
        public const string IndividualUserNotFoundErrorCode = "IndividualUserNotFound";
        public const string IndividualUserMobileNotFoundErrorCode = "IndividualUserMobileNotFound";
        public const string FileType = "FileType";
        public const string KeyVaultToken = "KeyVault.Token";
        public const string Ppsid = "PPSID";
        public const string SponsorNo = "SponsorNo";
        public const string UserType = "UserType";
        public const string SponsorType = "SponsorType";
        public const string EstablishmentType = "EstablishmentType";
        public const string UDBNo = "UDBNo";
        public const string EstablishmentCode = "estab_code";
        public const string EstablishmentStatusEn = "estab_statusEn";
        public const string EstablishmentStatusAr = "estab_statusAr";
        public const string Username = "username";
        public const string BktRefundType = "BKT";
        public const string JobId = "JobId";
        public const string FieldTypeTextBox = "00000000000000000000000000000031";
        public const string FieldTypeSummary = "00000000000000000000000000000034";
        public const string MaxLengthFieldValidation = "00000000000000000000000000000003";
        public const string UnAuthorized = "Unauthorized";

        public class Services
        {
            public const string ResidenceRenew = "745F4FA7B13B49F392BAB81C980DB7E8";
            public const string EntryPermitNewResidenceService = "B4D5063747EB42029035E533BBEB426E";
            public const string EntryPermitNewLeisureShortSingleService = "AC2994B7A6A141BF9DBF7A4B2C01ACAB";
            public const string EntryPermitNewLeisureShortMultiService = "94AE97BD791A43729D7F10D9710DAD8A";
            public const string EntryPermitNewLeisureLongMultiService = "C3106AFBFBAD4099B1B4DA6EA3025329";
            public const string EntryPermitNewLeisureLongSingleService = "B1BF267DA3854734B6EA8DCA7BD9EA06";
            public const string EntryPermitCancelPrivateService = "F5DD2E421EF648869A01160053124F5D";
            public const string ResidenceCancelService = "FB6820583AFD4EF0AB5A5DB9DB289603";
            public const string ResidenceNewService = "862FC0387B1D4055801B2E2594FE3530";
        }

        public class ParentCategories
        {
            public const string EntryPermit = "948CECE51468461B944EE09E7B609102";
            public const string Residence = "D2FCCA0D10B34596AD3C51A23CA32448";
        }

        public class SystemProperties
        {
            public const string ResidencyPickupLookupIdKey = "Lookups.ResidencyPickup";
            public const string YearsOfResidenceLookupIdKey = "Lookups.YearsOfResidence";
            public const string InsideCountryLookupIdKey = "Lookups.InsideCountry";
            public const string PaymentURL = "Payments.MobileUrl";
            public const string PaymentRedirectUrl = "Payments.RedirectUrl";
            public const string MappingMatrixApplicationFieldsIdKey = "MappingMatrix.ApplicationFieldsId";
            public const string MappingMatrixApplicationDefaultValuesIdKey = "ApplicationDefaultValuesMatrix";
            public const string MappingMatrixDataContractIdKey = "MappingMatrix.DataContractId";
            public const string FetchEntryPermitDataActionLookupIdKey = "Lookups.FetchEntryPermitDataAction";
        }

        public class Forms
        {
            public const string SponsorFileFormId = "5A808E4AED894D8983902010A3EE1E60";
            public const string OnArrivalExtensionFormId = "4C470337CFAB4461BB36A411726A8E35";
            public const string PassportRenewFormId = "9DC5D56613724CCDADF03090F1040A28";
            public const string EntryPermitAutofillFormId = "599D585D1CF14E8FAA58D06257C70769";
        }

        public class Fees
        {
            public const string DeliveryFeeId = "DA51494F7FA940A4AE6271888DD9A526";
            public const string ApplicationFeeId = "FDA0CBEED89A40B6BB98A7CD62F7517C";
            public const string IssuanceFeeId = "5C7C1042D7CD43F2BA4BB0B70D744A65";
        }

        public class Lookups
        {
            public const string SponsorFileLookupId = "9DF3C51B547E46FB8A1F0D2B875BDF54";
            public const string UserTypeLookupId = "7A2EAED0BF88408B80A85001F468CC19";
            public const string CountryLookupId = "9E863B86C5234DC1A8C2829DCC1F6EF7";
        }

        public class LanguagesLookup
        {
            public const string ArabicLanguage = "1";
            public const string EnglishLanguage = "2";
        }

        public class PickupLookup
        {
            public const string ZajelDelivery = "C7D97783C4F146F281D4AF91B5A61DED";
            public const string SelfCollection = "FAD96F14B47C4A529DD6B40EAD46A0DF";
        }

        public class UserTypeLookup
        {
            public const string CitizenUserType = "20252777362A43918F1C9108342342AB";
            public const string ResidentUserType = "230AAC5F9259481F95A92AAC60F5D705";
            public const string EstablishmentUserType = "93BECEED174543698C4E3F9A300C1878";
            public const string GCCUserType = "EC92A0F279A04EAD8746DB8B35BD1438";
            public const string GCCMOIUserType = "9B353C88E7D44F988B228B3B6119DE82";
            public const string TypingCenterUserType = "0E4898C5737542DE8045F3386480F147";
            public const string SuperAdminUserType = "3F23D2324C714FF6BEED4EC2503D12D3";
        }

        public class Platforms
        {
            public const string WebPlatform = "00000000000000000000000000000001";
            public const string MobilePlatform = "00000000000000000000000000000002";
        }
    }
}