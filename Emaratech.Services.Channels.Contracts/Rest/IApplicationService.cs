using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest
{
    [ServiceContract]
    public interface IApplicationService
    {
        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Get applicable services", "Get applicable services", "GetServices")]
        [WebGet(UriTemplate = "/services/allowed", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestService>> GetServices();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Get all services", "Get all services", "GetAllServices")]
        [WebGet(UriTemplate = "/services/all", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestService>> GetAllServices();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Get Service Info", "Get Service Info", "GetServiceInfo")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/{serviceId}/select", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestServiceInfoResponse> GetServiceInfo(string serviceId, RestServiceInfoRequest selection);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Approve disclaimer", "Approve disclaimer", "ApproveDisclaimer")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/disclaimer", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestServiceInfoResponse> ApproveDisclaimer(RestApproveDisclaimerRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Select applicant", "Select applicant", "SelectApplicant")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/dependants/submit", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestSubmitResponse> SelectApplicant(RestSelectApplicantRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Submit Application Data", "Submit Application Data", "SubmitApplication")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/applications/submit", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestSubmitResponse> SubmitApplication(RestSubmitApplicationRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Resubmit Application Document", "Resubmit Application Document", "ResubmitApplicationDocument")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/applications/resubmit/documents", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task ResubmitApplicationDocument(RestResubmitDocumentRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Upload Documents", "Upload Documents", "UploadDocument")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/documents/upload", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestUploadDocumentResponse> UploadDocument(RestUploadDocumentRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Delete Document", "Delete document with specified document id", "DeleteDocument")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/documents/delete",
            BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task DeleteDocument(RestDeleteDocumentRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Submit Documents", "Submit Documents", "SubmitDocuments")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/documents/submit", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestSubmitResponse> SubmitDocuments(RestSubmitDocumentRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Submit action", "Submit action", "SubmitAction")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/action/submit", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestSubmitResponse> SubmitAction(RestSubmitActionRequest request);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Pay Application", "Pay Application response from noqodi", "PayApplication")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/pay/noqodi", ResponseFormat = WebMessageFormat.Json)]
        Task<RestPaymentResponse> PayApplication(RestPayApplication response);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch Required Documents", "Fetch Required Documents by Application Number", "FetchRequiredDocuments")]
        [WebGet(UriTemplate = "/services/documents/required/{applicationNumber}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestDocumentsRequiredResponseWrapper> FetchRequiredDocuments(string applicationNumber);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch Application Statuses", "Fetch All Application Statuses", "FetchApplicationStatus")]
        [WebGet(UriTemplate = "/services/application/status/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestApplicationStatusWrapper> FetchApplicationStatus();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch Image Stream", "Fetch Image Stream", "GetImageStream")]
        [WebGet(UriTemplate = "/services/document/{documentId}", RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Task<Stream> GetImageStream(string documentId);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Delete Application", "Delete application with specified application id", "DeleteApplication")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/application/delete/{applicationId}",
            BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task DeleteApplication(string applicationId);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Update Application Incomplete payment status", "Update Application Incomplete payment status with specified application id", "UpdateIncompletePaymentStatus")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/application/status/{workflowToken}/incompletePayment", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task UpdateIncompletePaymentStatus(string workflowToken);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Get entry permit new visa type lookup", "Get the visa type lookup for entry permit new service", "GetEntryPermitVisaTypeLookup")]
        [WebGet(UriTemplate = "/services/lookups/visatype",
            BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestLookup>> GetEntryPermitVisaTypeLookup();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Get transfer residence to new passport visa type lookup", "Get the visa type lookup for transfer residence to passport service", "GetResidenceTransferPassportVisaTypeLookup")]
        [WebGet(UriTemplate = "/services/lookups/transferpassportvisatype",
            BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestLookup>> GetResidenceTransferPassportVisaTypeLookup();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Get establishment list by PPSID", "Get establishment list by PPSID", "GetEstablishmentList")]
        [WebGet(UriTemplate = "/services/lookups/estabList",
          BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestLookup>> GetEstablishmentList();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch Refundable Applications", "Fetch applications eligible for refund", "FetchRefundableApplications")]
        [WebGet(UriTemplate = "/services/applications/refund/{refundType}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplications(string refundType);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch Application Detail for refund", "Fetch application detail for refund", "FetchRefundableApplicationDetail")]
        [WebGet(UriTemplate = "/services/applications/{applicationId}/refund/{refundType}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<RestRefundableApplication> FetchRefundableApplicationDetail(string applicationId, string refundType);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Process Application Refund", "Process application for refund", "ProcessApplicationRefund")]
        [WebInvoke(Method = "POST", UriTemplate = "/services/applications/refund/{refundType}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task ProcessApplicationRefund(string refundType, RestRefundRequest refundRequest);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch all refundable applications", "Fetch all refundable applications list", "FetchRefundableApplicationsList")]
        [WebGet(UriTemplate = "/services/applications/refund/", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplicationsList();

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch refunded applications", "Fetch applications on which refund operation is performed and are not in refund eligibility list anymore", "FetchRefundedApplications")]
        [WebGet(UriTemplate = "/services/applications/refunded/{applicationId=null}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestRefundableApplication>> FetchRefundedApplications(string applicationId);

        [SwaggerWcfTag("Channel")]
        [SwaggerWcfTag("Services")]
        [SwaggerWcfPath("Fetch all incomplete payment applications", "Fetch all incomplete payment applications list", "GetIncompletePaymentApplications")]
        [WebGet(UriTemplate = "/services/applications/incompletepayment/", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Task<IEnumerable<RestDependentApplicationInfo>> GetIncompletePaymentApplications();
        //[SwaggerWcfTag("Channel")]
        //[SwaggerWcfTag("Services")]
        //[SwaggerWcfPath("RSA Authentication", " RSA Authentication", "Authenticate")]
        //[WebInvoke(Method = "POST", UriTemplate = "/RSAAuthenticate/", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[OperationContract]
        //Task<RSAAuthenticationResponse> Authenticate(RSAAuthenticationData AuthenticationRequestData);
    }
}