using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using Emaratech.Services.Channels.Helpers.Lookups;
using log4net;

namespace Emaratech.Services.Channels.Workflows.Steps.Processing
{
    public class BuildSponsorInfoReportStep : ProcessingStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BuildSponsorInfoReportStep));
        private string SponsorFileNo { get; }

        private readonly ILookup professionLookup = LookupProfession.Instance;

        private readonly ILookup nationalityLookup = LookupNationality.Instance;

        private readonly ILookup maritalStatusLookup = LookupMaritalStatus.Instance;

        private readonly ILookup emirateLookup = LookupEmirate.Instance;
        private readonly ILookup cityLookup = LookupCity.Instance;

        private readonly ILookup areaLookup = LookupArea.Instance;


        private Action<SponsorInfo> StoreAction { get; }
             

        public BuildSponsorInfoReportStep(string sponsorFileNo, Action<SponsorInfo> argStore, ProcessingStep next) : base(next)
        {
            SponsorFileNo = sponsorFileNo;
            StoreAction = argStore;
        }

        protected override async Task Execute()
        {
            string address = string.Empty;



            Log.Debug($"SAS Report SponsorFileNo : {SponsorFileNo}");
            // var sponsorInfo = await ServicesHelper.GetIndividualDetailedInformation(EmiratesId, Convert.ToDateTime(DateOfBirth));
            var sponsorInfo = await ServicesHelper.GetIndividualDetailedInfoBySponsorNo(SponsorFileNo);

           

            var individualDependentsCount = await ServicesHelper.GetIndividualDependentSummary(sponsorInfo?.IndividualUserInfo?.IndividualSponsorshipInformation?.SponsorshipNo);
            //var name = $"{sponsorInfo.IndividualProfileInformation.FirstNameEn} {sponsorInfo.IndividualProfileInformation.LastNameEn}";

            int? totalVisaCount = individualDependentsCount.PermitTotalCount;
            int? totalVisaActive = individualDependentsCount.PermitActive;
            int? totalVisaAbscond = individualDependentsCount.PermitAbsconding;
            int? totalVisaClosed = individualDependentsCount.PermitClosed;


            int? totalResidentCount = individualDependentsCount.ResidenceTotalCount;
            int? totalResidentActive = individualDependentsCount.ResidenceActive;
            int? totalResidentAbscond = individualDependentsCount.ResidenceAbsconding;
            int? totalResidentClosed = individualDependentsCount.ResidenceClosed;

            var nameEn = $"{sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.FirstNameEn} {sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.LastNameEn}";
            var nameAr = $"{sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.FirstNameAr} {sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.LastNameAr}";
            
            var occupationAr = professionLookup.GetAr(sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());
            var nationalityAr = nationalityLookup.GetAr(sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.NationalityId.ToString());
            var maritalStatusAr = maritalStatusLookup.GetAr(sponsorInfo?.IndividualUserInfo?.IndividualProfileInformation?.SocialStatusId.ToString());
            var personNo = sponsorInfo.IndividualUserInfo?.IndividualProfileInformation?.PersonNo;
            // here we will fetch person_no  from api then assign it to new property of sponsorinfo .

            if (sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.Count>0)
            {
                StringBuilder addressBuilder = new StringBuilder();
                addressBuilder.Append($"{sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1).Building?.Replace(" ", "")} ");
                addressBuilder.Append($"{sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1).Street?.Replace(" ", "")} ");
                addressBuilder.Append($"{emirateLookup.GetEn(sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1)?.EmirateId.ToString())?.Replace(" ", "")} ");
                addressBuilder.Append($"{cityLookup.GetEn(sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1)?.CityId.ToString())?.Replace(" ", "")} ");
                addressBuilder.Append($"{areaLookup.GetEn(sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1)?.AreaId.ToString())?.Replace(" ", "")} ");
                addressBuilder.Append($"{sponsorInfo?.IndividualUserInfo?.IndividualAddressInfo?.FirstOrDefault(x => x.AddressTypeId == 1)?.POBOX}");
                address = addressBuilder.ToString();
            }
            StoreAction(new SponsorInfo(nameEn, nameAr, SponsorFileNo, address, false,
                                        totalVisaCount, totalVisaActive, totalVisaAbscond, totalVisaCount,
                                        totalResidentCount, totalResidentActive, totalResidentAbscond, totalResidentClosed, 
                                        occupationAr, nationalityAr, maritalStatusAr,sponsorInfo?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceIssueDate.Value.ToString("dd-MM-yyyy"),personNo));
        }
    }
}