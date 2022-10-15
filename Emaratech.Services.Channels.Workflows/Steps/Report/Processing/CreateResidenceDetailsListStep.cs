using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using log4net;
using System.Configuration;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class CreateResidenceDetailsListStep : ProcessingStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreateResidenceDetailsListStep));
        private Func<IEnumerable<RestIndividualDependentsInfoWrapper>> GetDependentsFunc { get; }
        private Action<List<ResidenceDetails>> StoreAction { get; }
        private ILookup genderLookup => LookupGender.Instance;
        private ILookup nationalityLookup => LookupNationality.Instance;
        private ILookup visaTypeLookup => LookupVisaType.Instance;
        private ILookup residenceStatusLookup => LookupResidenceStatus.Instance;
        private ILookup professionLookup => LookupProfession.Instance;

        public CreateResidenceDetailsListStep(Func<IEnumerable<RestIndividualDependentsInfoWrapper>> argGetDependentsFunc,
            Action<List<ResidenceDetails>> store,
            ProcessingStep argNextStep) : base(argNextStep)
        {
            GetDependentsFunc = argGetDependentsFunc;
            StoreAction = store;
        }

        protected override async Task Execute()
        {
            var residenceDetailsList = new List<ResidenceDetails>();
            uint residencePermitSerialNo = 1;
            var residenceNoList = new List<string>();

            foreach (var dependant in GetDependentsFunc()??new List<RestIndividualDependentsInfoWrapper>())
            {
                if (string.Equals(dependant.IndividualVisaInformation.VisaType, "R"))
                {
                    var visaNumber = dependant.IndividualVisaInformation.VisaNumber;
                    residenceNoList.Add(visaNumber);
                }
            }

            Log.Debug($"Going to get dependents detailed residence information from vision with dependents count {residenceNoList.Count}");
            var dependentList = await ServicesHelper.GetIndividualProfileListByResNoForReport(residenceNoList);
            Log.Debug($"Resident dependents detailed information received from vision");

            var travelDetailInfoList = await ServicesHelper.GetIndividualCurrentTravelStatusListByResNo(residenceNoList);

            Log.Debug($"Travel Info received of residence dependents");
            var limit = Convert.ToInt32(ConfigurationManager.AppSettings["SasResLimit"] ?? Int32.MaxValue.ToString());

            //foreach (var dependent in dependentList)
            if (dependentList != null && dependentList.Count > 0)
            {
                for (int i = 0; i < dependentList.Count && i <= limit; i++)
                {
                    DateTime? lastTravelDate = travelDetailInfoList?.Find(p => p.ResidenceNo == dependentList[i].IndividualUserInfo?.IndividualResidenceInfo.ResidenceNo)?.TravelDate;
                    string lastTravelTypeId = travelDetailInfoList?.Find(p => p.ResidenceNo == dependentList[i].IndividualUserInfo?.IndividualResidenceInfo.ResidenceNo)?.TravelTypeId;
                    var details = CreateDetailsCommon(dependentList[i], Convert.ToString(residencePermitSerialNo++), lastTravelDate, lastTravelTypeId, CreateResidenceDetails);
                    residenceDetailsList.Add(details);
                }
            }
            else
            {
                Log.Debug("Dependent list is null for resident dependent");
            }

            Log.Debug("Build resident dependents step finished");
            StoreAction(residenceDetailsList);
        }

        private T CreateDetailsCommon<T>(RestIndividualUserFormInfo dependant, string serialNo, DateTime? lastTravelDate, string lastTravelTypeId, Func<string, string, string, string, string, string, RestIndividualUserFormInfo, string, DateTime?, string, T> createConcrete)
        {
            
            var name = dependant.IndividualUserInfo.IndividualProfileInformation.FullNameAr;
            var udb = dependant?.IndividualUserInfo?.IndividualProfileInformation?.UDBNo;
            var gender = genderLookup.GetAr(dependant?.IndividualUserInfo?.IndividualProfileInformation?.GenderId.ToString());
            var passportNo = dependant?.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            var nationality = nationalityLookup.GetAr(dependant?.IndividualUserInfo?.IndividualProfileInformation?.NationalityId.ToString());
            var profession = professionLookup.GetAr(
                    dependant?.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            return createConcrete(name, udb, gender, passportNo, nationality, profession, dependant, serialNo, lastTravelDate, lastTravelTypeId);
        }

        private ResidenceDetails CreateResidenceDetails(string name, string udb, string gender, string passportNo, string nationality, string profession, RestIndividualUserFormInfo dependant, string serialNo, DateTime? lastTravelDate, string lastTravelTypeId)
        {
            var residencePermitNo = dependant?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceNo;
            var visaType = visaTypeLookup.GetAr(dependant?.IndividualUserInfo?.IndividualResidenceInfo?.VisaTypeId.ToString());
            var issuingDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceIssueDate?.ToString("dd/MM/yyyy"));
            var expiryDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceExpiryDate?.ToString("dd/MM/yyyy"));
            var status = residenceStatusLookup.GetAr(dependant?.IndividualUserInfo?.IndividualResidenceInfo?.ResidenceStatusId?.ToString());
            serialNo = DateUtility.ConvertToArabicNumbers(serialNo);

            var cancelDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualResidenceInfo?.CancelDate?.ToString("dd/MM/yyyy"));
            var closeDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualResidenceInfo?.CloseDate?.ToString("dd/MM/yyyy"));
            var travelDate = DateUtility.ConvertToArabicNumbers(lastTravelDate?.ToString("dd/MM/yyyy"));

            var lasttravelType = string.Empty;
            if (lastTravelTypeId == "1")
            {
                lasttravelType = "حركة دخول";
            }
            else if (lastTravelTypeId == "2")
            {
                lasttravelType = "حركة خروج";
            }

            return new ResidenceDetails(serialNo, residencePermitNo, visaType, name, udb,
                gender, passportNo, nationality, profession, issuingDate, expiryDate, status, cancelDate, closeDate, travelDate, lasttravelType);
        }
    }
}