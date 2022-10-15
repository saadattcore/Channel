using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IReportService
    {
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Get history of all reports", "Get history of all reports", "GetReportsHistory")]
        [WebGet(UriTemplate = "/reports/all", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestReportsHistory>> GetReportsHistory();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Get reports allowed to be applied", "Get reports allowed to be applied", "GetAllowedReports")]
        [WebGet(UriTemplate = "/reports/allowed/{systemId}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestReportsAllowed>> GetAllowedReports(string systemId);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Get report by application id", "Get report by application id", "GetReport")]
        [WebGet(UriTemplate = "/reports/{applicationId}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<Stream> GetReport(string applicationId);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Get report by token", "Get report by token", "GetReportWithToken")]
        [WebGet(UriTemplate = "/reports?token={token}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<Stream> GetReportWithToken(string token);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Send Report", "Send report to specified email address", "SendReport")]
        [WebInvoke(Method = "POST", UriTemplate = "/reports/send/{applicationId}?email={email}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task SendReport(string applicationId, string email);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Select Query", "Execute select query", "Reporting")]
        [WebInvoke(Method = "POST", UriTemplate = "/reporting", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestReportingResponse> Reporting(RestReportingRequest req);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Insert, Delete and Update Query", "Execute insert, delete, update query", "ReportingUpdate")]
        [WebInvoke(Method = "PUT", UriTemplate = "/reporting", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<int?> ReportingUpdate(RestReportingRequest req);

        /// <summary>
        /// Get PDF As Base64string report for list of ids
        /// </summary>
        /// <returns>The application report result which report content as base64string <see cref="RestReportResponse"/></returns>
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("ReportServices")]
        [SwaggerWcfPath("Allow to generate different reports like evisa, cancellation based on requested type", "Allow to generate different reports for applications or files like evisa, cancellation based on requested type", "GenerateReport")]
        [WebInvoke(Method = "POST", UriTemplate = "/reports/generate", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestReportResponse> GenerateReport(RestReportsRequest reportRequest);
    }
}
