using Emaratech.Services.Application.Api;
using Emaratech.Services.Document.Api;
using Emaratech.Services.Lookups.Api;
using Emaratech.Services.SMS.Api;
using Emaratech.Services.Systems.Api;
using Emaratech.Services.UserManagement.Api;
using System.Configuration;
using Emaratech.Services.MappingMatrix.Api;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.Localization.Api;
using Emaratech.Services.VisionIntegration.Api;
using Emaratech.Services.Payment.Api;

namespace Emaratech.Services.Channels
{
    public class ApiFactory 
    {
        public static ApiFactory Default => new ApiFactory();

        public ISystemApi GetSystemApi()
        {
            return new DefaultServiceFactory().GetSystemApi();
        }


        public ISMSApi GetSmsApi()
        {
            return new DefaultServiceFactory().GetSmsApi();
        }

        public IUserApi GetUserApi()
        {
            return new DefaultServiceFactory().GetUserApi();
        }

        public ILookupApi GetLookupApi()
        {
            return  new DefaultServiceFactory().GetLookupApi();
        }
        public IApplicationSearchApi GetApplicationSearchApi()
        {
            return new DefaultServiceFactory().GetApplicationSearchApi();
        }

        public IApplicationApi GetApplicationApi()
        {
            return new DefaultServiceFactory().GetApplicationApi();
        }

        public IApplicationStatusApi GetApplicationStatusApi()
        {
            return new DefaultServiceFactory().GetApplicationStatusApi();
        }

        public IDocumentApi GetDocumentApi()
        {
            return new DefaultServiceFactory().GetDocumentApi();
        }       

        public IMappingMatrixApi GetMappingMatrixApi()
        {
            return new DefaultServiceFactory().GetMappingMatrixApi();
        }

        public IVisionIndividualApi GetVisionIndividualApi()
        {
            return new DefaultServiceFactory().GetVisionApi();
        }

        public IVisionEstablishmentApi GetVisionEstablishmentApi()
        {
            return new DefaultServiceFactory().GetVisionEstablishmentApi();
        }

        public ILocalizationApi GetLocalizationApi()
        {
            return new DefaultServiceFactory().GetLocalizationApi();
        }

        public IEDNRDIntegrationApi GetEdnrdIntegrationApi()
        {
            return new DefaultServiceFactory().GetEdnrdIntegrationApi();
        }

        public IPaymentsApi GetPaymentApi()
        {
            return new DefaultServiceFactory().GetPaymentApi();
        }
    }
}