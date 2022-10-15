using System;
using System.IO;
using System.Reflection;
using Emaratech.Services.Channels.Reports.Models.Sns;
using RazorEngine.Templating;

namespace Emaratech.Services.Channels.Workflows.Steps.SnsReport.Utils
{
    public class ServerCls : MarshalByRefObject
    {
        private ServerCls() { }

        public static ServerCls Create() => new ServerCls();

        public string ParseFromTemplate(Root argReportModelRoot)
        {
            string emailHtmlBody;

            using (var service = RazorEngineService.Create())
            {
                var typeLocation = this.GetType().Assembly.Location;
                var templateFolderPath = Path.Combine(Path.GetDirectoryName(typeLocation), Constants.EmailTemplatesFolder, Constants.EmailTemplateFile);
                emailHtmlBody = service.RunCompile(File.ReadAllText(templateFolderPath), Constants.TemplateCacheKey, typeof(Root), argReportModelRoot);
            }
            return emailHtmlBody;
        }

        public string ParseFromTemplate(Root argReportModelRoot, string argTemplateLocation)
        {
            string emailHtmlBody;

            using (var service = RazorEngineService.Create())
            {
                emailHtmlBody = service.RunCompile(File.ReadAllText(argTemplateLocation), Constants.TemplateCacheKey, typeof(Root), argReportModelRoot);
            }
            return emailHtmlBody;
        }
    }

}