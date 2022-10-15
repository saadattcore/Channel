using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Channels.BusinessLogic.Dashboard;
using Emaratech.Services.Vision.Model;
using log4net;

namespace Emaratech.Services.Channels.Helpers
{
    public class VisionHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VisionHelper));
        public static async Task<IList<RestIndividualTravelInfo>> GetTravelInfoResidenceVisas(List<string> lstResidenceNos)
        {
            if (lstResidenceNos == null || lstResidenceNos.Count <= 0)
                return null;

            return await ApiFactory.Default.GetVisionIndividualApi().GetIndividualCurrentTravelStatusByResNoAsync(new RestVisionQueryCriteria { Values = lstResidenceNos });
        }
    }
}