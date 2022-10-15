namespace Emaratech.Services.Channels.Workflows.Models
{
    internal class VisaApplication
    {
        public string Service { get; set; }
        public string VisaType { get; set; }
        public string AppType { get; set; }
        public string AppSubType { get; set; }

        public VisaApplication(string service, string visaType, string appType, string appSubType)
        {
            Service = service;
            VisaType = visaType;
            AppType = appType;
            AppSubType = appSubType;
        }
    }
}