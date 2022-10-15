using System.Configuration;
using Emaratech.Services.Application.Api;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.Channels.Services;
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

namespace Emaratech.Services.Channels
{
    public class DefaultServiceFactory : IServiceFactory
    {
        public virtual ISystemApi GetSystemApi()
        {
            return new SystemApi(ConfigurationManager.AppSettings["SystemApi"]);
        }

        public IMappingMatrixApi GetMappingMatrixApi()
        {
            return new MappingMatrixApi(ConfigurationManager.AppSettings["MappingMatrixApi"]);
        }

        public IDocumentApi GetDocumentApi()
        {
            return new DocumentApi(ConfigurationManager.AppSettings["DocumentApi"]);
        }

        public IDataContractApi GetDataContractApi()
        {
            return new DataContractApi(ConfigurationManager.AppSettings["DataContractsApi"]);
        }

        public IVisionIndividualApi GetVisionApi()
        {
            return new VisionIndividualApi(ConfigurationManager.AppSettings["VisionApi"]);
        }

        public IVisionEstablishmentApi GetVisionEstablishmentApi()
        {
            return new VisionEstablishmentApi(ConfigurationManager.AppSettings["VisionApi"]);
        }

        public IVisionCommonApi GetVisionCommonApi()
        {
           return new VisionCommonApi(ConfigurationManager.AppSettings["VisionApi"]);
        }

        public IApplicationApi GetApplicationApi()
        {
            return new ApplicationApi(ConfigurationManager.AppSettings["ApplicationApi"]);
        }

        public ILookupApi GetLookupApi()
        {
            return new LookupApi(ConfigurationManager.AppSettings["LookupsApi"]);
        }

        public IFeeApi GetFeeApi()
        {
            return new FeeApi(ConfigurationManager.AppSettings["FeeApi"]);
        }

        public IPaymentsApi GetPaymentApi()
        {
            return new PaymentsApi(ConfigurationManager.AppSettings["PaymentApi"]);
        }

        public IFormsApi GetFormsApi()
        {
            return new FormsApi(ConfigurationManager.AppSettings["FormsApi"]);
        }

        public ILayoutsApi GetFormsLayoutApi()
        {
            return new LayoutsApi(ConfigurationManager.AppSettings["FormsApi"]);
        }

        public Forms.Api.ILookupsApi GetFormsLookupsApi()
        {
            return new Forms.Api.LookupsApi(ConfigurationManager.AppSettings["FormsApi"]);
        }

        public IFieldSetsApi GetFormsFieldSetsApi()
        {
            return new FieldSetsApi(ConfigurationManager.AppSettings["FormsApi"]);
        }

        public ILocalizationApi GetLocalizationApi()
        {
            return new LocalizationApi(ConfigurationManager.AppSettings["LocalizationApi"]);
        }

        public IEDNRDIntegrationApi GetEdnrdIntegrationApi()
        {
            return new EDNRDIntegrationApi(ConfigurationManager.AppSettings["VisionIntegration"]);
        }

        public IApplicationStatusApi GetApplicationStatusApi()
        {
            return new ApplicationStatusApi(ConfigurationManager.AppSettings["ApplicationApi"]);
        }

        public IApplicationSearchApi GetApplicationSearchApi()
        {
            return new ApplicationSearchApi(ConfigurationManager.AppSettings["ApplicationApi"]);
        }

        public IZajelApi GetZajelApi()
        {
            return new ZajelApi(ConfigurationManager.AppSettings["ZajelApi"]);
        }
        
        public IEmailApi GetEmailApi()
        {
            return new EmailApi(ConfigurationManager.AppSettings["EmailApi"]);
        }

        public ILegalAdviceApi GetLegalAdviceApi()
        {
            return new LegalAdviceApi(ConfigurationManager.AppSettings["LegalAdviceApi"]);
        }

        public IPassportServicesApi GetPassportServicesApi()
        {
            return new PassportServicesApi(ConfigurationManager.AppSettings["PassportServicesApi"]);
        }

        public ITokensApi GetTokensApi()
        {
            return new TokensApi(ConfigurationManager.AppSettings["TokensApi"]);
        }

        public IServiceApi GetServiceApi()
        {
           return new ServiceApi(ConfigurationManager.AppSettings["ServiceApi"]);
        }

        public IWorkflowsApi GetWorkflowsApi()
        {
            return new WorkflowsApi(ConfigurationManager.AppSettings["WorkflowApi"]);
        }

        public ISMSApi GetSmsApi()
        {
           return new SMSApi(ConfigurationManager.AppSettings["SMSApi"]);
        }

        public IUserApi GetUserApi()
        {
            return new UserApi(ConfigurationManager.AppSettings["UserApi"]);
        }

        public Template.Api.ICRUDApi GetTemplateApi()
        {
            return new Template.Api.CRUDApi(ConfigurationManager.AppSettings["TemplateApi"]);
        }
    }
}