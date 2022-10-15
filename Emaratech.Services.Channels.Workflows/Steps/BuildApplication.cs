using Emaratech.Services.Vision.Model;
using Emaratech.Services.Workflows.Engine;
using log4net;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class BuildApplication : ChannelWorkflowStep
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(BuildApplication));

        public InputParameter<string> UserId { get; set; }
        public InputParameter<string> UserType { get; set; }
        public InputParameter<string> SystemId { get; set; }
        public InputParameter<string> ServiceId { get; set; }
        public InputParameter<string> SponsorNo { get; set; }
        public InputParameter<string> SponsorType { get; set; }
        public OutputParameter UnifiedApplication { get; set; }
        public OutputParameter PPSID { get; set; }
        public WorkflowInstanceContext Context { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            UnifiedApplication = new OutputParameter(nameof(UnifiedApplication));
            PPSID = new OutputParameter(nameof(PPSID));
        }

        protected async Task BuildUnifiedApplicationDocument(RestIndividualUserFormInfo visionData)
        {

            var tasks = await Task.WhenAll(BuildApplicationDetails(visionData),
                BuildApplicantDetails(visionData),
                BuildSponsorDetails(visionData),
                BuildReportDetails());
            JObject application = new JObject()
            {
                {"ApplicationDetails", tasks[0]},
                {"ApplicantDetails", tasks[1]},
                {"SponsorDetails", tasks[2]},
                {"ReportDetails", tasks[3]}
            };

            UnifiedApplication.Set(application);
        }

        protected virtual  Task<JObject> BuildApplicationDetails(RestIndividualUserFormInfo visionData)
        {
            JObject details = new JObject
            {
                {"ApplicationId", null},
                {"ServiceId", ServiceId.Get()},
                {"UserID", UserId.Get()},
                {"ApplicationType", null},
                {"ApplicationSubType", null},
                {"IsUrgent", null},
                {"PickedDate", null},
                {"IsPassportUpdated", null},
                {"ResidencyPickup", null},
                {"SystemId", SystemId?.Get()},
                {"WorkflowToken", Context?.WorkflowInstanceId}
            };

            return Task.FromResult(details);
        }

        protected  virtual  Task<JObject> BuildApplicantDetails(RestIndividualUserFormInfo visionData)
        {
            var profile = visionData?.IndividualUserInfo.IndividualProfileInformation;
            var passport = visionData?.IndividualUserInfo.IndividualPassportInformation;
            var address = visionData?.IndividualUserInfo.IndividualAddressInfo.FirstOrDefault(x => x.AddressTypeId == 1);

            JObject details = new JObject
            {
                {"ApplicationId", null},
                {"FirstNameA", profile?.FirstNameAr},
                {"MiddleNameA", profile?.SecondNameAr},
                {"LastNameA", profile?.LastNameAr},
                {"FullNameA", profile?.FullNameAr},
                {"FirstNameE", profile?.FirstNameEn},
                {"MiddleNameE", profile?.SecondNameEn},
                {"LastNameE", profile?.LastNameEn},
                {"FullNameE", profile?.FullNameEn},
                {"PersonNo", null},
                {"UAEEnterDate", null},
                {"AccompanyNo", null},
                {"SexId", profile?.GenderId.ToString()},
                {"PreviousNationalityId", profile?.PreviosNationalityId.ToString()},
                {"CurrentNationalityId", profile?.NationalityId.ToString()},
                {"MaritalStatusId", profile?.SocialStatusId?.ToString()},
                {"FatherNameA", profile?.FatherNameAr},
                {"MotherNameA", profile?.MotherNameAr},
                {"HusbandNameA", null},
                {"FatherNameE", profile?.FatherNameEn},
                {"MotherNameE", profile?.MotherNameEn},
                {"HusbandNameE", null},
                {"ProfessionId", profile?.ProfessionId.ToString()},
                {"ReligionId", profile?.ReligionId.ToString()},
                {"EducationId", profile?.EducationId.ToString()},
                {"BirthDate", profile?.BirthDate?.ToString("MM/dd/yyyy")},
                {"BirthCountryId", profile?.BirthCountryId.ToString()},
                {"BirthPlaceE", profile?.BirthPlaceEn},
                {"BirthPlaceA", profile?.BirthPlaceAr},
                {"PassportNo", passport?.PassportNumber},
                {"PassportTypeId", passport?.PassportTypeId.ToString()},
                {"PassportIssueGovId", profile?.NationalityId.ToString()},
                {"PassportIssueCountryId", passport?.IssueCountryId.ToString()},
                {"PassportPlaceE", passport?.IssuePlaceEn},
                {"PassportPlaceA", passport?.IssuePlaceAr},
                {
                    "PassportIssueDate",
                    passport?.IssueDate == null ? null : passport.IssueDate.Value.ToString("MM/dd/yyyy")
                },
                {
                    "PassportExpiryDate",
                    passport?.ExpiryDate == null ? null : passport.ExpiryDate.Value.ToString("MM/dd/yyyy")
                },
                {"PassportRenewExpiryDate", null},
                {"IsPassportRenewed", null},
                {"Building", address?.Building},
                {"Flat", null},
                {"Street", address?.Street},
                {"EmirateId", address?.EmirateId?.ToString()},
                {"CityId", address?.CityId?.ToString()},
                {"AreaId", address?.AreaId?.ToString()},
                {"ResidenceTelNo", null},
                {"POBox", address?.POBOX},
                {"POBoxEmirate", address?.POBOXEmirateId},
                {"AddressOutside1", address?.Address1},
                {"AddressOutside2", address?.Address2},
                {"AddressOutsideCity", null},
                {"AddressOutsideCountryId", null},
                {"AddressOutsideTelNo", null},
                {"VehicleNo", null},
                {"VehicleCountryId", null},
                {"MobileNo", profile?.MobileNumber}
            };


            // Languages up to 3
            for (int i = 0; i < visionData?.IndividualUserInfo.IndividualLanguageInformation.Count && i < 3; i++)
            {
                details.Add("Language_" + (i + 1) + "_Id", visionData?.IndividualUserInfo.IndividualLanguageInformation[i].LanguageId);
            }

            PPSID.Set(profile?.PPSID);
            return Task.FromResult(details);
        }

        protected virtual  Task<JObject> BuildSponsorDetails(RestIndividualUserFormInfo visionData)
        {
            RestIndividualProfileInfo sponsorProfile;

            if (visionData?.IndividualSponsorInfo == null)
            {
                LOG.Debug("Sponsor info is null");
                sponsorProfile = new RestIndividualProfileInfo();
            }
            else
            {
                LOG.Debug("Sponsor info is not null");
                sponsorProfile = visionData?.IndividualSponsorInfo?.ProfileInfo;
            }

            LOG.Debug($"Sponsor number is {SponsorNo.Get()}");
            JObject details = new JObject
            {
                {"ApplicationId", null},
                {"NewSponsorFileNo", null},
                {"MobileNo", null},
                {"WorkTelephoneNo", null},
                {"POBox", null},
                {"Salary", null},
                {"ChannelId", null},
                {"SponsorNo", SponsorNo.Get()},
                {"SponsorFullNameA", sponsorProfile?.FullNameAr},
                {"SponsorFullNameE", sponsorProfile?.FullNameEn},
                {"SponsorEmail", null},
                {"SponsorRelationId", null},
                {"IsSponsorFileOpen", null},
                {"SponsorTypeId", SponsorType.Get()}
            };
            
            LOG.Debug("Nodes successfully added");

            return Task.FromResult(details);
        }

        protected virtual  Task<JObject> BuildReportDetails()
        {
            JObject details = new JObject
            {
                {"ApplicationId", null},
                {"Email", null},
                {"ReportData", null},
                {"FileType", null}
            };

            return Task.FromResult(details);
        }

    }
}