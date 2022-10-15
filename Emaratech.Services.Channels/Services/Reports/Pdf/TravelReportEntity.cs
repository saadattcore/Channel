using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emaratech.Services.Channels.Services.Reports.Pdf
{
    internal class ReportEntity
    {
        public string path { get; set; }
        public string data { get; set; }
        public Dictionary<string, string> parameters { get; set; }
    }
}