namespace Emaratech.Services.Channels.Reports.Models.Sns
{
    public class EntryPermitDetails : BaseDependentInfo
    {
        public string entrypermitNo { get; set; }
        public EntryPermitDetails(
            string argSerialNo,
            string argEntryPermitNo,
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
            string argTravelType) : base(argSerialNo, argVisaType, argName, argUdb, argGender,
            argPassNo, argNationality, argProfession, argIssuingDate, argExpiryDate, argStatus,argCancelDate,argCloseDate, argTravelDate,argTravelType)
        {
            entrypermitNo = argEntryPermitNo;
        }
    }
}