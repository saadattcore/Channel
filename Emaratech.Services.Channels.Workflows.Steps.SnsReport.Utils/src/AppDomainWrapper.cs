using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Emaratech.Services.Channels.Reports.Models.Sns;

namespace Emaratech.Services.Channels.Workflows.Steps.SnsReport.Utils
{
    public class AppDomainWrapper
    {
        private static readonly Lazy<AppDomainWrapper> lazy = new Lazy<AppDomainWrapper>(() => new AppDomainWrapper());
        public static AppDomainWrapper Instance => lazy.Value;
        private AppDomain ad;

        private AppDomainWrapper()
        {
            ConstructApplicationDomain("EmailTemplateAppDomain");
        }

        public static AppDomainWrapper Create() => new AppDomainWrapper();

        private static void CbAppDomainInitializer(string[] args)
        {
            Console.WriteLine($"Domain {AppDomain.CurrentDomain.FriendlyName} is initialized.");
        }

        private void ConstructApplicationDomain(string argDomainName)
        {
            var strongNames = new StrongName[0];
            //var thisAssembly = Assembly.GetAssembly(typeof(AppDomainWrapper)).Location;
            //var assemblyFolder = Path.GetDirectoryName(thisAssembly);

            //var executingAssembly = Assembly.GetExecutingAssembly().Location;
            //var appFolder = Path.GetDirectoryName(executingAssembly);

            //var appDomainFolder = AppDomain.CurrentDomain.BaseDirectory;

            AppDomainSetup ads = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory + @"\bin",
                AppDomainInitializer = CbAppDomainInitializer,
                DisallowBindingRedirects = false,
                DisallowCodeDownload = true,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };


            ad = AppDomain.CreateDomain(argDomainName, null, ads, new PermissionSet(PermissionState.Unrestricted), strongNames);
            // ad.UnhandledException += AdOnUnhandledException;
            // ad.AssemblyLoad += AdOnAssemblyLoad;
            // ad.DomainUnload += AdOnDomainUnload;
            // ad.ProcessExit += AdOnProcessExit;
        }

        public void UnloadApplicationDomain()
        {
            AppDomain.Unload(ad);
        }

        public string ParseFromTemplate(Root argReportModelRoot)
        {
            var scls = (ServerCls)ad.CreateInstanceAndUnwrap(this.GetType().Assembly.FullName, "Emaratech.Services.Channels.Workflows.Steps.SnsReport.Utils.ServerCls");
            return scls.ParseFromTemplate(argReportModelRoot);
        }

    }
}