using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Helpers.Lookups;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.WcfCommons.Faults.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;

namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports.Cancellation
{
    public abstract class CancellationReportBaseHanlder : ReportHandler
    {
        protected const string dateFormat = "dd/MM/yyyy";
        protected JsonSerializerSettings serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        #region Private Fields
        protected ILookup nationalityLookup => LookupNationality.Instance;
        protected ILookup visaTypeLookup => LookupVisaType.Instance;
        protected ILookup professionLookup => LookupProfession.Instance;
        protected ILookup passportTypeLookup => LookupPassportType.Instance;
        #endregion

        protected abstract Task<IList<RestIndividualUserFormInfo>> GetUserFormInfo(IList<string> ids);

        public override async Task<IList<ReportRecordInfo>> GetReportEntries(IList<string> residenceIds)
        {
            try
            {
                var entryPermitReprots = new List<ReportRecordInfo>();
                var permitsInfo = await GetUserFormInfo(residenceIds);

                foreach (var permit in permitsInfo)
                {

                    var reprot = GetReport(permit);

                    if (reprot != null)
                    {
                        entryPermitReprots.Add(reprot);
                    }
                }

                return entryPermitReprots;
            }
            catch (FaultException fault)
            {
                Log.Error(fault);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ErrorCodes.BadRequest.ToServiceFault(ex.Message);
            }
        }

        public ReportRecordInfo GetReport(RestIndividualUserFormInfo permitsInfo)
        {
            var report = new ReportRecordInfo();

            report.ReportData = SerializeAndGetReportData(permitsInfo);
            report.ReportPath = ReportConstants.ResdenceCancellationReport;
            report.IsRequiredDateParameter = true;
            report.HasInstructions = false;

            return report;
        }

        protected abstract string SerializeAndGetReportData(RestIndividualUserFormInfo permitsInfo);

        internal static string GetNotes(int travelTypeId, string locale, int noteType, string validatyDateEn, string travelDateEn, int? noOfDays)
        {
            string note = string.Empty;

            switch (noteType)
            {
                case 1:
                    if (travelTypeId == (int)TravelType.Entry)
                    {

                        if (locale == "en")
                        {
                            if (noOfDays == 0)
                                note = $"Note: must leave the country or change status up to: {System.DateTime.Now.ToString("dd-MM-yyyy")} . If the person does not comply with this, he will bear all legal procedures.";
                            else
                                note = $"Note: must leave the country or change status up to: {validatyDateEn} . If the person does not comply with this, he will bear all legal procedures.";
                        }
                        else if (locale == "ar")
                        {
                            if (noOfDays == 0)
                                note = $"يجب مغادرة الدولة أو تعديل الوضع بحد أقصى بتاريخ: { System.DateTime.Now.ToString("dd-MM-yyyy")} وفي حال عدم الالتزام بذلك يتحمل المذكور أعلاه كافة الإجراءات القانونية المترتبة على ذلك";
                            else
                                note = $"يجب مغادرة الدولة أو تعديل الوضع بحد أقصى بتاريخ: {validatyDateEn } وفي حال عدم الالتزام بذلك يتحمل المذكور أعلاه كافة الإجراءات القانونية المترتبة على ذلك";
                        }
                    }
                    else if (travelTypeId == (int)TravelType.Exit)
                    {
                        if (locale == "en")
                            note = $"Note: the person has left the country on:{travelDateEn}";
                        else if (locale == "ar")
                            note = $"{travelDateEn} :ملاحظة: غادر المذكور أعلاه الدولة بتاريخ ";
                    }
                    break;

                case 2:
                    if (travelTypeId == (int)TravelType.Entry)
                    {
                        if (locale == "en")
                            note = $"The sponsor complies to notify GDRFA-D if the sponsored doesn’t leave the country or change status within ({noOfDays}) day(s) from the date of cancellation, and to bear all legal actions against it.";
                        else if (locale == "ar")
                            note = $"يتعهد الكفيل بإبلاغ الإدارة في حال عدم مغادرة المكفول أو تعديل وضعه خلال ({noOfDays}) يوم من تاريخ الإلغاء، وأن يتحمل كافة الإجراءات القانونية تجاه ذلك";
                    }
                    else
                        note = string.Empty;
                    break;

                default:
                    break;
            }

            return note;
        }

        internal static string FormatPermitNo(string permitNo)
        {
            if (string.IsNullOrEmpty(permitNo))
                return string.Empty;

            return $"{permitNo.Substring(0, 3)}/{permitNo.Substring(3, 4)}/{permitNo.Substring(7)}";
        }

        internal static int CalculateNoOfDays(DateTime? validityDate, DateTime? cancellationDate)
        {
            int remainingDays = (validityDate.Value - DateTime.Now).Days;

            if (remainingDays > 30)
            {
                remainingDays = 30;
            }
            if (remainingDays < 0)
            {
                remainingDays = 0;
            }
            return remainingDays;
        }
    }
}