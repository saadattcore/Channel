using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json.Linq;
using Emaratech.Services.Vision.Model;
using Newtonsoft.Json;
using Emaratech.Services.Reporting.JasperReports;
using Emaratech.Services.Channels.Helpers.Lookups;
using System.Net.Http;
using System.Net.Http.Headers;
using Emaratech.Services.Channels.Workflows.Steps.Report;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class TravelReport
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TravelReport));


        private readonly ILookup genderLookup = LookupGender.Instance;
        private readonly ILookup nationalityLookup = LookupNationality.Instance;
        private readonly ILookup visaTypeLookup = LookupVisaType.Instance;
        private readonly ILookup residenceStatusLookup = LookupResidenceStatus.Instance;

        private readonly ILookup permitStatusLookup = LookupPermitStatus.Instance;
        private readonly ILookup professionLookup = LookupProfession.Instance;

        private readonly ILookup passportTypeLookup = LookupPassportType.Instance;

        private readonly ILookup maritalStatusLookup = LookupMaritalStatus.Instance;

        private readonly ILookup educationLookup = LookupEducation.Instance;

        private readonly ILookup religionLookup = LookupReligion.Instance;

        private readonly ILookup emirateLookup = LookupEmirate.Instance;
        private readonly ILookup cityLookup = LookupCity.Instance;
        private readonly ILookup areaLookup = LookupArea.Instance;

        private readonly ILookup residenceTypeLookup = LookupResidenceType.Instance;
        private readonly ILookup closeTypeLookup = LookupCloseType.Instance;

        private string FormatString(string en, string ar)
        {
            StringBuilder str = new StringBuilder();
            if (!string.IsNullOrEmpty(en))
            {
                str.Append(en);
            }
            if (!string.IsNullOrEmpty(ar))
            {
                str.Append(" / ");
                str.Append(ar);
            }
            return str.ToString();
        }

        private JObject BuildTravelHistoryDataResidence(RestIndividualUserFormInfo restIndivResInfo, RestIndividualTravelInfo restTravel)
        {
            var residenceDetail = restIndivResInfo.IndividualUserInfo.IndividualResidenceInfo;
            var profileDetail = restIndivResInfo.IndividualUserInfo.IndividualProfileInformation;
            var passportDetail = restIndivResInfo.IndividualUserInfo.IndividualPassportInformation;

            //visionData?.IndividualUserInfo.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);
            var indivAddressDetail = restIndivResInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);//.IndividualAddressInfo[0];

            var landLineContactDetail = restIndivResInfo.IndividualUserInfo?.IndividualContactDetails.FirstOrDefault(c => c.ContactTypeId == ContactType.LandLine.GetHashCode());
            var officeLineContactDetail = restIndivResInfo.IndividualUserInfo?.IndividualContactDetails.FirstOrDefault(c => c.ContactTypeId == ContactType.Office.GetHashCode());
            var sponsorOfficeContactDetails = restIndivResInfo?.IndividualSponsorInfo?.ContactInfo.FirstOrDefault(c => c.ContactTypeId == ContactType.Office.GetHashCode());

            var sponsorInfo = ServicesHelper.GetIndividualDetailedInfoBySponsorNo(restIndivResInfo?.IndividualSponsorInfo?.SponsorshipInfo?.SponsorshipNo).Result;

            // var address = visionData?.IndividualUserInfo.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);
            var sponsorAddressInfo = sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);

            var sponsorEmirateEn = emirateLookup.GetEn(sponsorAddressInfo?.EmirateId.ToString());
            var sponsorEmirateAr = emirateLookup.GetAr(sponsorAddressInfo?.EmirateId.ToString());

            var sponsorCityEn = cityLookup.GetEn(sponsorAddressInfo?.CityId.ToString());
            var sponsorCityAr = cityLookup.GetAr(sponsorAddressInfo?.CityId.ToString());

            var sponsorAreaEn = areaLookup.GetEn(sponsorAddressInfo?.AreaId.ToString());
            var sponsorAreaAr = areaLookup.GetAr(sponsorAddressInfo?.AreaId.ToString());

            var sponsorPoBox = sponsorAddressInfo?.POBOX;

            var sponsorProfileDetail = restIndivResInfo.IndividualSponsorInfo?.ProfileInfo;
            var spnAddress = sponsorInfo.IndividualUserInfo.IndividualAddressInfo.FirstOrDefault();


            var genderEn = genderLookup.GetEn(profileDetail.GenderId?.ToString());
            var genderAr = genderLookup.GetAr(profileDetail.GenderId?.ToString());

            var prsNationalityEn = nationalityLookup.GetEn(profileDetail?.NationalityId.ToString());
            var prsNationalityAr = nationalityLookup.GetAr(profileDetail?.NationalityId.ToString());

            var prvNationalityEn = nationalityLookup.GetEn(profileDetail?.PreviosNationalityId.ToString());
            var prvNationalityAr = nationalityLookup.GetAr(profileDetail?.PreviosNationalityId.ToString());

            var birthCountryEn = nationalityLookup.GetEn(profileDetail?.BirthCountryId.ToString());
            var birthCountryAr = nationalityLookup.GetAr(profileDetail?.BirthCountryId.ToString());

            var issueCountryEn = nationalityLookup.GetEn(passportDetail?.IssueCountryId.ToString());
            var issueCountryAr = nationalityLookup.GetAr(passportDetail?.IssueCountryId.ToString());


            var indivprofessionEn = professionLookup.GetEn(profileDetail?.ProfessionId.ToString());
            var indivprofessionAr = professionLookup.GetAr(profileDetail?.ProfessionId.ToString());

            var passportTypeEn = passportTypeLookup.GetEn(passportDetail?.PassportTypeId.ToString());
            var passportTypeAr = passportTypeLookup.GetAr(passportDetail?.PassportTypeId.ToString());

            var maritalStatusEn = maritalStatusLookup.GetEn(profileDetail?.SocialStatusId.ToString());
            var maritalStatusAr = maritalStatusLookup.GetAr(profileDetail?.SocialStatusId.ToString());

            var indivEducationEn = educationLookup.GetEn(profileDetail?.EducationId.ToString());
            var indivEducationAr = educationLookup.GetAr(profileDetail?.EducationId.ToString());

            var sponsorEducationEn = educationLookup.GetEn(sponsorProfileDetail?.EducationId.ToString());
            var sponsorEducationAr = educationLookup.GetAr(sponsorProfileDetail?.EducationId.ToString());

            var religionEn = religionLookup.GetEn(profileDetail?.ReligionId.ToString());
            var religionAr = religionLookup.GetAr(profileDetail?.ReligionId.ToString());


            var indivEmirateEn = emirateLookup.GetEn(indivAddressDetail?.EmirateId.ToString());
            var indivEmirateAr = emirateLookup.GetAr(indivAddressDetail?.EmirateId.ToString());

            var indivCityEn = cityLookup.GetEn(indivAddressDetail?.CityId.ToString());
            var indivCityAr = cityLookup.GetAr(indivAddressDetail?.CityId.ToString());

            var indivAreaEn = areaLookup.GetEn(indivAddressDetail?.AreaId.ToString());
            var indivAreaAr = areaLookup.GetAr(indivAddressDetail?.AreaId.ToString());


            var indivResTypeEn = visaTypeLookup.GetEn(residenceDetail?.VisaTypeId.ToString());
            var indivResTypeAr = visaTypeLookup.GetAr(residenceDetail?.VisaTypeId.ToString());

            var closeTypeEn = closeTypeLookup.GetEn(residenceDetail?.CloseTypeId.ToString());
            var closeTypeAr = closeTypeLookup.GetAr(residenceDetail?.CloseTypeId.ToString());

            var resStatusEn = residenceStatusLookup.GetEn(residenceDetail?.ResidenceStatusId.ToString());
            var resStatusAR = residenceStatusLookup.GetAr(residenceDetail?.ResidenceStatusId.ToString());

            JObject travelInfoResidence = new JObject();

            string resFile_No = residenceDetail.ResidenceNo;
            string resFileNo = resFile_No.Substring(0, 3) + "/" + resFile_No.Substring(3, 4) + "/" + resFile_No.Substring(7, 7);

            travelInfoResidence["RES_FIL_NO"] = resFileNo;
            travelInfoResidence["RES_TYPE"] = FormatString(indivResTypeEn, indivResTypeAr);//indivResTypeEn + " / " + indivResTypeAr;
            travelInfoResidence["RES_ISS_DT"] = residenceDetail?.ResidenceIssueDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["ORIG_ISS_DT"] = residenceDetail?.ResidenceOriginalIssueDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["RES_EXP_DT"] = residenceDetail?.ResidenceExpiryDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["FILE_STATUS_RES"] = FormatString(resStatusEn, resStatusAR);//resStatusEn + " / " + resStatusAR;
            travelInfoResidence["NM"] = profileDetail?.FullNameEn;
            travelInfoResidence["NM_E"] = profileDetail?.FullNameEn;
            travelInfoResidence["NM_A"] = profileDetail?.FullNameAr;
            travelInfoResidence["GENDER"] = FormatString(genderEn, genderAr);//genderEn+" / "+genderAr;

            //  travelInfoResidence["NO_ACC"] = profileDetail.GenderId; //// sub report data

            travelInfoResidence["ACC_COUNT"] = residenceDetail?.NumberOfDependents;

            travelInfoResidence["PRS_NAT"] = FormatString(prsNationalityEn, prsNationalityAr);//prsNationalityEn + " / "+ prsNationalityAr;
            travelInfoResidence["PRV_NAT"] = FormatString(prvNationalityEn, prvNationalityAr);//prvNationalityEn + " / " + prvNationalityAr; 
            travelInfoResidence["MAR_STA"] = FormatString(maritalStatusEn, maritalStatusAr);//maritalStatusEn + " / " + maritalStatusAr; 
            travelInfoResidence["BIRTH_CNTRY"] = FormatString(birthCountryEn, birthCountryAr);//birthCountryEn + " / " + birthCountryAr; 
            travelInfoResidence["DOB"] = profileDetail?.BirthDate?.ToString("dd/MM/yyyy");
            travelInfoResidence["BIRTH_PLC"] = FormatString(profileDetail.BirthPlaceEn, profileDetail.BirthPlaceAr);
            //profileDetail.BirthPlaceEn + " / " + profileDetail.BirthPlaceAr; 

            travelInfoResidence["PASS_NO"] = passportDetail?.PassportNumber;
            travelInfoResidence["PASS_TYPE"] = FormatString(passportTypeEn, passportTypeAr);///passportTypeEn + " / " + passportTypeAr;
            travelInfoResidence["PASS_ISS_DT"] = passportDetail?.IssueDate?.ToString("dd/MM/yyyy");
            travelInfoResidence["PASS_EXP_DT"] = passportDetail?.ExpiryDate?.ToString("dd/MM/yyyy");
            travelInfoResidence["PASS_CNTRY"] = FormatString(issueCountryEn, issueCountryAr);//issueCountryEn + " / " + issueCountryAr;
            travelInfoResidence["PASS_PLC"] = FormatString(passportDetail?.IssuePlaceEn, passportDetail?.IssuePlaceAr);
            //passportDetail?.IssuePlaceEn + " / " + passportDetail?.IssuePlaceAr;

            travelInfoResidence["EMIRT"] = FormatString(indivEmirateEn, indivEmirateAr);//indivEmirateEn + " / " + indivEmirateAr; 
            travelInfoResidence["CITY"] = FormatString(indivCityEn, indivCityAr);//indivCityEn + " / " + indivCityAr; 
            travelInfoResidence["AREA"] = FormatString(indivAreaEn, indivAreaAr);//indivAreaEn + " / " + indivAreaAr;
            travelInfoResidence["PO_BOX"] = indivAddressDetail?.POBOX;

            travelInfoResidence["RES_TEL_NO"] = landLineContactDetail?.CONTACTDETAIL;//landLineContactDetail?.CONTACTDETAIL;
            travelInfoResidence["OFF_TEL_NO"] = officeLineContactDetail?.CONTACTDETAIL;
            travelInfoResidence["PERM_ADRS_E"] = officeLineContactDetail?.CONTACTDETAIL;

            travelInfoResidence["PROF"] = FormatString(indivprofessionEn, indivprofessionAr);//indivprofessionEn + " / " + indivprofessionAr ;
            travelInfoResidence["EDUCATION"] = FormatString(indivEducationEn, indivEducationAr);// indivEducationEn + " / " + indivEducationAr;

            travelInfoResidence["NEW_SPNSR_FIL_NO"] = restIndivResInfo?.IndividualSponsorInfo?.SponsorshipInfo?.SponsorshipNo;
            travelInfoResidence["SPONSOR_NM"] = FormatString(sponsorProfileDetail?.FullNameEn, sponsorProfileDetail?.FullNameAr);
            //sponsorProfileDetail?.FullNameEn + " / " + sponsorProfileDetail?.FullNameAr;
            travelInfoResidence["EDUCATION"] = FormatString(sponsorEducationEn, sponsorEducationAr);//sponsorEducationEn +" / "+ sponsorEducationAr;

            travelInfoResidence["SPN_EMIRAT"] = FormatString(sponsorEmirateEn, sponsorEmirateAr);//sponsorEmirateEn + " / " + sponsorEmirateAr; ;
            travelInfoResidence["SPN_CITY"] = FormatString(sponsorCityEn, sponsorCityAr);//sponsorCityEn + " / " + sponsorCityAr; ;
            travelInfoResidence["SPN_AREA"] = FormatString(sponsorAreaEn, sponsorAreaAr);//sponsorAreaEn + " / " + sponsorAreaAr; ;

            travelInfoResidence["SPN_POBOX"] = sponsorPoBox;
            travelInfoResidence["SPN_PHONE"] = sponsorOfficeContactDetails?.CONTACTDETAIL;
            travelInfoResidence["APP_DATE"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            travelInfoResidence["BARCODE"] = residenceDetail?.ResidenceNo;//"0";
            travelInfoResidence["UDB_NO"] = profileDetail?.UDBNo;//restIndivResInfo?.IndividualUserInfo?.IndividualProfileInformation?.UDBNo;
            travelInfoResidence["CLOSE_TYPE"] = FormatString(closeTypeEn, closeTypeAr);//closeTypeEn + " / " + closeTypeAr;

            travelInfoResidence["ID_NO"] = profileDetail?.UDBNo;

            travelInfoResidence["RELIGION"] = FormatString(religionEn, religionAr);//religionEn + " / " + religionAr;

            travelInfoResidence["EntryExit"] = BuildEntryExitInfo(restTravel);//JArray.FromObject(BuildEntryExitInfo(restTravel));

            //travelInfoResidence["Accompany"] = JArray.FromObject(BuildResidenceAccompanyInfo(restTravel));

            return travelInfoResidence;
        }

        private JObject BuildTravelHistoryDataPermit(RestIndividualUserFormInfo restIndivPermitInfo, RestIndividualTravelInfo restTravel)
        {
            var permitDetail = restIndivPermitInfo.IndividualUserInfo?.IndividualPermitInfo;
            var profileDetail = restIndivPermitInfo.IndividualUserInfo?.IndividualProfileInformation;
            var passportDetail = restIndivPermitInfo.IndividualUserInfo?.IndividualPassportInformation;

            var addressDetail = restIndivPermitInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);//.IndividualAddressInfo[0];
            var sponsorProfileDetail = restIndivPermitInfo.IndividualSponsorInfo?.ProfileInfo;

            var landLineContactDetail = restIndivPermitInfo.IndividualUserInfo?.IndividualContactDetails.FirstOrDefault(c => c.ContactTypeId == ContactType.LandLine.GetHashCode());
            var officeLineContactDetail = restIndivPermitInfo.IndividualUserInfo?.IndividualContactDetails.FirstOrDefault(c => c.ContactTypeId == ContactType.Office.GetHashCode());
            var sponsorOfficeContactDetails = restIndivPermitInfo?.IndividualSponsorInfo?.ContactInfo.FirstOrDefault(c => c.ContactTypeId == ContactType.Office.GetHashCode());

            var sponsorInfo = ServicesHelper.GetIndividualDetailedInfoBySponsorNo(restIndivPermitInfo?.IndividualSponsorInfo?.SponsorshipInfo?.SponsorshipNo).Result;
            var sponsorAddressInfo = sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);

            var sponsorEmirateEn = emirateLookup.GetEn(sponsorAddressInfo?.EmirateId.ToString());
            var sponsorEmirateAr = emirateLookup.GetAr(sponsorAddressInfo?.EmirateId.ToString());

            var sponsorCityEn = cityLookup.GetEn(sponsorAddressInfo?.CityId.ToString());
            var sponsorCityAr = cityLookup.GetAr(sponsorAddressInfo?.CityId.ToString());

            var sponsorAreaEn = areaLookup.GetEn(sponsorAddressInfo?.AreaId.ToString());
            var sponsorAreaAr = areaLookup.GetAr(sponsorAddressInfo?.AreaId.ToString());

            var sponsorPoBox = sponsorAddressInfo?.POBOX;


            var genderEn = genderLookup.GetEn(profileDetail?.GenderId.ToString());
            var genderAr = genderLookup.GetAr(profileDetail?.GenderId.ToString());

            var prsNationalityEn = nationalityLookup.GetEn(profileDetail?.NationalityId.ToString());
            var prsNationalityAr = nationalityLookup.GetAr(profileDetail?.NationalityId.ToString());

            var prvNationalityEn = nationalityLookup.GetEn(profileDetail?.PreviosNationalityId.ToString());
            var prvNationalityAr = nationalityLookup.GetAr(profileDetail?.PreviosNationalityId.ToString());

            var birthCountryEn = nationalityLookup.GetEn(profileDetail?.BirthCountryId.ToString());
            var birthCountryAr = nationalityLookup.GetAr(profileDetail?.BirthCountryId.ToString());

            var issueCountryEn = nationalityLookup.GetEn(passportDetail?.IssueCountryId.ToString());
            var issueCountryAr = nationalityLookup.GetAr(passportDetail?.IssueCountryId.ToString());


            var indivprofessionEn = professionLookup.GetEn(profileDetail?.ProfessionId.ToString());
            var indivprofessionAr = professionLookup.GetAr(profileDetail?.ProfessionId.ToString());


            var passportTypeEn = passportTypeLookup.GetEn(passportDetail?.PassportTypeId.ToString());
            var passportTypeAr = passportTypeLookup.GetAr(passportDetail?.PassportTypeId.ToString());

            var maritalStatusEn = maritalStatusLookup.GetEn(profileDetail?.SocialStatusId.ToString());
            var maritalStatusAr = maritalStatusLookup.GetAr(profileDetail?.SocialStatusId.ToString());

            var indivEducationEn = educationLookup.GetEn(profileDetail?.EducationId.ToString());
            var indivEducationAr = educationLookup.GetAr(profileDetail?.EducationId.ToString());

            var sponsorEducationEn = educationLookup.GetEn(sponsorProfileDetail?.EducationId.ToString());
            var sponsorEducationAr = educationLookup.GetAr(sponsorProfileDetail?.EducationId.ToString());

            var religionEn = religionLookup.GetEn(profileDetail?.ReligionId.ToString());
            var religionAr = religionLookup.GetAr(profileDetail?.ReligionId.ToString());


            var indivEmirateEn = emirateLookup.GetEn(addressDetail?.EmirateId.ToString());
            var indivEmirateAr = emirateLookup.GetAr(addressDetail?.EmirateId.ToString());

            var indivCityEn = cityLookup.GetEn(addressDetail?.CityId.ToString());
            var indivCityAr = cityLookup.GetAr(addressDetail?.CityId.ToString());

            var indivAreaEn = areaLookup.GetEn(addressDetail?.AreaId.ToString());
            var indivAreaAr = areaLookup.GetAr(addressDetail?.AreaId.ToString());

            var indivVisaTypeEn = visaTypeLookup.GetEn(permitDetail?.VisaTypeId.ToString());
            var indivVisaTypeAr = visaTypeLookup.GetAr(permitDetail?.VisaTypeId.ToString());

            var indivePermitFileTypeEn = residenceTypeLookup.GetEn(permitDetail?.VisaTypeId.ToString());
            var indivePermitFileTypeAr = residenceTypeLookup.GetAr(permitDetail?.VisaTypeId.ToString());

            var closeTypeEn = closeTypeLookup.GetEn(permitDetail?.CloseTypeId.ToString());
            var closeTypeAr = closeTypeLookup.GetAr(permitDetail?.CloseTypeId.ToString());

            var visaStatusEn = permitStatusLookup.GetEn(permitDetail?.PermitStatusId.ToString());
            var visaStatusAR = permitStatusLookup.GetAr(permitDetail?.PermitStatusId.ToString());

            JObject travelInfoResidence = new JObject();

            travelInfoResidence["NEW_RES_FIL_NO"] = permitDetail?.PermitNo;
            travelInfoResidence["VISA_FIL_NO"] = permitDetail?.PermitNo;
            travelInfoResidence["VISA_TYPE"] = FormatString(indivVisaTypeEn, indivVisaTypeAr);//indivVisaTypeEn + " / " + indivVisaTypeAr;//permitDetail?.VisaType;
            travelInfoResidence["VISA_ISSUE_DT"] = permitDetail?.PermitIssueDate.Value.ToString("dd/MM/yyyy");
            //  travelInfoResidence["VALIDITY_DT"] = permitDetail.PermitIssueDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["VALIDITY_DT"] = permitDetail?.PermitExpiryDate.Value.ToString("dd/MM/yyyy");
            // travelInfoResidence["FILE_STATUS"] = indivePermitFileTypeEn.PermitStatusId;
            travelInfoResidence["FILE_STATUS_EP"] = FormatString(visaStatusEn, visaStatusAR);///visaStatusEn + " / " + visaStatusAR;
            travelInfoResidence["NM"] = profileDetail?.FullNameEn;
            travelInfoResidence["NM_E"] = profileDetail?.FullNameEn;
            travelInfoResidence["NM_A"] = profileDetail?.FullNameAr;
            travelInfoResidence["GENDER"] = FormatString(genderEn, genderAr);//genderEn + " / " + genderAr;

            //  travelInfoResidence["NO_ACC"] = profileDetail.GenderId; //// sub report data

            travelInfoResidence["LAST_ACC_NO"] = permitDetail?.NumberOfDependents;
            travelInfoResidence["ACC_COUNT"] = permitDetail?.NumberOfDependents;

            travelInfoResidence["PRS_NAT"] = FormatString(prsNationalityEn, prsNationalityAr);//prsNationalityEn + " / " + prsNationalityAr;
            travelInfoResidence["PRV_NAT"] = FormatString(prvNationalityEn, prvNationalityAr);//prvNationalityEn + " / " + prvNationalityAr; ;
            travelInfoResidence["MAR_STA"] = FormatString(maritalStatusEn, maritalStatusAr);//maritalStatusEn + " / " + maritalStatusAr; ;
            travelInfoResidence["BIRTH_CNTRY"] = FormatString(birthCountryEn, birthCountryAr);//birthCountryEn + " / " + birthCountryAr;
            travelInfoResidence["DOB"] = profileDetail?.BirthDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["BIRTH_PLC"] = FormatString(profileDetail?.BirthPlaceEn, profileDetail?.BirthPlaceAr);
            //profileDetail?.BirthPlaceEn + " / " + profileDetail?.BirthPlaceAr;
            travelInfoResidence["PASS_NO"] = passportDetail?.PassportNumber;
            travelInfoResidence["PASS_TYPE"] = FormatString(passportTypeEn, passportTypeAr);//passportTypeEn + " / " + passportTypeAr;
            travelInfoResidence["PASS_DT"] = passportDetail?.IssueDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["PASS_EXP_DT"] = passportDetail?.ExpiryDate.Value.ToString("dd/MM/yyyy");
            travelInfoResidence["PASS_CNTRY"] = FormatString(issueCountryEn, issueCountryAr);
            //issueCountryEn + " / " + issueCountryAr;
            travelInfoResidence["PASS_PLC"] = FormatString(passportDetail?.IssuePlaceEn, passportDetail?.IssuePlaceAr);
            //passportDetail?.IssuePlaceEn +" / " + passportDetail?.IssuePlaceAr;

            travelInfoResidence["EMIRT"] = FormatString(indivEmirateEn, indivEmirateAr);//indivEmirateEn + " / " + indivEmirateAr;
            travelInfoResidence["CITY"] = FormatString(indivCityEn, indivCityAr);//indivCityEn + " / " + indivCityAr;
            travelInfoResidence["AREA"] = FormatString(indivAreaEn, indivAreaAr);//indivAreaEn + " / " + indivAreaAr;
            travelInfoResidence["PO_BOX"] = addressDetail?.POBOX;

            travelInfoResidence["RES_TEL_NO"] = landLineContactDetail?.CONTACTDETAIL;
            travelInfoResidence["OFF_TEL_NO"] = officeLineContactDetail?.CONTACTDETAIL;
            travelInfoResidence["PERM_ADRS_E"] = landLineContactDetail?.CONTACTDETAIL;

            travelInfoResidence["PROF"] = FormatString(indivprofessionEn, indivprofessionAr);//indivprofessionEn + " / " + indivprofessionAr;
            travelInfoResidence["EDUCATION"] = FormatString(indivEducationEn, indivEducationAr);//indivEducationEn + " / " + indivEducationAr;

            travelInfoResidence["NEW_SPNSR_FIL_NO"] = restIndivPermitInfo?.IndividualSponsorInfo?.SponsorshipInfo?.SponsorshipNo;
            travelInfoResidence["SPONSOR_NM"] = FormatString(sponsorProfileDetail?.FullNameEn, sponsorProfileDetail?.FullNameAr);
            //sponsorProfileDetail?.FullNameEn + " / " + sponsorProfileDetail?.FullNameAr;
            travelInfoResidence["EDUCATION"] = FormatString(sponsorEducationEn, sponsorEducationAr);//sponsorEducationEn + " / " + sponsorEducationAr;

            travelInfoResidence["SPN_EMIRAT"] = FormatString(sponsorEmirateEn, sponsorEmirateAr);//sponsorEmirateEn + " / " + sponsorEmirateAr;
            travelInfoResidence["SPN_CITY"] = FormatString(sponsorCityEn, sponsorCityAr);//sponsorCityEn + " / " + sponsorCityAr;
            travelInfoResidence["SPN_AREA"] = FormatString(sponsorAreaEn, sponsorAreaAr);//sponsorAreaEn + " / " + sponsorAreaAr;

            travelInfoResidence["SPN_POBOX"] = sponsorPoBox;
            travelInfoResidence["SPN_PHONE"] = sponsorOfficeContactDetails?.CONTACTDETAIL;
            travelInfoResidence["APP_DATE"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            travelInfoResidence["CURRENT_DATE"] = DateTime.Now.ToString("dd/MM/yyyy");

            travelInfoResidence["BARCODE"] = permitDetail?.PermitNo;
            travelInfoResidence["UDB_NO"] = profileDetail?.UDBNo;//sponsorProfileDetail?.UDBNo;

            travelInfoResidence["CLOSE_TYPE"] = FormatString(closeTypeEn, closeTypeAr);//closeTypeEn + " / " + closeTypeAr;
            travelInfoResidence["ID_NO"] = profileDetail?.UDBNo;//sponsorProfileDetail?.UDBNo;

            travelInfoResidence["RELIGION"] = FormatString(religionEn, religionAr);
            //religionEn + " / " + religionAr;

            travelInfoResidence["EntryExit"] = BuildEntryExitInfo(restTravel);//JArray.FromObject(BuildEntryExitInfo(restTravel));

            //travelInfoResidence["Accompany"] = JArray.FromObject(BuildResidenceAccompanyInfo(restTravel));

            return travelInfoResidence;
        }

        private JObject BuildEntryExitInfo(RestIndividualTravelInfo restTravel)
        {
            var travelInfoObj = new JObject();

            travelInfoObj["TRL_TP"] = restTravel?.TravelTypeId;//travelEn + " / " + travelAr;
            travelInfoObj["TRL_DT"] = restTravel?.TravelDate.Value.ToString("dd/MM/yyyy HH:mm:ss");
            travelInfoObj["travel_dttime"] = restTravel?.TravelDate.Value.ToString("dd/MM/yyyy HH:mm");
            travelInfoObj["DEST_CD"] = FormatString(restTravel?.DestinationEn, restTravel?.DestinationAr);
            travelInfoObj["FLGHT_NO"] = FormatString(restTravel?.PortEn, restTravel?.PortAr);
            travelInfoObj["TRL_ACC"] = restTravel?.AccompanyCount;
            travelInfoObj["TRL_PRT"] = FormatString(restTravel?.PortEn, restTravel?.PortAr);//restTravel?.PortEn + " / " + restTravel?.PortEn;
            return travelInfoObj;
        }

        public async Task<string> Generate(string residenceNo,string permitNo)
        {
            if (residenceNo != null)
            {
                Log.Debug($"Travel Record for Residence no : {residenceNo}");
                var residenceInfo = await ServicesHelper.GetIndividualProfileByResidenceNo(residenceNo);
                // var travelHisotryInfoResidence = await ServicesHelper.GetIndividualTravelHistoryByResNo(ResidenceNo.Get());
                //var travelRecord = await ServicesHelper.GetIndividualCurrentTravelStatusByPpsId(residenceInfo.IndividualUserInfo.IndividualResidenceInfo.PPSID);
                var travelRecord = await ServicesHelper.GetIndividualCurrentTravelStatusByResNo(residenceNo);

                Log.Debug($"Travel Record for Redisence no : {residenceNo}. ====  {JsonConvert.SerializeObject(travelRecord)}");

                var reportDatajObject = BuildTravelHistoryDataResidence(residenceInfo, travelRecord);
                var jsonData = JsonConvert.SerializeObject(reportDatajObject);
                Log.Debug("Got data from travel history data residence");

                Log.Debug("Got data from travel history date residence");
                Log.Debug($"Report Data for travel status report ==>  {jsonData}");
                return jsonData;

            }
            else
            {

                Log.Debug($"Travel Record for Permit no : {permitNo}");
                var permitInfo = await ServicesHelper.GetIndividualProfileByPermitNo(permitNo);
                // var travelHisotryInfoPermit = await ServicesHelper.GetIndividualTravelHistoryByPermitNo(PermitNo.Get());
                // var travelRecord = await ServicesHelper.GetIndividualCurrentTravelStatusByPpsId(permitInfo.IndividualUserInfo.IndividualPermitInfo.PPSID);
                var travelRecord = await ServicesHelper.GetIndividualCurrentTravelStatusByPermitNo(permitNo);

                Log.Debug($"Travel Record details for Permit no : {permitNo}. ====  {JsonConvert.SerializeObject(travelRecord)}");
                var reportDatajObject = BuildTravelHistoryDataPermit(permitInfo, travelRecord);

                var jsonData = JsonConvert.SerializeObject(reportDatajObject);
                Log.Debug($"Report Data for travel status report ==>  {jsonData}");
                return jsonData;


            }
        }

    }

    public class GenerateTravelStatusReport : ChannelWorkflowStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GenerateTravelStatusReport));
        #region lookup section


        #endregion
        public InputParameter<string> ResidenceNo { get; set; } 

        public InputParameter<string> PermitNo { get; set; }

        public InputParameter<string> ReportEmailAddress { get; set; }

        public ReferenceParameter<JObject> ApplicationDataToSave { get; set; }
        public OutputParameter ReportData { get; set; }
      
        public OutputParameter FileType { get; set; }

        string visaNumber=string.Empty;
    
        public GenerateTravelStatusReport()
        {
            ReportData = new OutputParameter(nameof(ReportData));

           FileType = new OutputParameter(nameof(FileType));
           // ApplicationDataToSave = new ReferenceParameter<JObject>("ReportDetails", <>);


        }

        public override async Task<WorkflowStepState> Execute()
        {
            await base.Execute();

           // var ppsId = PPSID.Get();

            //Log.Debug($"PPS ID For Certificate of Entry Exit : {ppsId}.");

            CheckRequiredInput(ResidenceNo);

            if (ParametersRequiringInput.Count > 0)
            {
                CheckRequiredInput(PermitNo);

                if (ParametersRequiringInput.Count > 1)
                    return StepState = WorkflowStepState.InputRequired;
            }


            var reportDatajObject = new JObject();

            if (ResidenceNo != null && ResidenceNo.IsFilled())
            {
                var residenceInfo = await ServicesHelper.GetIndividualProfileByResidenceNo(ResidenceNo.Get());
                var profileDetail = residenceInfo.IndividualUserInfo.IndividualProfileInformation;
                reportDatajObject["NM_E"] = profileDetail?.FullNameEn;
                reportDatajObject["NM_A"] = profileDetail?.FullNameAr;
                FileType.Set(ServiceType.Residence.GetHashCode().ToString());
                visaNumber = ResidenceNo.Get();
            }
            else
            {
                var permitInfo = await ServicesHelper.GetIndividualProfileByPermitNo(PermitNo.Get());
                var profileDetail = permitInfo.IndividualUserInfo?.IndividualProfileInformation;
                reportDatajObject["NM_E"] = profileDetail?.FullNameEn;
                reportDatajObject["NM_A"] = profileDetail?.FullNameAr;
                FileType.Set(ServiceType.EntryPermit.GetHashCode().ToString());
                visaNumber = PermitNo.Get();


            }
            SetOutputValues(reportDatajObject);
            return StepState = WorkflowStepState.Done;
        }


        private void SetOutputValues(JObject reportData)
        {
            ApplicationDataToSave.Value["ReportDetails"]["ReportData"] = null;
            ApplicationDataToSave.Value["ReportDetails"]["FileType"] = FileType.Get().ToString();
            ApplicationDataToSave.Value["ReportDetails"]["Email"] = ReportEmailAddress.Get();
            ApplicationDataToSave.Value["ReportDetails"]["VisaNumber"] = visaNumber;
            ApplicationDataToSave.Value["ApplicantDetails"]["FullNameE"] = reportData["NM_E"].ToString();
            ApplicationDataToSave.Value["ApplicantDetails"]["FullNameA"] = reportData["NM_A"].ToString();
        }

        
    }
}
