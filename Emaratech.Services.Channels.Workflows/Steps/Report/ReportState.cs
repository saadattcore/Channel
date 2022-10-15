using System;
using System.Collections.Generic;
using Emaratech.Services.Channels.Reports.Models.Sns;
using Emaratech.Services.Vision.Model;
using ResidenceDetails = Emaratech.Services.Channels.Reports.Models.Sns.ResidenceDetails;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Workflows.Steps.Report
{
    public class ReportState
    {
        public SponsorInfo SponsorInfo { get; private set; }
        public IEnumerable<RestIndividualDependentsInfoWrapper> DependentsInfo { get; private set; }
        public List<ResidenceDetails> DependentsResidenceDetails { get; private set; }
        public List<EntryPermitDetails> DependentsEntryPermitDetails { get; private set; }
        public string JsonReport { get;  set; }
        public string HtmlReport { get; private set; }
        public string EmailName => "Dejan Novakovic";
        public string EmailAddress => "dejan.novakovic@emaratech.ae";

        public string SponsorInfoJsonData  { get;set; }

        public string ResiDependentInfoJsonData { get; set; }

        public string PermitDependentInfoJsonData { get; set; }

        public void SetSponsorInfo(SponsorInfo argSponsorInfo)
        {
            SponsorInfo = argSponsorInfo;
            SponsorInfoJsonData = JsonConvert.SerializeObject(argSponsorInfo);
        }

        public SponsorInfo GetSponsorInfo()
        {
            return SponsorInfo;
        }

        public void SetDependentsInfo(IEnumerable<RestIndividualDependentsInfoWrapper> argDependentsInfo)
        {
            DependentsInfo = argDependentsInfo;
        }


        public IEnumerable<RestIndividualDependentsInfoWrapper> GetDependentsInfo()
        {
            return DependentsInfo;
        }

        public void SetDependentsResidenceDetails(List<ResidenceDetails> argDependentsResidenceDetails)
        {
            DependentsResidenceDetails = argDependentsResidenceDetails;
            ResiDependentInfoJsonData = JsonConvert.SerializeObject(argDependentsResidenceDetails);
        }
        public List<ResidenceDetails> GetDependentsResidenceDetails()
        {
            return DependentsResidenceDetails;
        }
        public void SetDependentsEntryPermitDetails(List<EntryPermitDetails> argDependentEntryPermitDetails)
        {
            DependentsEntryPermitDetails = argDependentEntryPermitDetails;
            PermitDependentInfoJsonData = JsonConvert.SerializeObject(argDependentEntryPermitDetails);
        }
        public List<EntryPermitDetails> GetDependentsEntryPermitDetails()
        {
            return DependentsEntryPermitDetails;
        }
        public void SetJsonReport(string argReport)
        {
            JsonReport = argReport;
        }
        public void SetHtmlReport(string argReport)
        {
            HtmlReport = argReport;
        }

    }
}