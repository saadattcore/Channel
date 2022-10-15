using Emaratech.Services.WcfCommons.Dynamic;
using Emaratech.Services.WcfCommons.Rest;
using Ninject;
using Ninject.Web.Common;
using SwaggerWcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Emaratech.Services.WcfCommons.Rest.Modules;
using Ninject.Modules;

namespace Emaratech.Services.Channels
{
    public partial class Global : NinjectHttpApplication
    {
        /// <summary>
        /// Creates the kernel that will manage application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        protected override IKernel CreateKernel()
        {
            var modules = new INinjectModule[]
            {
                new ServiceNinjectModule(),
                new LoggingInitializationNinjectModule(),
                new AutoMapperInitializationNinjectModule(Assembly.GetExecutingAssembly()),
                new SwaggerInitializationNinjectModule()
            };
            return new StandardKernel(modules);

        }

        protected override void OnApplicationStarted()
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate {
                return true;
            };
            base.OnApplicationStarted();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie userCookie = Request.Cookies["USER_ID"];
            if (Request.Cookies["SSO_ID"] != null && userCookie != null)
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userCookie.Value)
                };
                Context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationTypes.Password));
            }
        }
    }
}