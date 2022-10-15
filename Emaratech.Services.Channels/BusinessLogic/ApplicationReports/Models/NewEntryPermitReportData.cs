namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    using Newtonsoft.Json;

    public class NewEntryPermitReportData
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("entryPermitNo")]
        public string EntryPermitNo { get; set; }

        [JsonProperty("dateandplace")]
        public string DateAndPlace { get; set; }

        [JsonProperty("dateandplaceAr")]
        public string DateAndPlaceAr { get; set; }

        [JsonProperty("validity")]
        public string Validity { get; set; }

        [JsonProperty("uidno")]
        public string UidNo { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("fullNameAr")]
        public string FullNameAr { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("NationalityAr")]
        public string NationalityAr { get; set; }

        [JsonProperty("placeofbirth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("placeofbirthAr")]
        public string PlaceOfBirthAr { get; set; }

        [JsonProperty("dateofbirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("passportno")]
        public string PassportNo { get; set; }

        [JsonProperty("passportnoAr")]
        public string PassportNoAr { get; set; }

        [JsonProperty("profession")]
        public string Profession { get; set; }

        [JsonProperty("professionAr")]
        public string ProfessionAr { get; set; }

        [JsonProperty("visatype")]
        public string VisaType { get; set; }

        [JsonProperty("sponsorname")]
        public string SponsorName { get; set; }

        [JsonProperty("sponsornameAr")]
        public string SponsorNameAr { get; set; }

        [JsonProperty("sponsoraddress")]
        public string SponsorAddress { get; set; }

        [JsonProperty("entrypermitnoformated")]
        public string EntryPermitNoFormated { get; set; }

        [JsonProperty("barcodevalue")]
        public string BarcodeValue { get; set; }

        [JsonProperty("barcodeentrypermitno")]
        public string BarcodeEntryPermitNo { get; set; }

        [JsonProperty("serialNo")]
        public string SerialNo { get; set; }

        [JsonProperty("wifename")]
        public string WifeName { get; set; }

        [JsonProperty("childname")]
        public string ChildName { get; set; }
    }
}
