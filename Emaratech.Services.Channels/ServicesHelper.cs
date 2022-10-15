using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Application.Api;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.BusinessLogic.Dashboard;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Contracts.Rest.Models.LegalAdvice;
using Emaratech.Services.Channels.Models.Enums;
using Emaratech.Services.Common.Caching;
using Emaratech.Services.DataContracts.Api;
using Emaratech.Services.DataContracts.Model;
using Emaratech.Services.Document.Api;
using Emaratech.Services.Document.Model;
using Emaratech.Services.Email.Api;
using Emaratech.Services.Email.Model;
using Emaratech.Services.Fee.Api;
using Emaratech.Services.Fee.Model;
using Emaratech.Services.Forms.Api;
using Emaratech.Services.Forms.Model;
using Emaratech.Services.Localization.Api;
using Emaratech.Services.Lookups.Api;
using Emaratech.Services.Lookups.Model;
using Emaratech.Services.MappingMatrix.Api;
using Emaratech.Services.MappingMatrix.Model;
using Emaratech.Services.Payment.Api;
using Emaratech.Services.Payment.Model;
using Emaratech.Services.Security.KeyVault.Api;
using Emaratech.Services.Security.KeyVault.Model;
using Emaratech.Services.Systems.Api;
using Emaratech.Services.Systems.Properties;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.Vision.Model;
using Emaratech.Services.VisionIntegration.Api;
using Emaratech.Services.Zajel.Api;
using Emaratech.Services.Zajel.Model;
using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels
{
    public static class ServicesHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServicesHelper));

        private const string LookupCacheKeyPrefix = "Lookup";
        private const string FormCacheKeyPrefix = "Form";
        private const string MappingMatrixCacheKeyPrefix = "MappingMatrix";

        public static ISystemProperties SystemProperties { get; set; }
        public static ISystemApi SystemApi { get; set; }
        private static IMappingMatrixApi mappingMatrixApi;
        private static IDocumentApi documentApi;
        private static IDataContractApi dataContractApi;
        public static IVisionIndividualApi VisionApi { get; set; }
        public static IVisionEstablishmentApi VisionEstabApi { get; set; }
        public static IApplicationApi ApplicationApi { get; set; }
        private static IApplicationStatusApi applicationStatusApi;
        public static IApplicationSearchApi ApplicationSearchApi { get; set; }
        public static ILookupApi LookupApi { get; set; }
        private static IFeeApi feeApi;
        private static IPaymentsApi paymentApi;
        private static IFormsApi formsApi;
        private static Forms.Api.ILookupsApi formsLookupsApi;
        private static IEDNRDIntegrationApi ednrdIntegrationApi;
        public static IEmailApi EmailApi { get; set; }
        private static IZajelApi zajelApi;
        private static ILegalAdviceApi legalAdviceApi;
        private static IPassportServicesApi passportServicesApi;
        public static ITokensApi TokenApi { get; set; }

        public static void Init(IServiceFactory serviceFactory, ISystemProperties properties)
        {
            SystemProperties = properties;
            SystemApi = serviceFactory.GetSystemApi();
            mappingMatrixApi = serviceFactory.GetMappingMatrixApi();
            documentApi = serviceFactory.GetDocumentApi();
            dataContractApi = serviceFactory.GetDataContractApi();
            VisionApi = serviceFactory.GetVisionApi();
            ApplicationApi = serviceFactory.GetApplicationApi();
            applicationStatusApi = serviceFactory.GetApplicationStatusApi();
            ApplicationSearchApi = serviceFactory.GetApplicationSearchApi();
            LookupApi = serviceFactory.GetLookupApi();
            feeApi = serviceFactory.GetFeeApi();
            paymentApi = serviceFactory.GetPaymentApi();
            formsApi = serviceFactory.GetFormsApi();
            serviceFactory.GetFormsLayoutApi();
            formsLookupsApi = serviceFactory.GetFormsLookupsApi();
            serviceFactory.GetFormsFieldSetsApi();
            serviceFactory.GetLocalizationApi();
            ednrdIntegrationApi = serviceFactory.GetEdnrdIntegrationApi();
            EmailApi = serviceFactory.GetEmailApi();
            zajelApi = serviceFactory.GetZajelApi();
            VisionEstabApi = serviceFactory.GetVisionEstablishmentApi();
            legalAdviceApi = serviceFactory.GetLegalAdviceApi();
            passportServicesApi = serviceFactory.GetPassportServicesApi();
            TokenApi = serviceFactory.GetTokensApi();
        }

        public static async Task<string> GetSystemProperty(string systemId, string propertyName)
        {
            var parent = propertyName.Split('.')[0];
            return await SystemProperties.Property(systemId, propertyName, parent);
        }

        #region Mapping matrix

        private static string SearchCriteriaToCacheKey(string mappingMatrix, SearchCriteria sc)
        {
            var values = sc.ValuesDict != null
                    ? string.Join("_", sc.ValuesDict.Select(x => $"{x.Name}_{x.Value}_{x.Version}"))
                    : string.Join("_", sc.Values);
            return $"{mappingMatrix}_{sc.IncludeExcluded}_{values}_{sc.ResolveExpressions}";
        }

        public static async Task<IEnumerable<T>> ResolveMappingMatrix<T>(string mappingMatrixId, List<SearchVersion> parameters, Func<IList<string>, IList<long?>, T> extractor, bool resolveExpression = false)
        {
            var searchCriteria = new SearchCriteria
            {
                IncludeExcluded = false,
                ValuesDict = parameters,
                ResolveExpressions = resolveExpression
            };
            var items = await Cache.Default.Run(MappingMatrixCacheKeyPrefix, SearchCriteriaToCacheKey(mappingMatrixId, searchCriteria),
                () => mappingMatrixApi.SearchAsync(mappingMatrixId, searchCriteria));
            return items.Select(x => extractor(x.Values, x.Versions)).ToList();
        }

        public static async Task<IEnumerable<T>> ResolveMappingMatrix<T>(string mappingMatrixId, List<string> values, Func<IList<string>, IList<long?>, T> extractor)
        {
            var searchCriteria = new SearchCriteria
            {
                IncludeExcluded = false,
                Values = values
            };
            var items = await Cache.Default.Run(MappingMatrixCacheKeyPrefix, SearchCriteriaToCacheKey(mappingMatrixId, searchCriteria),
                () => mappingMatrixApi.SearchAsync(mappingMatrixId, searchCriteria));
            return items.Select(x => extractor(x.Values, x.Versions)).ToList();
        }

        public static Task<MatrixWithSystemsDimensions> GetMappingMatrix(string matrixId)
        {
            return Cache.Default.Run(MappingMatrixCacheKeyPrefix, matrixId,
                () => mappingMatrixApi.GetMappingMatrixWithSystemsDimensionsAsync(matrixId));
        }

        public static List<MappingMatrix.Model.Item> GetMappingMatrixItems(string matrixId)
        {
            var dimensions = Cache.Default.Run(MappingMatrixCacheKeyPrefix, matrixId,
                () => mappingMatrixApi.GetMappingMatrixDimensions(matrixId, string.Empty));

            var items = mappingMatrixApi.Search(
                            matrixId,
                            new MappingMatrix.Model.SearchCriteria
                            {
                                IncludeExcluded = false,
                                Values = new List<string> { "", "" },
                            });
            return items;
        }

        #endregion Mapping matrix

        #region Vision
        public static async Task<RestIndividualUserFormInfo> GetIndividualProfileByResidenceNo(string residenceNo)
        {
            return await VisionApi.GetIndividualDetailedInformationByResNoAsync(residenceNo);
        }

        public static async Task<RestIndividualUserFormInfo> GetIndividualProfileByPpsId(string ppsId)
        {
            return await VisionApi.GetIndividualDetailedInformationByPpsIdAsync(ppsId);
        }

        public static async Task<RestIndividualInfo> GetIndividualDetailedInformation(string emiratesId, DateTime dateOfBirth)
        {
            return await VisionApi.GetIndividualDetailedInformationAsync(emiratesId, dateOfBirth);
        }

        public static async Task<RestEstablishmentProfileInfo> GetEstablishmentProfile(string establishmentCode)
        {
            return await VisionEstabApi.GetEstablishmentProfileAsync(establishmentCode);
        }

        public static async Task<RestEstablishmentSummary> GetEstablishmentDetailedStats(string establishmentCode)
        {
            return await VisionEstabApi.GetEstablishmentDetailedStatsAsync(establishmentCode);
        }

        public static async Task<RestEstablishmentSummary> GetEstablishmentDetailedStatsReport(string establishmentCode)
        {
            return await VisionEstabApi.GetEstablishmentDetailedStatsReportAsync(establishmentCode);
        }

        public static async Task<RestIndividualUserFormInfo> GetIndividualProfileByPermitNo(string permitNo)
        {
            return await VisionApi.GetIndividualDetailedInformationByPermitNoAsync(permitNo);
        }

        public static async Task<List<RestIndividualUserFormInfo>> GetIndividualProfileListByPermitNoForReport(List<string> permitNo)
        {
            return await VisionApi.GetIndividualDetailedInformationListByPermitNoForReportAsync(new RestVisionQueryCriteria { Values = permitNo });
        }

        public static async Task<List<RestIndividualUserFormInfo>> GetIndividualProfileListByResNoForReport(List<string> ResNo)
        {
            return await VisionApi.GetIndividualDetailedInformationListByResNoForReportAsync(new RestVisionQueryCriteria { Values = ResNo });
        }

        public static async Task<RestIndividualDependentsSummary> GetIndividualDependentSummary(string sponsorNo)
        {
            return await VisionApi.GetDependentsSummaryAsync(sponsorNo);
        }

        public static async Task<RestIndividualUserFormInfo> GetIndividualProfileByPassportInfo(string passportNumber, string birthYear, string nationalityId)
        {
            return await VisionApi.GetIndividualDetailedInformationByPassportNoAsync(passportNumber, birthYear, nationalityId);
        }

        public static async Task<RestIndividualUserFormInfo> GetIndividualDetailedAutoFill(string passportNumber, string uid, string birthYear, string nationalityId)
        {
            return await VisionApi.GetIndividualDetailedAutoFillAsync(new RestIndividualAutoFill
            {
                PassportNo = passportNumber,
                UnifiedNo = uid,
                BirthYear = birthYear,
                NationalityId = nationalityId
            });
        }

        public static async Task<RestIndividualUserFormInfo> GetIndividualDetailedInfoBySponsorNo(string sponsorNo)
        {
            return await VisionApi.GetIndividualDetailedInformationBySponsorNoAsync(sponsorNo);
        }

        public static async Task<RestIndividualDependentsInfo> GetIndividualDependentsInformation(string sponsorNo)
        {
            return await VisionApi.GetIndividualDependentsInformationAsync(sponsorNo);
        }

        public static async Task<IEnumerable<RestIndividualDependentsInfoWrapper>> GetDependents(string sponsorFileNo)
        {
            var res = await VisionApi.GetIndividualDependentsInformationAsync(sponsorFileNo);
            return res.IndividualDependents;
        }

        public static async Task<IEnumerable<RestIndividualDependentsInfoWrapper>> GetEstablishmentDependents(string establishmentCode)
        {
            var dependents = await VisionEstabApi.GetEstablishmentDependentsInformationAsync(establishmentCode);
            return dependents.EstablishmentDependents;
        }

        public static async Task<RestIndividualHealthTestResult> GetIndividualHealthTestInformation(string residenceNo)
        {
            return await VisionApi.GetIndividualHealthTestInformationAsync(residenceNo);
        }

        public static async Task<RestIndividualResidenceInfo> GetIndividualResidenceInfo(string residenceNo)
        {
            return await VisionApi.GetIndividualResidenceInfoAsync(residenceNo);
        }

        public static async Task<RestIndividualResidenceInfo> GetIndividualResidenceInfoBySponsorNo(string sponsorNo)
        {
            return await VisionApi.GetIndividualResidenceInfoBySponsorNoAsync(sponsorNo);
        }

        public static async Task<RestIndividualViolationInfo> GetIndividualViolationByResNo(string residenceNo)
        {
            return await VisionApi.GetIndividualViolationByResNoAsync(residenceNo);
        }

        public static async Task<RestIndividualViolationInfo> GetIndividualViolationByPermitNo(string permitNo)
        {
            return await VisionApi.GetIndividualViolationByPermitNoAsync(permitNo);
        }

        public static async Task<int?> GetIndividualViolationPaymentHistoryByPermitNo(string permitNo)
        {
            return await VisionApi.GetIndividualViolationPaymentByPermitNoAsync(permitNo);
        }

        public static async Task<int?> GetIndividualViolationPaymentHistoryByResNo(string residenceNo)
        {
            return await VisionApi.GetIndividualViolationPaymentByResNoAsync(residenceNo);
        }

        public static async Task<RestIndividualTravelInfo> GetIndividualCurrentTravelStatusByPpsId(string ppsId)
        {
            if (!string.IsNullOrEmpty(ppsId))
                return (await VisionApi.GetIndividualCurrentTravelStatusByPpsIdAsync(new RestVisionQueryCriteria { Values = new List<string> { ppsId } })).FirstOrDefault();

            return null;
        }

        public static async Task<RestIndividualTravelInfo> GetIndividualCurrentTravelStatusByResNo(string resNo)
        {
            if (!string.IsNullOrEmpty(resNo))
                return (await VisionApi.GetIndividualCurrentTravelStatusByResNoAsync(new RestVisionQueryCriteria { Values = new List<string> { resNo } })).FirstOrDefault();
            return null;
        }

        public static async Task<RestIndividualTravelInfo> GetIndividualCurrentTravelStatusByPermitNo(string permitNo)
        {
            if (!string.IsNullOrEmpty(permitNo))
                return (await VisionApi.GetIndividualCurrentTravelStatusByPermitNoAsync(new RestVisionQueryCriteria { Values = new List<string> { permitNo } })).FirstOrDefault();
            return null;
        }

        public static async Task<List<RestIndividualTravelInfo>> GetIndividualCurrentTravelStatusListByPermitNo(List<string> permitNo)
        {

            return (await VisionApi.GetIndividualCurrentTravelStatusByPermitNoAsync(new RestVisionQueryCriteria { Values = permitNo }));

        }

        public static async Task<List<RestIndividualTravelInfo>> GetIndividualCurrentTravelStatusListByResNo(List<string> resNo)
        {

            return (await VisionApi.GetIndividualCurrentTravelStatusByResNoAsync(new RestVisionQueryCriteria { Values = resNo }));

        }

        public static async Task<List<RestIndividualTravelInfo>> GetIndividualTravelHistoryByResNo(string residenceNo)
        {
            if (!string.IsNullOrEmpty(residenceNo))
                return await VisionApi.GetIndividualTravelHistoryByResNoAsync(residenceNo);
            return null;
        }

        public static async Task<List<RestIndividualTravelInfo>> GetIndividualTravelHistoryByPermitNo(string permitNo)
        {
            if (!string.IsNullOrEmpty(permitNo))
                return await VisionApi.GetIndividualTravelHistoryByPermitNoAsync(permitNo);
            return null;
        }

        public static async Task<bool?> UpdateIndividualViolation(int amount, string referenceNumber, string ppsid, int? channelId = 2)
        {
            return await VisionApi.UpdateIndividualViolationAsync(new RestIndividualViolationPayment
            {
                ChannelId = channelId,
                EChannelAppId = null,
                PaidAmount = amount,
                PaymentDate = DateTime.Now,
                PaymentReferenceNo = referenceNumber,
                PPSID = ppsid,
                ViolationFineClassTypeId = 1
            });
        }

        public static async Task<List<RestDependentGenericInfo>> GetNumberOfWifes(string sponsorNo)
        {
            return await VisionApi.GetWifeCountAsync(sponsorNo);
        }

        public static async Task<bool?> UpdateSponsorVerifyStatus(string sponsorNumber)
        {
            return await VisionApi.UpdateSponsorVerifyStatusAsync(sponsorNumber);
        }

        public static async Task<RestIndividualSponsorshipInfo> GetSponsorInfoBySponsorNo(string sponsorNumber)
        {
            return await VisionApi.GetSponsorInfoBySponsorNoAsync(sponsorNumber);
        }

        #endregion Vision

        public static async Task<IEnumerable<RestLookupDetail>> GetLookupItems(string lookupId)
        {
            return await Cache.Default.Run(LookupCacheKeyPrefix, lookupId, async () =>
             {
                 Log.Debug("LookupId ==> " + lookupId);
                 return (await LookupApi.GetLookupDetailsAsync(lookupId, string.Empty, string.Empty)).Data;
             });
        }

        #region Application
        public static async Task<string> SaveApplication(RestApplication application)
        {
            return await ApplicationApi.SaveAsync(application);
        }

        public static async Task UpdateApplicationBatchId(string applicationId, string batchId)
        {
            await ApplicationApi.UpdateApplicationFieldAsync(applicationId, new RestApplicationSearchKeyValues
            {
                PropertyName = "TRANSACTIONBATCHID",
                Entity = "ApplicationDetails",
                Value = batchId
            });
        }

        public static async Task UpdateApplicationCompleteStatus(string applicationId)
        {
            await applicationStatusApi.UpdateCompleteStatusAsync(applicationId);
        }

        public static async Task UpdateApplicationIncompleteDocumentStatus(string applicationId)
        {
            await applicationStatusApi.UpdateIncompleteDocumentStatusAsync(applicationId);
        }

        public static async Task UpdateApplicationIncompletePayment(string applicationId)
        {
            await applicationStatusApi.UpdateIncompletePaymentStatusAsync(applicationId);
        }

        public static async Task UpdateApplicationFailedPaymentStatusAsync(string applicationId)
        {
            await applicationStatusApi.UpdateFailedPaymentStatusAsync(applicationId);
        }

        public static async Task UpdateApplicationPaymentStatus(string applicationId, string batchId, int paymentType)
        {
            await applicationStatusApi.UpdatePaymentStatusAsync(applicationId, batchId, paymentType);
        }

        public static async Task<int?> UpdateApplicationDocuments(string applicationId, IEnumerable<RestApplicationDocument> documents)
        {
            return await ApplicationApi.UpdateDocumentsAsync(
                new RestApplication
                {
                    ApplicationId = applicationId,
                    ApplicationDocument = documents.ToList()
                });
        }

        public static async Task<int?> UpdateApplicationFees(string applicationId, IEnumerable<RestApplicationFee> fees)
        {
            return await ApplicationApi.UpdateFeesAsync(
                new RestApplication
                {
                    ApplicationId = applicationId,
                    ApplicationFee = fees.ToList()
                });
        }

        public static async Task<bool> IsApplicationExist(string serviceId, string passportNumber, string nationalityId)
        {
            var result = await ApplicationSearchApi.GetApplicationsByCriteriaAsync(new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ApplicationId"
                    }
                },
                ServiceIds = new List<string> { serviceId },
                PassportNumber = passportNumber,
                NationalityId = nationalityId,
                IsPaid = "false"
            });

            return result?.RestApplicationSearchRow?.ToList().Count > 1;
        }

        public static async Task<bool> IsApplicationExistForStatuses(List<string> serviceIds, string passportNumber, string nationalityId)
        {
            var applicationSearchResult = (await ApplicationSearchApi.GetApplicationsByCriteriaAsync(new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ApplicationId"
                    }
                    ,new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "StatusId"
                    }
                },
                ServiceIds = serviceIds,
                PassportNumber = passportNumber,
                NationalityId = nationalityId,
                StatusIdNotIn = ConfigurationManager.AppSettings["StatustoCheckExistingApplications"]
            })).RestApplicationSearchRow.FirstOrDefault();

            if (applicationSearchResult != null)
            {
                var applicationId = applicationSearchResult.RestApplicationSearchKeyValues.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
                var statusId = applicationSearchResult.RestApplicationSearchKeyValues.FirstOrDefault(p => p.PropertyName.ToLower() == "statusid")?.Value;
                Log.Debug($"Application already exists for passport no {passportNumber} with application id {applicationId} and status id {statusId}");
            }

            return applicationSearchResult != null;
        }

        public static async Task<bool> IsApplicationExistForStatuses(string serviceId, string visaNumber)
        {

            var applicationSearchResult = (await ApplicationSearchApi.GetApplicationsByCriteriaAsync(new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ApplicationId"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "StatusId"
                    }
                },
                //ServiceIds = new List<string> { serviceId }, // Removing services to validate all scenarios
                VisaNumber = visaNumber,
                StatusIdNotIn = ConfigurationManager.AppSettings["StatustoCheckExistingApplications"],
                ServiceIdsNotIn = new List<string> { Constants.Services.EntryPermitCancelPrivateService, Constants.Services.ResidenceCancelService }
                //IsPaid = "true" 
            })).RestApplicationSearchRow.FirstOrDefault();

            if (applicationSearchResult != null)
            {
                var applicationId = applicationSearchResult.RestApplicationSearchKeyValues.FirstOrDefault(p => p.PropertyName.ToLower() == "applicationid")?.Value;
                var statusId = applicationSearchResult.RestApplicationSearchKeyValues.FirstOrDefault(p => p.PropertyName.ToLower() == "statusid")?.Value;
                Log.Debug($"Application already exists for visa number {visaNumber} with application id {applicationId} and status id {statusId}");
            }

            return applicationSearchResult != null;
        }

        public static async Task<string> GetApplicationId()
        {
            return await ApplicationApi.GetUnifiedApplicationIdAsync();
        }

        public static async Task<RestApplicationSearchRow> SearchApplicationByApplicationId(string applicationId)
        {
            var applicationSearchResult = (await ApplicationSearchApi.GetApplicationsByCriteriaAsync(new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "*"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicantDetails",
                        PropertyName = "*"
                    },
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "SponsorDetails",
                        PropertyName = "*"
                    },
                },
                ApplicationId = applicationId
            })).RestApplicationSearchRow.ToList();


            var applicationSearchRow = applicationSearchResult.FirstOrDefault(a => a.RestApplicationSearchKeyValues.Any(p =>
                                            p.PropertyName.ToLower() == "applicationid" && p.Value == applicationId));

            return applicationSearchRow;
        }


        public static async Task<bool> IsApplicationExist(string serviceId, string visaNumber)
        {

            var applicationSearchResult = await ApplicationSearchApi.GetApplicationsByCriteriaAsync(new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                {
                    new RestApplicationSearchKeyValues
                    {
                        Entity = "ApplicationDetails",
                        PropertyName = "ApplicationId"
                    }
                },
                ServiceIds = new List<string> { serviceId },
                VisaNumber = visaNumber,
                IsPaid = "true"
            });

            return applicationSearchResult?.RestApplicationSearchRow?.ToList().Count > 0;
        }

        public static async Task<RestApplicationFee> GetFeeByFeeTypeId(string userId, string feeTypeId)
        {
            return await ApplicationApi.GetFeeByFeeTypeIdAsync(userId, feeTypeId);
        }
        #endregion Application

        #region Document
        public static async Task<IEnumerable<RestDocument>> SearchDocuments(string mappingMatrixId, List<DocumentParameter> parameters)
        {
            var docInput = new DocumentInput
            {
                MappingMatrixId = mappingMatrixId,
                Parameters = parameters
            };
            return await documentApi.GetServiceDocumentsAsync(docInput);
        }

        public static async Task<IEnumerable<RestApplicationDocument>> GetApplicationDocuments(string applicationId)
        {
            return await ApplicationApi.GetApplicationDocumentsAsync(applicationId);
        }
        public static async Task<UploadDocument> GetDocument(string documentId)
        {
            return await documentApi.GetDocumentAsync(documentId);
        }
        public static async Task<IEnumerable<UploadDocument>> GetDocumentInfo(params string[] documentIds)
        {

            return await documentApi.GetDocumentInfosAsync(new RestDocumentInfoRequest(documentIds.ToList()));
        }
        #endregion Document

        #region Forms service


        public static Task<IEnumerable<EmaratechServicesFormsContractsRestModelsRestNamedType>> GetPlatforms()
        {
            return Cache.Default.Run(FormCacheKeyPrefix, "Platforms", async () => (IEnumerable<EmaratechServicesFormsContractsRestModelsRestNamedType>)await formsLookupsApi.EmaratechServicesFormsFormsServiceGetPlatformsAsync());
        }

        public static async Task<RestRenderGraph> RenderFormForPlatform(string formId, string platformId)
        {
            return await Cache.Default.Run(FormCacheKeyPrefix, $"{formId}_{platformId}", () => GetFormWithFieldLengthValidation(formId, platformId));
        }

        private static async Task<RestRenderGraph> GetFormWithFieldLengthValidation(string formId, string platformId)
        {
            var conf = await formsApi.EmaratechServicesFormsFormsServiceRenderFormGraphForPlatformAsync(formId, platformId);

            var fields = DataHelper.GetFormFieldByEntity(conf);
            var orignalFields = new List<RestRenderItem>();
            var entities = fields.Select(kvp => new RestApplicationFields
            {
                Entity = kvp.Key,
                Fields = kvp.Value
                            .Where(x => x.OriginalField.FieldTypeId == Constants.FieldTypeTextBox || x.OriginalField.FieldTypeId == Constants.FieldTypeSummary)
                            .Select(x =>
                            {
                                orignalFields.Add(x.OriginalField);
                                return x.Name;
                            })
                            .ToList()
            }).ToList();

            var metadata = await ApplicationApi.GetApplicationFieldsMetadataAsync(new RestApplicationEntities { Entities = entities });

            foreach (var fieldMetadata in metadata)
            {
                var originalField = orignalFields
                    .FirstOrDefault(x => x.Entity == fieldMetadata.Entity && x.Name == fieldMetadata.Name);
                if (originalField != null)
                {
                    var lengthValidation = new EmaratechServicesFormsContractsRestModelsRestFieldValidation
                    {
                        ResourceKey = "MaxLength",
                        Stop = true,
                        ValidationName = string.Empty,
                        ValidationTypeId = Constants.MaxLengthFieldValidation,
                        Data2 = fieldMetadata.MaxLength.ToString()
                    };

                    if (originalField.Validations == null)
                    {
                        originalField.Validations = new List<EmaratechServicesFormsContractsRestModelsRestFieldValidation>();
                    }
                    originalField.Validations.Add(lengthValidation);
                }
            }

            return conf;
        }
        #endregion Forms service

        public static async Task<IEnumerable<FeeConfiguration>> GetFees(string mappingMatrixId, List<FeeParameter> parameters)
        {
            var feeInput = new FeeInput
            {
                MappingMatrixId = mappingMatrixId,
                Parameters = parameters
            };
            return await feeApi.GetFeeAsync(feeInput);
        }

        #region Payment service
        public static async Task<PaymentConfiguration> GetPaymentConfiguration()
        {
            return await paymentApi.GetPaymentConfigurationAsync();
        }

        public static async Task<string> GetPaymentBatchId()
        {
            return await paymentApi.GenerateBatchIdAsync();
        }

        public static async Task<RestPaymentResponse> Pay(string batchId, string merchantId, long amount, string systemId, string forwardUrl, IEnumerable<RestBeneficiary> beneficiaries, bool? isVirtualBankEnabled, string channel = "noqodi", string passcode = "")
        {
            if (isVirtualBankEnabled == true)
            {
                var randomBatchId = Convert.ToString(new Random().Next(23231, 2123324322));

                return new RestPaymentResponse
                {
                    BatchId = randomBatchId,
                    ForwardURL = forwardUrl + "&batchid=" + randomBatchId,
                    PostData = "http://localhost/VirualBank/VirtualBankResponse.html"
                };
            }

            var paymentInfos = new RestPaymentInfo
            {
                BatchId = batchId,
                RedirectURL = forwardUrl,
                PaymentChannelName = channel,
                PaymentChannelPasscode = passcode,
                SystemId = systemId,
                TotalAmount = amount.ToString(),
                Beneficiaries = beneficiaries?.ToList(),
                EpgMerchant = merchantId
            };
            return await paymentApi.ProcessPaymentAsync(paymentInfos);

        }

        public static async Task<RestPaymentStatus> MarkPaymentStatus(string input, string systemId, string merchantId, bool? isVirtualBankEnabled = null)
        {
            if (input.Contains("virtualbank") && isVirtualBankEnabled == true)
            {
                return new RestPaymentStatus
                {
                    BatchId = Convert.ToString(new Random().Next(1, 999999)),
                    Status = true
                };
            }

            return await paymentApi.ProcessPaymentResponseAsync(new RestResponseInput { Response = input, EpgMerchant = merchantId });
        }
        #endregion Payment service

        #region EDNRD
        public static async Task<string> GetEdnrdBatchId()
        {
            return await ednrdIntegrationApi.GenerateTransactionBatchIdAsync();
        }

        public static async Task<IEnumerable<Emaratech.Services.VisionIntegration.Model.RestEstablishmentProfileInfo>> GetUserEstablishments(string userName)
        {
            return await ednrdIntegrationApi.GetEstablishmentsListByUserAsync(userName);
        }

        public static List<string> GetUserEstablishmentsCode()
        {
            var userId = ClaimUtil.GetAuthenticatedUserId();
            var mappedProfile = Extensions.Mapper.MapUserProfile(
                                ApiFactory.Default
                                          .GetUserApi().
                                          GetUserAsync(userId)
                                          .Result);
            var establishmentList = ednrdIntegrationApi.GetEstablishmentsListByUserAsync(
                                                             mappedProfile.Username).Result;
            return establishmentList.Select(x => x.EstabCode).ToList();
        }
        #endregion EDNRD
        
        public static async Task<bool?> PushEmail(string argAttachment, string argContent, string argSubject, string argEmail, string argName)
        {
            var email = new RestAddress() { Email = argEmail, Name = argName };

            var attachment = new RestAttachment() { Name = argSubject, File = argAttachment };

            var emailToSend = new RestEmailProperties()
            {
                AccountId = "771640C526BA4804916329811C7129B5",
                Content = argContent,
                ContentType = "text/html; charset=utf-8",
                Subject = argSubject,
                Attachment = string.IsNullOrEmpty(argAttachment) ? null : new List<RestAttachment>() { attachment },
                To = new List<RestAddress>() { email }
            };
            return await EmailApi.SendEmailAsync(emailToSend);
        }

        public static async Task<RestReport> GetReportData(string applicationId, string userId)
        {
            return await ApplicationApi.GetReportAsync(userId, applicationId);
        }

        public static async Task<string> PutApplicationToZajelSubmitQueue(ApplicationInfo argApplicationInfo)
        {
            return await zajelApi.SendAsync(argApplicationInfo);
        }

        #region Legal advice
        public static async Task<JObject> SaveLegalAdvice(JObject legalAdvice)
        {
            return await legalAdviceApi.SaveLegalAdvice(legalAdvice);
        }

        public static async Task<IEnumerable<NationalityLookup>> GetLegalAdviceNationalities()
        {
            return await legalAdviceApi.GetNationalityLookup();
        }

        public static async Task<IEnumerable<ApplicantTypeLookup>> GetLegalAdviceApplicantTypes()
        {
            return await legalAdviceApi.GetApplicantTypeLookup();
        }

        public static async Task<IEnumerable<AdviceTypeLookup>> GetLegalAdviceAdviceTypes()
        {
            return await legalAdviceApi.GetAdviceTypeLookup();
        }

        public static async Task<JObject> FetchLegalAdvice(string adviceNumber)
        {
            return await legalAdviceApi.Fetch(adviceNumber);
        }

        public static async Task DeleteLegalAdviceDocument(string documentId)
        {
            await legalAdviceApi.DeleteDocument(documentId);
        }
        #endregion Legal advice

        #region Passport services
        public static async Task<JToken> SavePassportRenewRequest(JObject request)
        {
            return await passportServicesApi.SaveRenewRequest(request);
        }

        public static async Task DeletePassportDocument(string documentId)
        {
            await passportServicesApi.DeleteDocument(documentId);
        }

        public static async Task<JArray> GetPassportRenewDocuments()
        {
            return await passportServicesApi.GetRenewDocuments();
        }

        public static async Task<JArray> GetPassportEmiratesLookup()
        {
            return await passportServicesApi.GetEmirateLookup();
        }

        public static async Task<JToken> SavePassportNewRequest(JObject request)
        {
            return await passportServicesApi.SaveNewRequest(request);
        }

        public static async Task<JArray> GetPassportNewDocuments()
        {
            return await passportServicesApi.GetNewDocuments();
        }

        public static async Task<JArray> FetchPassportRenewRequest(string passportNo, DateTime birthDate)
        {
            return await passportServicesApi.FetchRenewRequest(passportNo, birthDate);
        }

        public static async Task<JArray> FetchPassportNewRequest(string emiratesId, string unifiedNumber)
        {
            return await passportServicesApi.FetchNewRequest(emiratesId, unifiedNumber);
        }
        #endregion Passport services

        public static async Task<RestIndividualDashboard> GetDashboardDependents(RestDashboardSearchRequest searchCriteria)
        {
            Log.Debug($"Going to perform search with criteria {JsonConvert.SerializeObject(searchCriteria)}");

            var applicationSearchResult = await IndividualDashboard.GetDependentApplicationSearchResult(searchCriteria);

            Log.Debug($"Total dependents applications found are {applicationSearchResult?.Count}");

            var individualDashboard = new RestIndividualDashboard();

            //Get visas of dependents of this user if there is no search criteria or criteria is entry permit permit or residence
            if (string.IsNullOrEmpty(searchCriteria?.ServiceType) || searchCriteria?.ServiceType == DashboardSearchType.EntryPermit.GetAttribute<DashboardSearchTypeAttr>().Type || searchCriteria?.ServiceType == DashboardSearchType.Residence.GetAttribute<DashboardSearchTypeAttr>().Type)
            {
                Log.Debug($"Going to get dependents from vision for {searchCriteria.UserId}");
                var dependentsProfiles = await VisionApi.GetIndividualDependentsFilteredInformationAsync(searchCriteria.SponsorNo, GetDependentMappedObject(searchCriteria));
                Log.Info("Dependent information found from vision " + dependentsProfiles?.IndividualDependents?.Count + " for sponsor no " + searchCriteria.SponsorNo);

                if (dependentsProfiles?.IndividualDependents == null)
                    individualDashboard.DependentsVisaInfo = new List<RestDependentVisaInfo>();
                else
                    individualDashboard.DependentsVisaInfo = await IndividualDashboard.GetDependentsVisas(searchCriteria.SponsorNo, applicationSearchResult, dependentsProfiles, searchCriteria);

                Log.Debug($"Total dependents profiles filtered are {individualDashboard.DependentsVisaInfo.Count}");
            }

            Log.Debug($"Dependents successfully filtered for {searchCriteria.UserId}");
            return individualDashboard;
        }

        private static RestIndividualDependentSearchCriteria GetDependentMappedObject(RestDashboardSearchRequest searchCriteria)
        {
            return new RestIndividualDependentSearchCriteria
            {
                Criteria = searchCriteria.Criteria,
                ServiceType = searchCriteria.ServiceType
            };
        }

        public static async Task<string> GenerateToken(string systemId, IDictionary<string, string> payload)
        {
            return await TokenApi.IssueTokenAsync(new RestTokenPayload()
            {
                Audience = "http://eap",
                Issuer = "http://eap",
                KeyName = await SystemProperties.Property(systemId, Constants.KeyVaultToken),
                Payload = payload.Select(x => new RestPayloadValue(x.Key, x.Value)).ToList(),
                SystemId = systemId
            });
        }

        #region RSA_Establishment

        public static async Task<string> GetRSADeviceId(string username)
        {
            return await ednrdIntegrationApi.GetRsaDeviceIdAsync(username);
        }

        #endregion
    }
}