using System;

namespace Emaratech.Services.Channels.Reports.Models.Sns
{
    public class BaseDependentInfo : MarshalByRefObject
    {
        public string sno { get; set; }
        public string visaType { get; set; }
        public string name { get; set; }
        public string udbNo { get; set; }
        public string gender { get; set; }
        public string passNo { get; set; }
        public string nationality { get; set; }
        public string profession { get; set; }
        public string issueDate { get; set; }
        public string expDate { get; set; }
        public string status { get; set; }

        public string cancelDate { get; set; }
        public string closeDate { get; set; }
        public string travelDate { get; set; }

        public string travelType { get; set; }
        public BaseDependentInfo(string argSerialNo,
            string argVisaType,
            string argName,
            string argUdb,
            string argGender,
            string argPassNo,
            string argNationality,
            string argProfession,
            string argIssuingDate,
            string argExpiryDate,
            string argStatus,
            string argCancelDate,
            string argCloseDate,
            string argTravelDate,
            string argTravelType)
        {
            sno = argSerialNo;
            visaType = argVisaType;
            name = argName;
            udbNo = argUdb;
            gender = argGender;
            passNo = argPassNo;
            nationality = argNationality;
            profession = argProfession;
            issueDate = argIssuingDate;
            expDate = argExpiryDate;
            status = argStatus;
            cancelDate = argCancelDate;
            closeDate = argCloseDate;
            travelDate = argTravelDate;
            travelType = argTravelType;

        }
    }
}
