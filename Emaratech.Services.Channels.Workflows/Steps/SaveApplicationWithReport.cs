using Emaratech.Services.Application.Model;
using Emaratech.Services.Workflows.Engine;
using Newtonsoft.Json;

namespace Emaratech.Services.Channels.Workflows.Steps
{
    public class SaveApplicationWithReport : SaveApplicationBase
    {
        public InputParameter<string> ReportData { get; set; }

        protected override RestApplication CreateServiceApplication()
        {
            var app = base.CreateServiceApplication();

            // Add data required for this step.
            //app.ReportData = new RestReportData {Data = ReportData.Get()};
            return app;
        }
    };
}