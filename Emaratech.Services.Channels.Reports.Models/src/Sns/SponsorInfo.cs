using System;

namespace Emaratech.Services.Channels.Reports.Models.Sns
{
    public class SponsorInfo : MarshalByRefObject
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string FileNo { get; set; }
        public string Address { get; set; }
        public bool Ban { get; set; }
        public string isBannedEstablishment { get; set; }
        public int? TotalVisaCount { get; set; }
        public int? TotalVisaActive { get; set; }
        public int? TotalVisaAbscond { get; set; }
        public int? TotalVisaClosed { get; set; }
        public int? TotalResidentCount { get; set; }
        public int? TotalResidentActive { get; set; }
        public int? TotalResidentAbscond { get; set; }
        public int? TotalResidentClosed { get; set; }

        public string Occupation { get; set; }

        public string Nationality { get; set; }

        public string MaritalStatus { get; set; }

        public string RegistrationDate { get; set; }

        public string PersonNo { get; set; }

        public SponsorInfo(string argNameEn, string argNameAr, string argFileNumber, string argAddress, bool argBan,
            int? totalVisaCount, int? totalVisaActive, int? totalVisaAbscond, int? totalVisaClosed,
            int? totalResidentCount, int? totalResidentActive, int? totalResidentAbscond, int? totalResidentClosed,
            string argOccupation, string argNationality, string argMaritalStatus, string argRegistrationDate , string personNo)
        {
            NameEn = argNameEn;

            NameAr = argNameAr;

            FileNo = argFileNumber;

            Address = argAddress;

            Ban = argBan;
            isBannedEstablishment = argBan.ToString().ToLower();
            TotalVisaCount = totalVisaCount;
            TotalVisaActive = totalVisaActive;
            TotalVisaAbscond = totalVisaAbscond;
            TotalVisaClosed = totalVisaClosed;
            TotalResidentCount = totalResidentCount;
            TotalResidentActive = totalResidentActive;
            TotalResidentAbscond = totalResidentAbscond;
            TotalResidentClosed = totalResidentClosed;
            Occupation = argOccupation;
            Nationality = argNationality;
            MaritalStatus = argMaritalStatus;
            RegistrationDate = argRegistrationDate;
            PersonNo = personNo;
        }
    }
}