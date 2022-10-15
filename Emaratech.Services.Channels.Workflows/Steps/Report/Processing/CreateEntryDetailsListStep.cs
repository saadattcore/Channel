using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using log4net;
using System.Configuration;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class CreateEntryDetailsListStep : ProcessingStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreateEntryDetailsListStep));

        public Func<IEnumerable<RestIndividualDependentsInfoWrapper>> GetDependentsFunc { get; }
        private Action<List<EntryPermitDetails>> StoreAction { get; }
        private readonly ILookup genderLookup = LookupGender.Instance;
        private readonly ILookup nationalityLookup = LookupNationality.Instance;
        private readonly ILookup visaTypeLookup = LookupVisaType.Instance;
        private readonly ILookup permiteStatusLookup = LookupPermitStatus.Instance;
        private readonly ILookup professionLookup = LookupProfession.Instance;

        public CreateEntryDetailsListStep(Func<IEnumerable<RestIndividualDependentsInfoWrapper>> argGetDependentsFunc,
            Action<List<EntryPermitDetails>> store,
            ProcessingStep argNextStep) : base(argNextStep)
        {
            GetDependentsFunc = argGetDependentsFunc;
            StoreAction = store;
        }

        protected override async Task Execute()
        {
            uint entryPermitSerialNo = 1;
            var entryPermitDetailsList = new List<EntryPermitDetails>();

            var permitNoList = new List<string>();

            foreach (var dependant in GetDependentsFunc()??new List<RestIndividualDependentsInfoWrapper>())
            {
                if (string.Equals(dependant.IndividualVisaInformation.VisaType, "P"))
                {
                    var visaNumber = dependant.IndividualVisaInformation.VisaNumber;
                    permitNoList.Add(visaNumber);
                }
            }

            Log.Debug($"Going to get dependents detailed permit information from vision with dependents count {permitNoList.Count}");
            var dependentList = await ServicesHelper.GetIndividualProfileListByPermitNoForReport(permitNoList);
            Log.Debug($"Permit dependents detailed information received from vision");

            var travelDetailInfoList = await ServicesHelper.GetIndividualCurrentTravelStatusListByPermitNo(permitNoList);
            Log.Debug($"Travel Info received of permit dependents");

            var limit = Convert.ToInt32(ConfigurationManager.AppSettings["SasEpLimit"] ?? Int32.MaxValue.ToString());

            //foreach (var dependent in dependentList)
            if (dependentList != null && dependentList.Count > 0)
            {
                for (int i = 0; i < dependentList?.Count && i <= limit; i++)
                {
                    DateTime? travelDate = travelDetailInfoList?.Find(p => p.PermitNo == dependentList[i].IndividualUserInfo?.IndividualPermitInfo.PermitNo)?.TravelDate;
                    string lastTravelTypeId = travelDetailInfoList?.Find(p => p.PermitNo == dependentList[i].IndividualUserInfo?.IndividualPermitInfo.PermitNo)?.TravelTypeId;
                    var details = CreateDetailsCommon(dependentList[i], Convert.ToString(entryPermitSerialNo++), travelDate, lastTravelTypeId, CreateEntryPermitDetails);
                    entryPermitDetailsList.Add(details);
                }
            }
            else
            {
                Log.Debug("Dependent list is null for permit dependent");
            }

            Log.Debug("Build permit dependents step finished");
            StoreAction(entryPermitDetailsList);
        }

        private T CreateDetailsCommon<T>(RestIndividualUserFormInfo dependant, string serialNo, DateTime? lastTravelDate, string lastTravelTypeId, Func<string, string, string, string, string, string, RestIndividualUserFormInfo, string, DateTime?, string, T> createConcrete)
        {
            var name = dependant.IndividualUserInfo.IndividualProfileInformation?.FullNameAr;
            var udb = dependant?.IndividualUserInfo?.IndividualProfileInformation?.UDBNo;
            var gender = genderLookup.GetAr(dependant?.IndividualUserInfo?.IndividualProfileInformation?.GenderId.ToString());
            var passportNo = dependant.IndividualUserInfo?.IndividualPassportInformation?.PassportNumber;
            var nationality = nationalityLookup.GetAr(dependant?.IndividualUserInfo?.IndividualProfileInformation?.NationalityId.ToString());
            var profession = professionLookup.GetAr(
                    dependant?.IndividualUserInfo?.IndividualProfileInformation?.ProfessionId.ToString());

            return createConcrete(name, udb, gender, passportNo, nationality, profession, dependant, serialNo, lastTravelDate, lastTravelTypeId);
        }

        private EntryPermitDetails CreateEntryPermitDetails(string name, string udb, string gender, string passportNo, string nationality, string profession, RestIndividualUserFormInfo dependant, string serialNo, DateTime? lastTravelDate, string lastTravelTypeId)
        {
            var entryPermitNo = dependant?.IndividualUserInfo?.IndividualPermitInfo?.PermitNo;

            var visaType = visaTypeLookup.GetAr(dependant?.IndividualUserInfo?.IndividualPermitInfo?.VisaTypeId.ToString());
            var issuingDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualPermitInfo?.PermitIssueDate?.ToString("dd/MM/yyyy"));
            var expiryDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualPermitInfo?.PermitStatusId == (int)PermitStatusType.Issued ? dependant?.IndividualUserInfo?.IndividualPermitInfo?.PermitExpiryDate?.ToString("dd/MM/yyyy") : dependant?.IndividualUserInfo?.IndividualPermitInfo?.ValidityDate?.ToString("dd/MM/yyyy"));
            var status = permiteStatusLookup.GetAr(dependant?.IndividualUserInfo?.IndividualPermitInfo?.PermitStatusId?.ToString());
            serialNo = DateUtility.ConvertToArabicNumbers(serialNo);
            var cancelDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualPermitInfo?.CancelDate?.ToString("dd/MM/yyyy"));
            var closeDate = DateUtility.ConvertToArabicNumbers(dependant?.IndividualUserInfo?.IndividualPermitInfo?.CloseDate?.ToString("dd/MM/yyyy"));
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

            return new EntryPermitDetails(serialNo, entryPermitNo, visaType, name, udb,
                gender, passportNo, nationality, profession, issuingDate, expiryDate, status, cancelDate, closeDate, travelDate, lasttravelType);
        }
    }
}