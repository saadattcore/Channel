using System;
using System.Collections.Generic;

namespace Emaratech.Services.Channels.Reports.Models.Sns
{
    public class Root : MarshalByRefObject
    {
        public SponsorInfo SponsorInfo { get; set; }
        public List<ResidenceDetails> resident { get; set; }
        public List<EntryPermitDetails> visa { get; set; }

        public Root(SponsorInfo argSponsorInfo, List<ResidenceDetails> argResidenceDetailsList, List<EntryPermitDetails> argEntryPermitDetails)
        {
            SponsorInfo = argSponsorInfo;
            resident = argResidenceDetailsList;
            visa = argEntryPermitDetails;
        }

    }
}