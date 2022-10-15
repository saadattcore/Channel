using Emaratech.Services.Application.Api;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.DataContracts.Api;
using Emaratech.Services.Document.Api;
using Emaratech.Services.Email.Api;
using Emaratech.Services.Fee.Api;
using Emaratech.Services.Forms.Api;
using Emaratech.Services.Localization.Api;
using Emaratech.Services.Lookups.Api;
using Emaratech.Services.MappingMatrix.Api;
using Emaratech.Services.Payment.Api;
using Emaratech.Services.Security.KeyVault.Api;
using Emaratech.Services.Services.Api;
using Emaratech.Services.SMS.Api;
using Emaratech.Services.Systems.Api;
using Emaratech.Services.UserManagement.Api;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.VisionIntegration.Api;
using Emaratech.Services.Workflows.Api;
using Emaratech.Services.Zajel.Api;
using ICRUDApi = Emaratech.Services.Template.Api.ICRUDApi;

namespace Emaratech.Services.Channels
{
    public interface IServiceFactory
    {
        ISystemApi GetSystemApi();
        IMappingMatrixApi GetMappingMatrixApi();
        IDocumentApi GetDocumentApi();
        IDataContractApi GetDataContractApi();
        IVisionIndividualApi GetVisionApi();
        IVisionEstablishmentApi GetVisionEstablishmentApi();
        IVisionCommonApi GetVisionCommonApi();
        IApplicationApi GetApplicationApi();
        IApplicationStatusApi GetApplicationStatusApi();
        IApplicationSearchApi GetApplicationSearchApi();
        ILookupApi GetLookupApi();
        IFeeApi GetFeeApi();
        IPaymentsApi GetPaymentApi();
        IFormsApi GetFormsApi();
        ILayoutsApi GetFormsLayoutApi();
        IZajelApi GetZajelApi();
        Forms.Api.ILookupsApi GetFormsLookupsApi();
        IFieldSetsApi GetFormsFieldSetsApi();
        ILocalizationApi GetLocalizationApi();
        IEDNRDIntegrationApi GetEdnrdIntegrationApi();
        IEmailApi GetEmailApi();
        ILegalAdviceApi GetLegalAdviceApi();
        IPassportServicesApi GetPassportServicesApi();
        ITokensApi GetTokensApi();
        IServiceApi GetServiceApi();
        IWorkflowsApi GetWorkflowsApi();
        ISMSApi GetSmsApi();
        IUserApi GetUserApi();
        ICRUDApi GetTemplateApi();
    }
}