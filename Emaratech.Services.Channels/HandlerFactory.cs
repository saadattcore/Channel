using System.Web;
using Ninject;

namespace Emaratech.Services.Channels
{
    class HandlerFactory : IHttpHandlerFactory
    {

        readonly StandardKernel kernel = new StandardKernel(new ServiceNinjectModule());

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var handler = kernel.Get<IHttpHandler>();
            return handler;
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}