using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Document.Model;
using Emaratech.Services.Channels.Contracts.Errors;
using Newtonsoft.Json;
using RestApplicationStatusWrapper = Emaratech.Services.Channels.Contracts.Rest.Models.Application.RestApplicationStatusWrapper;
using System.Configuration;
using System.Drawing.Imaging;
using System.IdentityModel.Claims;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.BusinessLogic.Refund;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.Channels.Extensions;
using Emaratech.Services.Localization.Model;
using Emaratech.Services.Identity.Utils;
using Emaratech.Services.Channels.Models;
using Emaratech.Services.Channels.Contracts.Extensions;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Common.Imaging;
using Emaratech.Services.Payment.Model;
using RestPaymentResponse = Emaratech.Services.Channels.Contracts.Rest.Models.Application.RestPaymentResponse;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Emaratech.Services.Channels
{
    public partial class ChannelService
    {
        public async Task<RestServiceInfoResponse> GetServiceInfo(string serviceId, RestServiceInfoRequest request)
        {
            AllowAnonymousAccess(serviceId);
            Log.Debug($"Start workflow with service id {serviceId} and request {JsonConvert.SerializeObject(request)}");
            var state = await StartWorkflow(serviceId, request);
            var response = GetResponse<RestServiceInfoResponse>(state.ToDictionary());
            return response;
        }

        public async Task<RestServiceInfoResponse> ApproveDisclaimer(RestApproveDisclaimerRequest request)
        {
            var state = await ResumeWorkflow(request.ToJObject());
            var response = GetResponse<RestServiceInfoResponse>(state.ToDictionary());
            return response;
        }

        public async Task<RestSubmitResponse> SubmitApplication(RestSubmitApplicationRequest request)
        {
            var state = await ResumeWorkflow(request.ToJObject());
            var response = GetResponse<RestSubmitResponse>(state.ToDictionary());
            return response;
        }

        public async Task<RestSubmitResponse> SelectApplicant(RestSelectApplicantRequest request)
        {
            var applicantContext = new ApplicantWorkflowContext()
            {
                PermitNumber = request.PermitNumber,
                WorkflowToken = request.WorkflowToken,
                ResidenceNumber = request.ResidenceNumber
            };
            var state = await ResumeWorkflow(applicantContext.ToJObject());
            var response = GetResponse<RestSubmitResponse>(state.ToDictionary());
            return response;
        }

        public async Task<RestUploadDocumentResponse> UploadDocument(RestUploadDocumentRequest request)
        {
            string serviceId = string.Empty;

            if (!string.IsNullOrEmpty(request.WorkflowToken))
                serviceId = await workflowApi.GetInstanceItemAsync(request.WorkflowToken, "serviceId");

            var documentId = string.Empty;

            var maxUploadSize = systemSettings.GetProperty<int>("MaxUploadSize");

            var docData = new MemoryStream(Convert.FromBase64String(request.Document));
            var outputData = new MemoryStream();
            ImageUtils.ScaleToSize(docData, maxUploadSize, outputData, ImageFormat.Jpeg);
            outputData.Seek(0, SeekOrigin.Begin);
            request.Document = Convert.ToBase64String(outputData.ToArray());

            if (serviceId == ConfigurationManager.AppSettings["LegalAdviceService"])
            {
                var attachment = new JObject();
                attachment["adviceNumber"] = "000000000";
                attachment["attachment"] = request.Document;
                attachment["attachmentType"] = "DOCUMENT";
                attachment["attachmentName"] = request.DocumentName;
                documentId = await legalAdviceApi.UploadDocument(attachment);

            }
            else if (serviceId == ConfigurationManager.AppSettings["PassportRenewService"]
                    || serviceId == ConfigurationManager.AppSettings["PassportNewService"])
            {
                var txnFile = JObject.FromObject(new
                {
                    txnFile = request.Document,
                    txnFileType = new
                    {
                        fileTypeId = request.DocumentTypeId
                    }
                });
                documentId = await passportServicesApi.UploadDocument(txnFile);
            }
            else
            {
                documentId = await documentApi.AddDocumentAsync(new UploadDocument()
                {
                    DocumentStream = request.Document,
                    DocumentTypeId = request.DocumentTypeId,
                    SystemId = SystemId
                });
            }

            return new RestUploadDocumentResponse
            {
                DocumentId = documentId,
                WorkflowToken = request.WorkflowToken
            };
        }

        public async Task DeleteDocument(RestDeleteDocumentRequest request)
        {
            var serviceId = await workflowApi.GetInstanceItemAsync(request.WorkflowToken, "serviceId");
            if (serviceId == ConfigurationManager.AppSettings["LegalAdviceService"])
            {
                await legalAdviceApi.DeleteDocument(request.DocumentId);
            }
            else if (serviceId == ConfigurationManager.AppSettings["PassportNewService"] ||
                serviceId == ConfigurationManager.AppSettings["PassportRenewService"])
            {
                await passportServicesApi.DeleteDocument(request.DocumentId);
            }
        }

        public async Task<RestSubmitResponse> SubmitDocuments(RestSubmitDocumentRequest request)
        {
            var state = await ResumeWorkflow(request.ToJObject());
            Log.Debug(state.ToJson());
            var response = GetResponse<RestSubmitResponse>(state.ToDictionary());
            return response;
        }

        public async Task<RestSubmitResponse> SubmitAction(RestSubmitActionRequest request)
        {
            var state = await ResumeWorkflow(request.ToJObject());
            var response = GetResponse<RestSubmitResponse>(state.ToDictionary());
            return response;
        }

        public async Task ResubmitApplicationDocument(RestResubmitDocumentRequest request)
        {
            await ApplicationHelper.ValidateApplication(ClaimUtil.GetAuthenticatedUserId(), request.ApplicationId);
            var applicationApi = ApiFactory.Default.GetApplicationApi();
            var applicationStatusApi = ApiFactory.Default.GetApplicationStatusApi();
            var docRequest = new Application.Model.RestApplication
            {
                ApplicationId = request.ApplicationId,
                ApplicationDocument = request.DocumentIds.ToList().Select(x => new Application.Model.RestApplicationDocument
                {
                    DocumenteId = x,
                    SerialNumber = Guid.NewGuid().ToString("N").ToUpper(),
                    DocumentType = ApiFactory.Default.GetDocumentApi().GetDocument(x).DocumentTypeId
                }).ToList()
            };

            Log.Debug("applicationApi.UpdateDocuments : " + Newtonsoft.Json.JsonConvert.SerializeObject(docRequest));

            await applicationStatusApi.UpdateResubmitStatusAsync(request.ApplicationId);

            Log.Debug($"Application status changed to resubmut for application number {request.ApplicationId}");

            await applicationApi.UpdateDocumentsAsync(docRequest);

            Log.Debug($"Document successfully reuploaded for application number {request.ApplicationId}");

            await applicationStatusApi.UnpickApplicationAsync(request.ApplicationId);

            Log.Debug($"Application successfully unpicked for application number {request.ApplicationId}");
        }

        public async Task<RestDocumentsRequiredResponseWrapper> FetchRequiredDocuments(string applicationNumber)
        {
            await ApplicationHelper.ValidateApplication(ClaimUtil.GetAuthenticatedUserId(), applicationNumber);
            var documentsRequired = await ApiFactory.Default.GetApplicationApi().GetApplicationDocumentsRequiredAsync(applicationNumber);

            if (documentsRequired != null && documentsRequired?.ApplicationDocument?.Count > 0)
            {
                Log.Debug($"Rejected documents found against application {applicationNumber}");

                RestDocumentsRequiredResponseWrapper responseDocumentRequired = new RestDocumentsRequiredResponseWrapper
                {
                    Comments = documentsRequired.StatusComments,
                    StatusId = documentsRequired.StatusId
                };

                responseDocumentRequired.Documents = new List<RestDocumentsRequiredResponse>();

                foreach (var doc in documentsRequired.ApplicationDocument)
                {
                    var restDocRequired = mapper.Map<RestDocumentsRequiredResponse>(await documentApi.GetDocumentTypeAsync(doc.DocumentType));
                    restDocRequired.Required = true;
                    if (restDocRequired?.Size == 0)
                        restDocRequired.Size = 100;
                    restDocRequired.RejectionReasons = doc.RejectionReasons;
                    restDocRequired.documentId = doc.DocumenteId;
                    responseDocumentRequired.Documents.Add(restDocRequired);
                }

                return responseDocumentRequired;
            }
            else
            {
                Log.Debug($"Documents not found for application {applicationNumber}");
                return null;
            }
        }

        public async Task<RestPaymentResponse> PayApplication(RestPayApplication restPay)
        {
            Log.Debug($"Going to pay application with data {JsonConvert.SerializeObject(restPay)}");

            var state = await ResumeWorkflow(restPay.ToJObject());
            var dict = state.ToDictionary();

            Log.Debug($"Data received from workflow is {JsonConvert.SerializeObject(dict)}");

            return new RestPaymentResponse() { PaymentCompleteUrl = dict.ContainsKey("PaymentCompleteUrl") ? dict["PaymentCompleteUrl"]?.ToString() : string.Empty };
        }

        public async Task<RestApplicationStatusWrapper> FetchApplicationStatus()
        {
            var applicationStatus = await ApiFactory.Default.GetApplicationApi().GetApplicationStatusAsync();

            return mapper.Map<RestApplicationStatusWrapper>(applicationStatus);
        }

        public virtual Task<Stream> GetImageStream(string documentId)
        {
            Log.Debug($"Going to get image stream for document id {documentId}");

            if (string.IsNullOrEmpty(documentId) || documentId == "null")
                return null;

            var document = documentApi.GetDocument(documentId);

            if (document == null)
                Log.Debug($"Document not found for document id {documentId}");

            MemoryStream documentStream = new MemoryStream(Convert.FromBase64String(document.DocumentStream));
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";

            return Task.FromResult((Stream)documentStream);
        }

        public async Task DeleteApplication(string applicationId)
        {
            await ApplicationHelper.ValidateApplication(ClaimUtil.GetAuthenticatedUserId(), applicationId);
            Log.Debug($"Going to delete application with application id {applicationId}");

            var applicationApi = ApiFactory.Default.GetApplicationApi();

            await applicationApi.DeleteApplicationAsync(applicationId, ClaimUtil.GetAuthenticatedUserId());

            Log.Debug($"Application with application id {applicationId} successfully deleted");
        }

        public async Task<IEnumerable<RestLookup>> GetEntryPermitVisaTypeLookup()
        {
            var entryPermitNew = ConfigurationManager.AppSettings["EntryPermitNewCategoryId"];
            var localizationApi = ApiFactory.Default.GetLocalizationApi();
            var lookups = new List<RestLookup>();

            // Get languages id
            var languages = await localizationApi.GetAllLanguagesAsync();
            var langEn = languages.FirstOrDefault(l => l.TitleEn == "English")?.Id;
            var langAr = languages.FirstOrDefault(l => l.TitleEn == "Arabic")?.Id;

            // Get the list of services under entry permit new category
            var services = (await serviceApi.GetSystemServicesAsync(new Emaratech.Services.Services.Model.Systems { SystemIds = new List<string> { systemSettings.SystemId } }, "1", short.MaxValue.ToString())).Data;
            var entryPermitNewServices = services.Where(s => s.CategoryId == entryPermitNew);

            // Get the translations of these services
            var titles = new RestTitles(entryPermitNewServices.Select(s => s.ResourceKey).ToList());
            var translations = await localizationApi.GetResourcesBySystemAsync(ClaimUtil.GetAuthenticatedSystemId(), titles);

            foreach (var service in entryPermitNewServices)
            {
                var lookup = new RestLookup { Code = service.ServiceId };
                var translation = translations?.FirstOrDefault(x => x.Title == service.ResourceKey);
                lookup.ValueEn = translation?.Values?.FirstOrDefault(x => x.Language == langEn)?.Value;
                lookup.ValueAr = translation?.Values?.FirstOrDefault(x => x.Language == langAr)?.Value;
                lookups.Add(lookup);
            }
            return lookups;
        }

        public async Task<IEnumerable<RestLookup>> GetResidenceTransferPassportVisaTypeLookup()
        {
            var transfereResidencePassportCategory = ConfigurationManager.AppSettings["TransfereResidencePassportCategoryId"];
            var localizationApi = ApiFactory.Default.GetLocalizationApi();
            var lookups = new List<RestLookup>();

            // Get languages id
            var languages = await localizationApi.GetAllLanguagesAsync();
            var langEn = languages.FirstOrDefault(l => l.TitleEn == "English")?.Id;
            var langAr = languages.FirstOrDefault(l => l.TitleEn == "Arabic")?.Id;

            // Get the list of services under entry permit new category
            var services = (await serviceApi.GetSystemServicesAsync(new Emaratech.Services.Services.Model.Systems { SystemIds = new List<string> { systemSettings.SystemId } }, "1", short.MaxValue.ToString())).Data;
            var transfereResidencePassportServices = services.Where(s => s.CategoryId == transfereResidencePassportCategory);

            // Get the translations of these services
            var titles = new RestTitles(transfereResidencePassportServices.Select(s => s.ResourceKey).ToList());
            var translations = await localizationApi.GetResourcesBySystemAsync(ClaimUtil.GetAuthenticatedSystemId(), titles);

            foreach (var service in transfereResidencePassportServices)
            {
                var lookup = new RestLookup { Code = service.ServiceId };
                var translation = translations?.FirstOrDefault(x => x.Title == service.ResourceKey);
                lookup.ValueEn = translation?.Values?.FirstOrDefault(x => x.Language == langEn)?.Value;
                lookup.ValueAr = translation?.Values?.FirstOrDefault(x => x.Language == langAr)?.Value;
                lookups.Add(lookup);
            }
            return lookups;
        }
        private void AllowAnonymousAccess(string serviceId)
        {
            Log.Debug($"Searching mapping matrix for anonymous with service id {serviceId}");

            var anonService = mappingMatrixApi.Search(ConfigurationManager.AppSettings["MappingMatrixAnonymousServices"],
                new MappingMatrix.Model.SearchCriteria
                {
                    IncludeExcluded = false,
                    Values = new List<string> { serviceId }
                });

            Log.Debug($"Anonymous service found in anon mapping matrix  {anonService.Count}");

            if (!ClaimUtil.IsLoggedIn() && anonService.Any())
            {
                ClaimUtil.ImpersonateAnonymousUser();
            }
        }

        public async Task<IEnumerable<RestLookup>> GetEstablishmentList()
        {
            // var ppsId = ClaimUtil.GetUserClaim("PPSID", null);

            var userId = ClaimUtil.GetAuthenticatedUserId();
            var userObj = ApiFactory.Default.GetUserApi().GetUserAsync(userId).Result;
            Log.Debug("User object received from user management api");
            var mappedProfile = Mapper.MapUserProfile(userObj);

            var visionEstablishmentApi = ApiFactory.Default.GetEdnrdIntegrationApi();
            Log.Debug($"Going to get establishment list from vision integration for username {mappedProfile.Username}");
            var establishmentList = await visionEstablishmentApi.GetEstablishmentsListByUserAsync(mappedProfile.Username);
            Log.Debug("Establishment list got from vision integration");

            var listRestLookup = new List<RestLookup>();
            foreach (var list in establishmentList)
            {
                var restLookup = new RestLookup();
                restLookup.Code = list.EstabCode.ToString();
                restLookup.ValueEn = list.EstabNameEn;
                restLookup.ValueAr = list.EstabNameAr;
                listRestLookup.Add(restLookup);
            }
            return listRestLookup.AsEnumerable();
        }

        //Api to get applications which can be refunded
        public async Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplications(string refundType)
        {
            IRefund refund = RefundFactory.GetRefundType(ClaimUtil.GetAuthenticatedUserId(), ClaimUtil.GetAuthenticatedUserType(), SystemId, refundType);
            var refundableApplication = await refund.FetchRefundableApplications();
            refundableApplication.ToList().ForEach(p => p.RefundType = refundType);
            return refundableApplication;
        }

        public async Task<RestRefundableApplication> FetchRefundableApplicationDetail(string applicationId, string refundType)
        {
            IRefund refund = RefundFactory.GetRefundType(ClaimUtil.GetAuthenticatedUserId(), ClaimUtil.GetAuthenticatedUserType(), SystemId, refundType);
            var refundableApplication = await refund.FetchRefundableApplicationDetail(applicationId);
            refundableApplication.RefundType = refundType;
            return refundableApplication;
        }

        public async Task ProcessApplicationRefund(string refundType, RestRefundRequest refundRequest)
        {
            await ApplicationHelper.ValidateApplication(ClaimUtil.GetAuthenticatedUserId(), refundRequest?.ApplicationId);
            if (string.IsNullOrEmpty(refundRequest?.ApplicationId))
                throw ChannelErrorCodes.InvalidInformationForRefund.ToWebFault("Application id for refund is empty");

            IRefund refund = RefundFactory.GetRefundType(ClaimUtil.GetAuthenticatedUserId(), ClaimUtil.GetAuthenticatedUserType(), SystemId, refundType);
            var refundInfo = await refund.ProcessApplicationRefund(refundRequest);

            Log.Info($"Going to mark application with application id {refundRequest.ApplicationId} as refund requested");
            await ApiFactory.Default.GetApplicationStatusApi().UpdateRefundInitiatedStatusAsync(refundRequest.ApplicationId, ConstantMessageCodes.IssueWithRefundRequest);

            Log.Info($"Going to process refund with parameters {JsonConvert.SerializeObject(refundRequest)}");
            RestPaymentStatus refundResponse;
            try
            {
                refundResponse = await paymentApi.ProcessRefundAsync(refundInfo);
            }
            catch (Exception exp)
            {
                Log.Error(exp);
                throw ChannelErrorCodes.RefundFailed.ToWebFault(nameof(ChannelErrorCodes.RefundFailed));
            }

            Log.Info($"Refund response is {JsonConvert.SerializeObject(refundResponse)}");

            if (refundResponse.Status == true || refundResponse.Error == ConstantMessageCodes.TransactionAlreadyRefunded)
            {
                Log.Info($"Going to update application refund status for application id {refundRequest.ApplicationId}");
                await ApiFactory.Default.GetApplicationStatusApi().UpdateRefundRequestedStatusAsync(refundRequest.ApplicationId, ConstantMessageCodes.RefundRequestSuccessful);
            }
            else
            {
                Log.Info($"Refund not successful from noqodi for application {refundRequest.ApplicationId} and response is {JsonConvert.SerializeObject(refundResponse)}");
                await ApiFactory.Default.GetApplicationStatusApi().UpdateRefundFailedStatusAsync(refundRequest.ApplicationId, string.IsNullOrEmpty(refundResponse?.Error) ? nameof(ChannelErrorCodes.RefundFailed) : refundResponse.Error);
                throw ChannelErrorCodes.RefundFailed.ToWebFault(refundResponse.Error);
            }
        }


        //This is a temporary method for old mobile release support, need to remove it after mobile new release push
        public async Task<IEnumerable<RestRefundableApplication>> FetchRefundableApplicationsList()
        {
            return await GetRefundedAplications(string.Empty);
        }

        //Api to get all applications to show on application refund status page

        public async Task<IEnumerable<RestRefundableApplication>> FetchRefundedApplications(string applicationId)
        {
            return await GetRefundedAplications(applicationId);
        }

        private async Task<IEnumerable<RestRefundableApplication>> GetRefundedAplications(string applicationId)
        {
            Log.Debug("Going to fetch refundable applications list");
            RefundBase refund = new RefundBase(ClaimUtil.GetAuthenticatedUserId(), ClaimUtil.GetAuthenticatedUserType(), SystemId);

            var searchCriteria = refund.GetAllRefundedApplicationsParams(applicationId);
            var searchResult = (await ApplicationHelper.GetApplicationSearchResult(searchCriteria)).ToList();
            List<RestRefundableApplication> lstRefundableApplications = new List<RestRefundableApplication>();

            foreach (var application in searchResult)
            {
                applicationId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
                var refundInfo = await ApiFactory.Default.GetApplicationApi().GetRefundDetailsAsync(applicationId);
                if (refundInfo == null)
                {
                    Log.Debug($"Refund details not found for application {applicationId} thats why skipping application");
                    continue;
                }

                var refundableApplication = new RestRefundableApplication
                {
                    ApplicationId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value,
                    TransactionBatchId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "transactionbatchid")?.Value,
                    StatusDate = Convert.ToDateTime(refundInfo.RefundRequestDate.ToString()),
                    Status = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "statusid")?.Value,
                    TotalAmount = refundInfo.TotalAmount.ToInt32(),
                    RefundableAmount = refundInfo.RefundAmount.ToInt32(),
                    PaymentType = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "payment_type")?.Value,
                    CreatedDate = Convert.ToDateTime(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "createddate")?.Value),
                    RefundType = refundInfo.RefundType,
                    FullName = new RestName
                    {
                        En = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "fullnamee")?.Value,
                        Ar = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "fullnamea")?.Value
                    },

                    RefundRequestDate = refundInfo.RefundRequestDate,
                    ServiceId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "serviceid")?.Value
                };

                refundableApplication.Actions = await refund.GetRefundActions(refundableApplication.Status, refundableApplication.ServiceId);

                if (refundableApplication.Status == Convert.ToString((int)ApplicationStatus.RefundInitiated))
                    refundableApplication.ErrorCode = ConstantMessageCodes.IssueWithRefundRequest;

                if (refundableApplication.Status == Convert.ToString((int)ApplicationStatus.RefundFailed))
                {
                    refundableApplication.ErrorCode = refundInfo.RefundStatus;
                    if (!(refundInfo.RefundStatus == ConstantMessageCodes.BeneficiaryDoesNotExist || refundInfo.RefundStatus == ConstantMessageCodes.InvalidIBANNo || refundInfo.RefundStatus == ConstantMessageCodes.InvalidBankName || refundInfo.RefundStatus == ConstantMessageCodes.InvalidBeneficiaryName || refundInfo.RefundStatus == ConstantMessageCodes.BankMismatch || refundInfo.RefundStatus == ConstantMessageCodes.InvalidLengthBeneficiaryName))
                        refundableApplication.Actions = ApplicationHelper.RemoveAction(refundableApplication.Actions, ApplicationActions.NewRefund);
                }

                if (refundableApplication.Status == Convert.ToString((int)ApplicationStatus.RefundRequested))
                    refundableApplication.ErrorCode = ConstantMessageCodes.RefundRequestSuccessful;

                if (refundableApplication.Status == Convert.ToString((int)ApplicationStatus.Refunded) || refundableApplication.Status == Convert.ToString((int)ApplicationStatus.TechnicalRefunded))
                    refundableApplication.ErrorCode = ConstantMessageCodes.RefundSuccessful;

                lstRefundableApplications.Add(refundableApplication);
            }
            Log.Debug($"Refund application status is {JsonConvert.SerializeObject(lstRefundableApplications)}");

            return lstRefundableApplications.AsEnumerable();

        }

        public async Task UpdateIncompletePaymentStatus(string workflowToken)
        {
            if (!string.IsNullOrEmpty(workflowToken))
            {
                Log.Debug($"Workflow token is {workflowToken}");
                var applicationId = await workflowApi.GetInstanceItemAsync(workflowToken, "applicationId");

                if (!string.IsNullOrEmpty(applicationId))
                {
                    Log.Debug($"Going to mark application id {applicationId} as incomplete payment");
                    await ApiFactory.Default.GetApplicationStatusApi().UpdateIncompletePaymentStatusAsync(applicationId);
                }
            }
            else
            {
                Log.Debug("Workflow token is empty in request");
                throw ChannelErrorCodes.BadRequest.ToWebFault("Workflow token is empty");
            }
        }

        public async Task<IEnumerable<RestDependentApplicationInfo>> GetIncompletePaymentApplications()
        {
            Log.Debug($"Going to perform search of incomplete payment applications");

            var applicationSearchResult = await ApplicationHelper.GetApplicationSearchResult(ApplicationHelper.GetIncompleteApplicationSearchCriteria(ClaimUtil.GetAuthenticatedUserId()));
            var incompletePaymentApplications = applicationSearchResult.Select(application => new RestDependentApplicationInfo
            {
                ApplicationNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value,
                ApplicationStatus = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "mapped_statusid")?.Value,
                Status = LookupHelper.GetLookupIdByVisaStatus(VisaStatus.InProgress),
                CreatedDate = Convert.ToDateTime(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "created_date")?.Value),
                VisaType = LookupHelper.GetLookupIdByVisaTypeCol2(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "filetypeid")?.Value),
                FirstName =
                    new RestName()
                    {
                        En = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "firstnamee")?.Value,
                        Ar = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "firstnamea")?.Value
                    },
                LastName =
                    new RestName()
                    {
                        En = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "lastnamee")?.Value,
                        Ar = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "lastnamea")?.Value,
                    },
                Nationality = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "currentnationalityid")?.Value,
                Relationship = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "sponsorrelationid")?.Value,
                VisaTypeId = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visatypeid")?.Value,
                VisaNumber = application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumber")?.Value,
                VisaNumberType = LookupHelper.GetLookupIdByVisaTypeCol2(application.RestApplicationSearchKeyValues?.FirstOrDefault(p => p.PropertyName.ToLower() == "visanumbertype")?.Value)
            }).ToList();

            Log.Debug($"Search performed for incomplete payment and rows found {incompletePaymentApplications.Count}");
            return incompletePaymentApplications.AsEnumerable();
        }
    }

}