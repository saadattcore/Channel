using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.eVisa.Reports
{
    internal class ReportEntity
    {
        public string path { get; set; }
        public string data { get; set; }
        public Dictionary<string, string> parameters { get; set; }
    }
}
