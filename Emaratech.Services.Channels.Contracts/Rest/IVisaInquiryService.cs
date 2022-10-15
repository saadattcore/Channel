using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.WcfCommons.Faults;
using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models.VisaInquiry;
using Emaratech.Services.WcfCommons.Faults.Models;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IVisaInquiryService
    {
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("VisaInquiry")]
        [SwaggerWcfPath("Verify Visa Validity", "Take certain parameters and return the visa validity information", "FetchVisaInformation")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/visainquiry/", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(ErrorModel))]
        Task<RestVisaInquiryResult> FetchVisaInquiryInformation(RestVisaInquiryCriteria request);
    }
}
