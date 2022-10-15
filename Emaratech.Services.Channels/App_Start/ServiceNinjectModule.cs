using Emaratech.Services.Channels.Contracts.DataAccess;
using Emaratech.Services.WcfCommons.Client;
using log4net.Config;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Web.Common;
using Emaratech.Services.Systems.Api;
using System.Configuration;
using Emaratech.Services.MappingMatrix.Api;
using Emaratech.Services.Services.Api;
using Emaratech.Services.Workflows.Api;
using Emaratech.Services.Vision.Api;
using Emaratech.Services.UserManagement.Api;
using Emaratech.Services.Security.KeyVault.Api;
//using Emaratech.Services.Registry;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.SMS.Api;
using Emaratech.Services.Document.Api;
using AutoMapper;
using Emaratech.Services.Channels.Helpers;
using Emaratech.Services.Email.Api;
using Emaratech.Services.Payment.Api;
using Emaratech.Services.Channels.Services;
using Emaratech.Services.Systems.Properties;
using Emaratech.Services.Common.Caching;
using Emaratech.Services.Channels.Contracts.Authorization;
using Emaratech.Services.Channels.Authorization;
using Emaratech.Services.Channels.BusinessLogic.ApplicationReports;

namespace Emaratech.Services.Channels
{
    public class ServiceNinjectModule : NinjectModule
    {

        public override void Load()
        {
            this.Bind<ISystemSettings>()
                .ToConstructor(
                    x => new SystemSettings(ConfigurationManager.AppSettings["SystemId"], x.Inject<IServiceFactory>()))
                    .InSingletonScope();
            this.Bind<IUserStore>()
                .To<UserStore>()
                .InRequestScope();
            this.Bind<IHttpHandler>()
                .To<Handler>()
                .InRequestScope();

            var serviceFactory = new DefaultServiceFactory();
            this.Bind<IServiceFactory>()
                .ToConstant(serviceFactory)
                .InSingletonScope();

            this.Bind<ConfigurationRepository>().ToSelf().InSingletonScope();

            this.Bind<IAuthorizationManager>()
                .To<AuthorizationManager>()
                .InRequestScope();

            ServicesHelper.Init(serviceFactory,
             new SystemProperties(Cache.CacheTimeInMinutes));

        }
    }
}