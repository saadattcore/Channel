using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Application.Api;
using Emaratech.Services.Application.Model;
using Emaratech.Services.Channels;
using Emaratech.Services.Channels.Contracts.Rest.Models.Enums;
using Emaratech.Services.Channels.Contracts.Rest.Models.Reports;
using Emaratech.Services.Channels.Extensions;
using Emaratech.Services.Channels.Services.Reports;
using Emaratech.Services.Channels.Services.Reports.Excel;
using Emaratech.Services.Channels.Services.Reports.Pdf;
using Emaratech.Services.Channels.Workflows;
using Emaratech.Services.Channels.Workflows.Steps;
using Emaratech.Services.Channels.Workflows.Steps.Processing;
using Emaratech.Services.Channels.Workflows.Steps.Report;
using Emaratech.Services.Channels.Workflows.Steps.Report.Processing;
using Emaratech.Services.Common.Configuration;
using Emaratech.Services.Email.Api;
using Emaratech.Services.Email.Model;
using Emaratech.Services.Lookups.Api;
using Emaratech.Services.Security.KeyVault.Api;
using Emaratech.Services.Systems.Api;
using Emaratech.Services.Systems.Properties;
using Emaratech.Services.Vision.Api;
using Emaratech.Utilities;
using log4net;
using log4net.Config;
using Microsoft.Win32;

