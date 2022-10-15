using Emaratech.Services.Ado;
using Emaratech.Services.Ado.Oracle;
using Emaratech.Services.WcfCommons.Client;
using log4net.Config;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emaratech.Services.Channels.DataAccess
{
    public class ServiceNinjectModule : NinjectModule
    {
        public override void Load()
        {
            XmlConfigurator.Configure();

            OracleDbTypeMapper.MapStringToNVarchar2();

            var repDbProvider = new OracleDbProvider("RepDB");
            this.Bind<IDbProvider>().ToConstant(repDbProvider).InSingletonScope();
        }
    }
}