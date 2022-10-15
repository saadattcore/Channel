using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Reports.Models.Sns;
using log4net;
using Emaratech.Services.Channels.Helpers.Lookups;

namespace Emaratech.Services.Channels.Workflows.Steps.Report.Processing
{
    public class BuildEstablishmentInfoReportStep : ProcessingStep
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BuildEstablishmentInfoReportStep));

        private string EstablishmentCode { set; get; }

        private Action<SponsorInfo> StoreAction { get; }

        public BuildEstablishmentInfoReportStep(string establishmentCode, Action<SponsorInfo> argStore, ProcessingStep next) : base(next)
        {
            EstablishmentCode = establishmentCode;
            StoreAction = argStore;
        }

        protected override async Task Execute()
        {
            string address = string.Empty;
            Log.Debug($"Going to get establishment information for code {EstablishmentCode}");
            var establishmentInfo = await ServicesHelper.GetEstablishmentProfile(EstablishmentCode);

            var estabNameEn = establishmentInfo.EstabNameEn;
            var estabNameAr = establishmentInfo.EstabNameAr;

            Log.Debug($"Going to get establishment dependents information {EstablishmentCode}");
            var establishmentDependentVisaDetails = await ServicesHelper.GetEstablishmentDetailedStatsReport(EstablishmentCode);
            Log.Debug($"Establishment dependents information received with {establishmentDependentVisaDetails.PermitDependents.Count} permits and residence dependents {establishmentDependentVisaDetails.ResidenceDependents.Count}");

            int? totalVisaCount = establishmentDependentVisaDetails.PermitDependents.Count;
            int? totalVisaActive = establishmentDependentVisaDetails.PermitActive.HasValue ? establishmentDependentVisaDetails.PermitActive.Value : 0;
            int? totalVisaAbscond = establishmentDependentVisaDetails.PermitAbsconding.HasValue ? establishmentDependentVisaDetails.PermitAbsconding.Value : 0;
            int? totalVisaClosed = establishmentDependentVisaDetails.PermitClosed.HasValue ? establishmentDependentVisaDetails.PermitClosed.Value : 0;

            int? totalResidentCount = establishmentDependentVisaDetails.ResidenceDependents.Count;
            int? totalResidentActive = establishmentDependentVisaDetails.ResidenceActive.HasValue ? establishmentDependentVisaDetails.ResidenceActive.Value : 0;
            int? totalResidentAbscond = establishmentDependentVisaDetails.ResidenceAbsconding.HasValue ? establishmentDependentVisaDetails.ResidenceAbsconding.Value : 0;
            int? totalResidentClosed = establishmentDependentVisaDetails.ResidenceClosed.HasValue ? establishmentDependentVisaDetails.ResidenceClosed.Value : 0;
            bool isBanned = establishmentInfo?.EstabStatusId == 3;
            

            if (establishmentInfo != null)
            {
                StringBuilder addressBuilder = new StringBuilder();
                addressBuilder.Append(string.Concat(establishmentInfo.FlatNo, " "));

                if (string.IsNullOrEmpty(establishmentInfo.AddressAr))
                    addressBuilder.Append(string.Concat(establishmentInfo.Building, " "));
                else
                    addressBuilder.Append(string.Concat(establishmentInfo.AddressAr, " "));

                addressBuilder.Append(string.Concat(establishmentInfo.Street, " "));

                addressBuilder.Append(string.Concat(LookupArea.Instance.GetAr(Convert.ToString(establishmentInfo.AreaId)), " "));
                address = addressBuilder.ToString();
            }

            StoreAction(new SponsorInfo(estabNameEn, estabNameAr, EstablishmentCode, address, isBanned,
                                        totalVisaCount, totalVisaActive, totalVisaAbscond, totalVisaClosed,
                                        totalResidentCount, totalResidentActive, totalResidentAbscond, totalResidentClosed, string.Empty, string.Empty, string.Empty, establishmentInfo?.RegIssueDate.Value.ToString("dd-MM-yyyy"),string.Empty));
        }
    }
}