using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Extensions;
using Emaratech.Services.Channels.Services.Reports.Pdf;
using Emaratech.Services.Email.Model;
using Emaratech.Services.Security.KeyVault.Model;
using Emaratech.Services.Template.Model;
using Emaratech.Utilities;

namespace Emaratech.Services.Channels.Services.Reports
{
    public class SendReportImpl : ReportServiceImplBase
    {

        class EmailData
        {
            public string ApplicationId { get; set; }
            public string ReportLink { get; set; }
            public IDictionary<string,object> Parameters { get; set; } 

        }

        public async Task<bool?> Send(string emailAddress,string applicationId,string userId, string systemId, RestReport rawReport)
        {
            var reportGeneratorPdf = new GeneratorPdf();
            string emailSubject = string.Empty;
            string emailTemplate = string.Empty;
            RestAddress emailDestination = null;

            if (rawReport.ServiceId.Equals(ReportConstants.SasReportServiceId))
            {
                emailDestination = new RestAddress { Email = emailAddress, Name = emailAddress };

                emailSubject = ReportConstants.EmailSubjectSasReport;
                emailTemplate = ReportConstants.SasReportEmailTemplate;

            }
            else if (rawReport.ServiceId.Equals(ReportConstants.EstablishmentSasReportServiceId))
            {

                emailDestination = new RestAddress { Email = emailAddress, Name = emailAddress };

                emailSubject = ReportConstants.EmailSubjectSasReport;
                emailTemplate = ReportConstants.SasReportEmailTemplate;

            }
            else if (rawReport.ServiceId.Equals(ReportConstants.TravelReportServiceId))
            {
                short fileType = short.Parse(rawReport.FileType);

                emailDestination = new RestAddress() { Email = emailAddress, Name = emailAddress };

                emailSubject = ReportConstants.EmailSubjectTravelReport;
                emailTemplate = ReportConstants.TravelReportEmailTemplate;

            }


            var tokenApi = new DefaultServiceFactory().GetTokensApi();
            var token = await PayloadBuilder.New().Key(systemId, TokenUtils.TokenKey(systemId))
                .Expiry(TimeSpan.FromDays(365))
                .Add("ApplicationId", applicationId)
                .Add("UserId", userId)
                .Issue(tokenApi);


            var link = $"{ChannelService.ApiHost}/v1/UChannel/reports?token={token}";
            var templateApi = new DefaultServiceFactory().GetTemplateApi();
            var data = new EmailData()
            {
                ApplicationId = applicationId,
                ReportLink = link,
                Parameters = new ConcurrentDictionary<string, object>()

            };

            var host = ConfigurationManager.AppSettings["WebHost"];
            data.Parameters.Add("Host", host);

            var content = await templateApi.RenderTemplateAsync(emailTemplate, new Context(data.ToJsonString()));

            return await SendEmail(emailSubject, string.Empty, content, emailDestination);

        }

        public async Task<bool?> CreateAndSend( string emailAddress, string applicationId, string userId,string systemId)
        {
            var rawReport = FetchRawReport(applicationId, userId);
            return await Send(emailAddress, applicationId, userId, systemId, rawReport);
        }

      
        private async Task<bool?> SendEmail(string emailSubject,string emailAttachment,string emailContent, RestAddress emailDestination)
        {

            var attachment = new RestAttachment() { Name = emailSubject, File = emailAttachment };
            var emailToSend = new RestEmailProperties()
            {
                AccountId = ReportConstants.EmailAccountId,
                Content = emailContent,
                ContentType = ReportConstants.EmailContentType,
                Subject = emailSubject,
                Attachment = string.IsNullOrEmpty(emailAttachment) ? null : new List<RestAttachment>() { attachment },
                To = new List<RestAddress>() { emailDestination }
            };

            return await new DefaultServiceFactory().GetEmailApi().SendEmailAsync(emailToSend);
        }
    }
}