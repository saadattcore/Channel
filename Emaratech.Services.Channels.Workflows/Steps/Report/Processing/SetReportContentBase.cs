using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public abstract class SetReportContentBase : ProcessingStep
    {
        private Func<SponsorInfo> GetSponsorInfoFunc { get; }
        private Func<IEnumerable<ResidenceDetails>> GetResidenceDetailsFunc { get; }
        private Func<IEnumerable<EntryPermitDetails>> GetEntryPermitDetailsFunc { get; }
        protected Action<string> SetReportOutput { get; }

        protected SetReportContentBase(
            Func<SponsorInfo> argGetSponsorInfo,
            Func<IEnumerable<ResidenceDetails>> argGetResidenceDetailsFunc,
            Func<IEnumerable<EntryPermitDetails>> argGetEntryPermitDetailsFunc,
            Action<string> argSetReportOutput,
            ProcessingStep argNextStep) : base(argNextStep)
        {
            GetSponsorInfoFunc = argGetSponsorInfo;
            GetResidenceDetailsFunc = argGetResidenceDetailsFunc;
            GetEntryPermitDetailsFunc = argGetEntryPermitDetailsFunc;
            SetReportOutput = argSetReportOutput;
        }

        protected override async Task Execute()
        {
            var rootObj = new Root(GetSponsorInfoFunc(), GetResidenceDetailsFunc().ToList(), GetEntryPermitDetailsFunc().ToList());

            JObject obj = new JObject();

            obj["name"] = rootObj.SponsorInfo.NameAr;
            obj["fileno"] = rootObj.SponsorInfo.FileNo;
            obj["address"] = rootObj.SponsorInfo.Address;
            obj["isBannedEstablishment"] = rootObj.SponsorInfo.isBannedEstablishment;
            obj["resident"] = JArray.FromObject(BuildResidentInfo(rootObj));
            obj["visa"] = JArray.FromObject(BuildVisaInfo(rootObj));
            obj["profession"] = rootObj.SponsorInfo.Occupation;
            obj["nationality"] = rootObj.SponsorInfo.Nationality;
            obj["matrialStatus"] = rootObj.SponsorInfo.MaritalStatus;

            string zeorInArabic = DateUtility.ConvertToArabicNumbers("0");
            
            obj["totResidentActive"] = rootObj.SponsorInfo.TotalResidentActive.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalResidentActive.Value.ToString()) : zeorInArabic;
            obj["totResidentAbscond"] = rootObj.SponsorInfo.TotalResidentAbscond.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalResidentAbscond.Value.ToString()) : zeorInArabic;
            obj["totResidentClosed"] = rootObj.SponsorInfo.TotalResidentClosed.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalResidentClosed.Value.ToString()) : zeorInArabic;
            obj["totResident"] = rootObj.SponsorInfo.TotalResidentCount.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalResidentCount.Value.ToString()) : zeorInArabic;



            obj["totVisaActive"] = rootObj.SponsorInfo.TotalVisaActive.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalVisaActive.Value.ToString()) : zeorInArabic;
            obj["totVisaAbscond"] = rootObj.SponsorInfo.TotalVisaAbscond.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalVisaAbscond.Value.ToString()) : zeorInArabic;
            obj["totVisaClosed"] = rootObj.SponsorInfo.TotalVisaClosed.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalVisaClosed.Value.ToString()) : zeorInArabic;
            obj["totVisa"] = rootObj.SponsorInfo.TotalVisaCount.HasValue ? DateUtility.ConvertToArabicNumbers(rootObj.SponsorInfo.TotalVisaCount.Value.ToString()) : zeorInArabic;


            obj["dateheader"] = "From: " + rootObj?.SponsorInfo?.RegistrationDate + " To: " + DateTime.Now.ToString("dd-MM-yyyy");

            obj["dbarano"] = rootObj?.SponsorInfo?.PersonNo;

            SetReportOutput(SerializeReport(obj));

            await Task.Run(() => { });
        }

        private List<JObject> BuildResidentInfo(Root root)
        {
            List<JObject> listTResidentInfo = new List<JObject>();
            foreach (var resObj in root.resident)
            {
                var resInfo = new JObject();
                resInfo["visaNo"] = resObj.visaNo;
                resInfo["sno"] = resObj.sno;
                resInfo["visaType"] = resObj.visaType;
                resInfo["name"] = resObj.name;
                resInfo["udbNo"] = resObj.udbNo;
                resInfo["gender"] = resObj.gender;
                resInfo["passNo"] = resObj.passNo;

                resInfo["nationality"] = resObj.nationality;
                resInfo["profession"] = resObj.profession;

                resInfo["issueDate"] = resObj.issueDate;
                resInfo["expDate"] = resObj.expDate;
                resInfo["status"] = resObj.status;
                resInfo["closedDate"] = resObj.closeDate;
                resInfo["cancelDate"] = resObj.cancelDate;
                resInfo["lastTravelDate"] = resObj.travelDate;
                resInfo["lastTravelType"] = resObj.travelType;
                listTResidentInfo.Add(resInfo);
            }

            return listTResidentInfo;

        }


        private List<JObject> BuildVisaInfo(Root root)
        {
            List<JObject> listTVisaInfo = new List<JObject>();
            foreach (var visaObj in root.visa)
            {
                var visaInfo = new JObject();
                visaInfo["entrypermitNo"] = visaObj.entrypermitNo;
                visaInfo["sno"] = visaObj.sno;
                visaInfo["visaType"] = visaObj.visaType;
                visaInfo["name"] = visaObj.name;
                visaInfo["udbNo"] = visaObj.udbNo;
                visaInfo["gender"] = visaObj.gender;
                visaInfo["passNo"] = visaObj.passNo;

                visaInfo["nationality"] = visaObj.nationality;
                visaInfo["profession"] = visaObj.profession;

                visaInfo["issueDate"] = visaObj.issueDate;
                visaInfo["expDate"] = visaObj.expDate;
                visaInfo["status"] = visaObj.status;
                visaInfo["closedDate"] = visaObj.closeDate;
                visaInfo["cancelDate"] = visaObj.cancelDate;
                visaInfo["lastTravelDate"] = visaObj.travelDate;
                visaInfo["lastTravelType"] = visaObj.travelType;
               

                listTVisaInfo.Add(visaInfo);
            }

            return listTVisaInfo;

        }

        protected abstract string SerializeReport(object data);
    }
}