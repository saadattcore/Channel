using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;

namespace Emaratech.Services.Channels.Models
{

    public class ApplicantWorkflowContext
    {
        [JsonProperty("residenceNo")]
        public string ResidenceNumber { get; set; }

        [JsonProperty("permitNo")]
        public string PermitNumber { get; set; }

        [JsonProperty("workflowToken")]
        public string WorkflowToken { get; set; }

    }

    public class ApplicationWorkflowContext
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("systemId")]
        public string SystemId { get; set; }

        [JsonProperty("authenticatedSystemId")]
        public string AuthenticatedSystemId { get; set; }

        [JsonProperty("serviceId")]
        public string ServiceId { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("platform")]
        public string PlatformId { get; set; }

        [JsonProperty("residenceNo")]
        public string ResidenceNumber { get; set; }

        [JsonProperty("permitNo")]
        public string PermitNumber { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userType")]
        public string UserType { get; set; }

        [JsonProperty("sponsorType")]
        public string SponsorType { get; set; }

        [JsonProperty("sponsorSponsorType")]
        public string SponsorSponsorType { get; set; }

        [JsonProperty("sponsorNo")]
        public string SponsorNo { get; set; }

        [JsonProperty("emiratesId")]
        public string EmiratesId { get; set; }

        [JsonProperty("birthDate")]
        public string BirthDate { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("establishmentType")]
        public string EstablishmentType { get; set; }

        [JsonProperty("establishmentCode")]
        public string EstablishmentCode { get; set; }

        [JsonProperty("stepsAssemblyPath")]
        public string StepsAssemblyPath { get; set; }

        [JsonProperty("visaNumber")]
        public string VisaNumber { get; set; }

        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("legalAdviceNumber")]
        public string LegalAdviceNumber { get; set; }

        [JsonProperty("searchInfo")]
        public RestSearchInfo SearchInfo { get; set; }

        [JsonProperty("jobId")]
        public string JobId { get; set; }

        [JsonProperty("genderId")]
        public string GenderId { get; set; }

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("parentCategoryId")]
        public string ParentCategoryId { get; set; }
    }
}