namespace Emaratech.Services.Reports.GeneratorJob
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private readonly string EstablishmentSASReportServiceId = ConfigurationSystem.AppSettings[nameof(EstablishmentSASReportServiceId)];
        private readonly string IndividualSASReportServiceId = ConfigurationSystem.AppSettings[nameof(IndividualSASReportServiceId)];
        private readonly string TravelReportServiceId = ConfigurationSystem.AppSettings[nameof(TravelReportServiceId)];
        private readonly string PaidStatusId = ConfigurationSystem.AppSettings[nameof(PaidStatusId)];
        private readonly string ReadyStatusId = ConfigurationSystem.AppSettings[nameof(ReadyStatusId)];

        private readonly string ReportFilePath = ConfigurationSystem.AppSettings[nameof(ReportFilePath)]
            ;

        private readonly string ReportsLocation = ConfigurationSystem.AppSettings[nameof(ReportsLocation)];
        private readonly List<string> serviceIds;

        public Program()
        {
            serviceIds = new List<string> { EstablishmentSASReportServiceId, IndividualSASReportServiceId, TravelReportServiceId };
        }

        private ReportState RunInd(string sponsorNo)
        {
           var  repState  = new ReportState();
            ProcessingStep startStep = new BuildSponsorInfoReportStep(sponsorNo, repState.SetSponsorInfo,
                new GetDependentsStep(sponsorNo, repState.SetDependentsInfo,
                    new CreateResidenceDetailsListStep(repState.GetDependentsInfo, repState.SetDependentsResidenceDetails,
                        new CreateEntryDetailsListStep(repState.GetDependentsInfo, repState.SetDependentsEntryPermitDetails,
                            new SetReportContentStep(repState.GetSponsorInfo, repState.GetDependentsResidenceDetails, repState.GetDependentsEntryPermitDetails, repState.SetJsonReport,
                                new SetEmailContentStep(repState.GetSponsorInfo, repState.GetDependentsResidenceDetails, repState.GetDependentsEntryPermitDetails, repState.SetHtmlReport,
                                    new NullStep()))))));
            startStep.Process().Wait();
            return repState;
        }


        private ReportState RunEst(string estCode)
        {
            var repState = new ReportState();

            ProcessingStep startStep = new BuildEstablishmentInfoReportStep(estCode, repState.SetSponsorInfo,
                new GetEstablishmentDependentStep(estCode, repState.SetDependentsInfo,
                    new CreateResidenceDetailsListStep(repState.GetDependentsInfo, repState.SetDependentsResidenceDetails,
                        new CreateEntryDetailsListStep(repState.GetDependentsInfo, repState.SetDependentsEntryPermitDetails,
                            new SetReportContentStep(repState.GetSponsorInfo, repState.GetDependentsResidenceDetails, repState.GetDependentsEntryPermitDetails, repState.SetJsonReport,
                                new SetEmailContentStep(repState.GetSponsorInfo, repState.GetDependentsResidenceDetails, repState.GetDependentsEntryPermitDetails, repState.SetHtmlReport,
                                    new NullStep()))))));
            startStep.Process().Wait();
            return repState;
        }

        private ReportState RunTravel(string visaNumber, string fileType)
        {
            var travelReport = new TravelReport();
            var serviceType = (ServiceType)Convert.ToInt32(fileType);
            string json;
            if (serviceType == ServiceType.Residence)
            {
                json = travelReport.Generate(visaNumber, null).Result;

            }
            else
            {
                json = travelReport.Generate(null,visaNumber).Result;
            }
            var reportState = new ReportState();
            reportState.SetJsonReport(json);
            return reportState;

        }

        private static List<RestApplicationSearchRow> GetStatus(string approvedStatus, List<string> serviceIds = null)
        {
            var criteria = new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                    {
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ReportDetails",
                            PropertyName = "EstCode"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ReportDetails",
                            PropertyName = "VisaNumber "
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ReportDetails",
                            PropertyName = "FileType"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "SponsorDetails",
                            PropertyName = "ApplicationId"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "SponsorDetails",
                            PropertyName = "SponsorNo"
                        },
                          new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "ServiceId"
                        }



                    },
                StatusId = approvedStatus, //ApplicationStatus.Approved.GetHashCode().ToString(),
                SelectedFlag = 2,
                SelectedFlagValue = 0,
                ServiceIds = serviceIds
            };

            var applicationSearchResult =
                (ServicesHelper.ApplicationSearchApi.GetApplicationsByCriteria(criteria)).RestApplicationSearchRow.ToList();

            string servicesId = serviceIds != null ? string.Join(",", serviceIds) : string.Empty;
            Log.Debug($"Paid Application Status Ids === { approvedStatus} and services id ==== {servicesId}")
            ;
            // Log.Debug($"Approved Application for eVisa :  ====  {JsonConvert.SerializeObject(applicationSearchResult)}");
            return applicationSearchResult;
        }


        void SendEmail(RestApplicationSearchRow application)
        {
            try
            {
                var applicationId = application.RestApplicationSearchKeyValues.Single(x =>  x.PropertyName == "APPLICATIONID").Value;
                var systemId = application.RestApplicationSearchKeyValues.Single(x => x.PropertyName == "SYSTEMID").Value;
                var userId = application.RestApplicationSearchKeyValues.Single(x =>  x.PropertyName == "USERID").Value;
                var email = application.RestApplicationSearchKeyValues.Single(x =>  x.PropertyName == "EMAIL").Value;

                Log.Debug($"Send email application id {applicationId}");

                var reportData = application.RestApplicationSearchKeyValues.Single(x => x.PropertyName == "REPORTDATA").Value;
                if (string.IsNullOrEmpty(reportData))
                    throw new Exception($"Report data is empty for {applicationId}");

                var serviceId = application.RestApplicationSearchKeyValues.Single(x => x.PropertyName == "SERVICEID").Value;

                var sent = Send(email, applicationId,  systemId,userId, reportData,serviceId);

                if (sent.HasValue && sent.Value)
                {
                    ServicesHelper.ApplicationApi.UpdateApplicationField(applicationId,
                        new RestApplicationSearchKeyValues()
                        {
                            PropertyName = "Flag3",
                            Entity = "ApplicationDetails",
                            Value = "1"
                        });


                    Log.Debug($"Send email application id {applicationId}")
                    ;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            
        }
        public void SendEmails()
        {

            var criteria = new RestApplicationSearchCriteria
            {
                SelectColumns = new List<RestApplicationSearchKeyValues>
                    {
                     new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "ApplicationId"
                        },
                    new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "UserId"
                        },
                     new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "SystemId"
                        },
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ReportDetails",
                            PropertyName = "Email"
                        }
                        ,
                          new RestApplicationSearchKeyValues
                        {
                            Entity = "ApplicationDetails",
                            PropertyName = "ServiceId"
                        },
                          new RestApplicationSearchKeyValues
                        {
                            Entity = "ReportDetails",
                            PropertyName = "ReportData"
                        }

                    },
                IsPaid = "true",
                IsBatchIdMarked = "true",
                SelectedFlag = 3,
                StatusId = PaidStatusId,
                SelectedFlagValue = 0,
                ServiceIds = serviceIds
            };
            var applications =
    (ServicesHelper.ApplicationSearchApi.GetApplicationsByCriteria(criteria)).RestApplicationSearchRow.ToList();

            Log.Debug($"{applications.Count} applications found for email");
            var tasks = applications.Select(application => Task.Run(() => SendEmail(application)));
            Task.WaitAll(tasks.ToArray());


        }
        void ProcessApplication(RestApplicationSearchRow application)
        {
            try
            {
                var applicationId = application.RestApplicationSearchKeyValues.Single(x => x.Entity == "SponsorDetails" && x.PropertyName == "APPLICATIONID").Value;

                Log.Debug($"Processing application id {applicationId}");
                ReportState state = new ReportState();
                var serviceId =
                    application.RestApplicationSearchKeyValues.Single(x => x.PropertyName == "SERVICEID").Value;
                if (serviceId == TravelReportServiceId)
                {
                    var visaNumber = application.RestApplicationSearchKeyValues.Single(x => x.Entity == "ReportDetails" && x.PropertyName == "VISANUMBER").Value;
                    var fileType = application.RestApplicationSearchKeyValues.Single(x => x.Entity == "ReportDetails" && x.PropertyName == "FILETYPE").Value;
                    state = RunTravel(visaNumber, fileType);

                }
                else if (serviceId == EstablishmentSASReportServiceId)
                {
                    var estCode = application.RestApplicationSearchKeyValues.Single(x => x.Entity == "ReportDetails" &&  x.PropertyName == "ESTCODE").Value;
                    if (!string.IsNullOrEmpty(estCode))
                    {
                        state = RunEst(estCode);
                    }
                    else
                    {
                        Log.Debug($"Establishment code  not found for  application id {applicationId}");

                    }
                }
                else
                {
                    var visaNo = application.RestApplicationSearchKeyValues.Single(x => x.Entity == "SponsorDetails" && x.PropertyName == "SPONSORNO").Value;
                    if (!string.IsNullOrEmpty(visaNo))
                    {
                        state = RunInd(visaNo);
                    }
                    else
                    {
                        Log.Debug($"Visa No not found for  application id {applicationId}");

                    }
                }
                if (state.JsonReport != null)
                {
                    Log.Debug($"Size before compression {state.JsonReport.Length}");
                    var memory = new MemoryStream();
                    GZipStream stream = new GZipStream(memory,CompressionMode.Compress, true);
                    byte[] data = Encoding.UTF8.GetBytes(state.JsonReport);
                    stream.Write(data,0,data.Length);
                    stream.Close();
                    memory.Seek(0, SeekOrigin.Begin);
                    state.JsonReport = Convert.ToBase64String(memory.GetBuffer());
                    Log.Debug($"Size after compression {state.JsonReport.Length}");


                    Log.Debug($"Saving report data {applicationId}");

                    ServicesHelper.ApplicationApi.UpdateApplicationField(applicationId,
                        new RestApplicationSearchKeyValues
                        {
                            Entity = "ReportDetails",
                            PropertyName = "ReportData",
                            Value = state.JsonReport
                        });

                    Log.Debug($"Setting report flag2 {applicationId}");
                    ServicesHelper.ApplicationApi.UpdateApplicationField(applicationId,
                        new RestApplicationSearchKeyValues()
                        {
                            PropertyName = "Flag2",
                            Entity = "ApplicationDetails",
                            Value = "1"
                        });


                    Log.Debug($"Processed application id {applicationId}");
                }



            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void GenerateReports(string serviceId,ReportState state, RestApplicationSearchRow application,string applicationId)
        {
            if (serviceId == EstablishmentSASReportServiceId)
            {
                var location = Path.Combine(ReportsLocation, ConfigurationSystem.AppSettings["EstabPdfReport"]);
                var generator = (ReportGenerator)EstablishmentSasReportPdfInterface.Create();
                var stream = generator.BuilReport(state.JsonReport, location, ConfigurationSystem.AppSettings["ReportApiPdf"]).Result;
                using (var file = new FileStream(Path.Combine(ReportFilePath, $"{applicationId}.pdf"), FileMode.OpenOrCreate))
                {
                    stream.CopyTo(file);
                }

                location = Path.Combine(ReportsLocation, ConfigurationSystem.AppSettings["EstabXslReport"]);
                stream = generator.BuilReport(state.JsonReport, location, ConfigurationSystem.AppSettings["ReportApiExcel"]).Result;
                using (var file = new FileStream(Path.Combine(ReportFilePath, $"{applicationId}.xls"), FileMode.OpenOrCreate))
                {
                    stream.CopyTo(file);
                }
            }
            else
            {
                var location = Path.Combine(ReportsLocation, ConfigurationSystem.AppSettings["IndPdfReport"]);
                var generator = (ReportGenerator)SasReportPdfInteface.Create();
                var stream = generator.BuilReport(state.JsonReport, location, ConfigurationSystem.AppSettings["ReportApiPdf"]).Result;
                using (var file = new FileStream(Path.Combine(ReportFilePath, $"{applicationId}.pdf"), FileMode.OpenOrCreate))
                {
                    stream.CopyTo(file);
                }

                location = Path.Combine(ReportsLocation, ConfigurationSystem.AppSettings["IndXslReport"]);
                stream = generator.BuilReport(state.JsonReport, location, ConfigurationSystem.AppSettings["ReportApiExcel"]).Result;
                using (var file = new FileStream(Path.Combine(ReportFilePath, $"{applicationId}.xls"), FileMode.OpenOrCreate))
                {
                    stream.CopyTo(file);
                }

            }
        }
        public void Process()
        {
            var applications = GetStatus(ReadyStatusId, serviceIds);
            Log.Debug($"{applications.Count} applications found");
            var tasks = applications.Select(application => Task.Run(() => ProcessApplication(application)));
            Task.WaitAll(tasks.ToArray());
        }

        private  bool? SendEmail(string emailSubject, string emailAttachment, string emailContent, RestAddress emailDestination)
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

            return  ServicesHelper.EmailApi.SendEmail(emailToSend);
        }

        public bool? Send(string emailAddress, string applicationId, string systemId, string userId, string rawReport,string serviceId)
        {
            var reportGeneratorPdf = new GeneratorPdf();
            string emailSubject = string.Empty;
            string emailTemplate = string.Empty;
            RestAddress emailDestination = null;

            if (serviceId.Equals(ReportConstants.SasReportServiceId))
            {
                emailDestination = new RestAddress { Email = emailAddress, Name = emailAddress };

                emailSubject = ReportConstants.EmailSubjectSasReport;
                emailTemplate = ReportConstants.SasReportEmailTemplate;

            }
            else if (serviceId.Equals(ReportConstants.EstablishmentSasReportServiceId))
            {

                emailDestination = new RestAddress { Email = emailAddress, Name = emailAddress };

                emailSubject = ReportConstants.EmailSubjectSasReport;
                emailTemplate = ReportConstants.SasReportEmailTemplate;

            }
            else if (serviceId.Equals(ReportConstants.TravelReportServiceId))
            {

                emailDestination = new RestAddress() { Email = emailAddress, Name = emailAddress };

                emailSubject = ReportConstants.EmailSubjectTravelReport;
                emailTemplate = ReportConstants.TravelReportEmailTemplate;

            }

            var tokenKey = ServicesHelper.GetSystemProperty(systemId, "KeyVault.Token").Result;


            var token = PayloadBuilder.New().Key(systemId, tokenKey)
                .Expiry(TimeSpan.FromDays(365))
                .Add("ApplicationId", applicationId)
                .Add("UserId", userId)
                .Issue(ServicesHelper.TokenApi).Result;


            ;

            var apiHost = ConfigurationSystem.AppSettings["ApiHost"];
            var webHost = ConfigurationSystem.AppSettings["WebHost"];
            ;

            var link = $"{apiHost}/v1/UChannel/reports?token={token}";
            var data = new EmailData()
            {
                ApplicationId = applicationId,
                ReportLink = link,
                Parameters = new ConcurrentDictionary<string, object>()

            };

            var host = webHost;
            data.Parameters.Add("Host", host);

            var templateApi = new Template.Api.CRUDApi(ConfigurationSystem.AppSettings["TemplateApi"]);
            var content = templateApi.RenderTemplate(emailTemplate, new Template.Model.Context(data.ToJsonString()));

            return SendEmail(emailSubject, string.Empty, content, emailDestination);

        }
        class EmailData
        {
            public string ApplicationId { get; set; }
            public string ReportLink { get; set; }
            public IDictionary<string, object> Parameters { get; set; }

        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            ConfigurationSystem.Load("ReportGenerator");
            ServicesHelper.SystemProperties = new SystemProperties(5);
            ServicesHelper.SystemApi = new SystemApi(ConfigurationSystem.AppSettings["SystemApi"]);
            ServicesHelper.LookupApi = new LookupApi(ConfigurationSystem.AppSettings["LookupsApi"]);
            ServicesHelper.ApplicationApi = new ApplicationApi(ConfigurationSystem.AppSettings["ApplicationApi"]);
            ServicesHelper.ApplicationSearchApi = new ApplicationSearchApi(ConfigurationSystem.AppSettings["ApplicationApi"]);
            ServicesHelper.TokenApi = new TokensApi(ConfigurationSystem.AppSettings["TokensApi"]);
            ServicesHelper.EmailApi = new EmailApi(ConfigurationSystem.AppSettings["EmailApi"]);

            ServicesHelper.VisionApi = new VisionIndividualApi(ConfigurationSystem.AppSettings["VisionApi"]);
            ServicesHelper.VisionEstabApi = new VisionEstablishmentApi(ConfigurationSystem.AppSettings["VisionApi"]);

          
            Program p = new Program();
            p.Process();
            p.SendEmails();
        }
    }
}
