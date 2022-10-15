namespace Emaratech.Services.Channels.BusinessLogic.ApplicationReports
{ 
    public class ReportRecordInfo
    {
        public string Id { get; set; }
        public string FileType { get; set; }
        public string ServiceId { get; set; }
        public string UserId { get; set; }
        public string ReportPath { get; set; }
        public string ReportData { get; set; }
        public string UserPhoto { get; set; }
        public bool IsRequiredDateParameter { get; set; }
        public bool IsRequiredPhotoParameter { get; set; }
        public bool HasInstructions { get; set; }
    }
}
