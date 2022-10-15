using Emaratech.Services.Channels.Contracts.Rest.Models.HappinessMeter;
using Emaratech.Services.WcfCommons.Faults.Models;
using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IHappinessMeterService
    {
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("HappinessMeter")]
        [SwaggerWcfPath("Get Happiness Meter Post Parameters", "Get Happiness Meter Post Parameters", "GetHappinessMeter")]
        [WebInvoke(Method = "GET", UriTemplate = "/happinessMeter/lang/{lang}/themeColor/{themeColor}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        System.IO.Stream GetHappinessMeter(string lang, string themeColor);
    }
}
