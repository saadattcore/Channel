namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{
    using Contracts.Errors;
    using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
    using Emaratech.Services.Channels.Helpers;
    using Models.Enums;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;


    public abstract class VisaReportHandler : ReportHandler
    {
        protected const string dateFormat = "dd/MM/yyyy";
        protected const string permitIssuePlaceEn = "Dubai";
        protected const string permitIssuePlaceAr = "دبي";

        public abstract override Task<IList<ReportRecordInfo>> GetReportEntries(IList<string> ids);

        protected string GetEntryPermitReportData(
                  string applicationId,
                  Vision.Model.RestIndividualUserFormInfo permitInfo)
        {
            // Initiate entry permit report object and get permit information from vision
            var entryPermitReport = new NewEntryPermitReportData();

            var individualPermitNo = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitNo;
            var permitIssueDate = permitInfo.IndividualUserInfo.IndividualPermitInfo.PermitIssueDate.Value.ToString(dateFormat);

            // Set entry permit basic data
            entryPermitReport.EntryPermitNo = individualPermitNo;
            entryPermitReport.DateAndPlace = permitIssueDate + " " + permitIssuePlaceEn;
            entryPermitReport.DateAndPlaceAr = permitIssueDate + " " + permitIssuePlaceAr;
            entryPermitReport.Validity = permitInfo.IndividualUserInfo?.IndividualPermitInfo.PermitExpiryDate.Value.ToString(dateFormat);
            entryPermitReport.UidNo = permitInfo.IndividualUserInfo?.IndividualProfileInformation.UDBNo;
            entryPermitReport.FullName = permitInfo.IndividualUserInfo?.IndividualProfileInformation.FullNameEn;
            entryPermitReport.FullNameAr = permitInfo.IndividualUserInfo?.IndividualProfileInformation.FullNameAr;
            entryPermitReport.PlaceOfBirth = permitInfo.IndividualUserInfo?.IndividualProfileInformation.BirthPlaceEn;
            entryPermitReport.PlaceOfBirthAr = permitInfo.IndividualUserInfo?.IndividualProfileInformation.BirthPlaceAr;
            entryPermitReport.DateOfBirth = permitInfo.IndividualUserInfo?.IndividualProfileInformation.BirthDate.Value.ToString(dateFormat);
            entryPermitReport.EntryPermitNoFormated = GetFormattedPermitNo(individualPermitNo);
            entryPermitReport.BarcodeValue = GetBarcodeNumber(individualPermitNo);
            entryPermitReport.BarcodeEntryPermitNo = GetBarcodeNumber(individualPermitNo);
            entryPermitReport.SerialNo = applicationId;
            entryPermitReport.WifeName = "None";
            entryPermitReport.ChildName = "None";

            // Set nationality
            entryPermitReport.Nationality = LookupHelper.GetEn(
                LookupHelper.NationalityLookuptDetails,
                permitInfo.IndividualUserInfo?.IndividualProfileInformation.NationalityId.ToString());

            entryPermitReport.NationalityAr = LookupHelper.GetAr(
                LookupHelper.NationalityLookuptDetails,
                permitInfo.IndividualUserInfo?.IndividualProfileInformation.NationalityId.ToString());

            // Set passport info 
            var passportNumber = permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;

            var passportTypeEn = LookupHelper.GetEn(
                LookupHelper.PassportTypeLookupDetails,
                permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportTypeId.ToString());

            var passportTypeAr = LookupHelper.GetAr(
                LookupHelper.PassportTypeLookupDetails,
                permitInfo.IndividualUserInfo?.IndividualPassportInformation?.PassportTypeId.ToString());

            entryPermitReport.PassportNo = FormatString(passportTypeEn, passportNumber);
            entryPermitReport.PassportNoAr = FormatString(passportTypeAr, passportNumber);

            // Set Profession
            entryPermitReport.Profession = LookupHelper.GetEn(
                LookupHelper.ProfessionLookupDetails,
                permitInfo.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            entryPermitReport.ProfessionAr = LookupHelper.GetAr(
                LookupHelper.ProfessionLookupDetails,
                permitInfo.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            // Set sponsor data
            // TODO: For now it only for establishment user, it should be for individual and establishment also
            var establishmentInfo = ServicesHelper.GetEstablishmentProfile(
                permitInfo.IndividualUserInfo.IndividualPermitInfo.EstCode).Result;

            entryPermitReport.SponsorName = establishmentInfo.EstabNameEn;
            entryPermitReport.SponsorNameAr = establishmentInfo.EstabNameAr;
            entryPermitReport.SponsorAddress = "TEL: " + establishmentInfo.MobileNo + ", ADDRESS: " + establishmentInfo.AddressEn;

            // Set visa description
            var visaTypeDesc = LookupHelper.GetAr(
                LookupHelper.VisaTypeLookupDetails,
                permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString())
                + "\n" +
                LookupHelper.GetEn(
                    LookupHelper.VisaTypeLookupDetails,
                    permitInfo.IndividualUserInfo.IndividualPermitInfo?.VisaTypeId.ToString());
            entryPermitReport.VisaType = visaTypeDesc;

            return JsonConvert.SerializeObject(entryPermitReport);
        }
    }
}