using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Workflows.Tests
{
    public class DataHelper
    {
        public static WorkflowModel BuildSimpleWorkflow(Dictionary<string, List<WorkflowStepParam>> stepsByclassName)
        {
            WorkflowModel workflow = new WorkflowModel
            {
                Name = "Work",
                WorkflowId = Guid.NewGuid().ToString()
            };

            List<Step> steps = new List<Step>();
            foreach (var kvp in stepsByclassName)
            {
                Step step = CreateStepConfig("Step", string.Empty);

                if (kvp.Value != null)
                {
                    foreach (WorkflowStepParam wsp in kvp.Value)
                    {
                        wsp.WorkflowStepId = step.StepId;
                    }
                }

                step.SpecificPairs = kvp.Value;

                List<StepParam> stepParam = new List<StepParam>
                {
                    CreateStepParam(step.StepId, StepParamType.Configuration, "Class", kvp.Key)
                };
                step.Pairs = stepParam;

                steps.Add(step);
            }

            workflow.AttachedSteps = steps;
            return workflow;
        }

        public static Step CreateStepConfig(string name, string description)
        {
            Step step = new Step
            {
                StepId = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Type = StepType.Execute
            };
            step.WorkflowStepId = step.StepId;
            return step;
        }

        public static StepParam CreateStepParam(string stepId, StepParamType type, string key, string value)
        {
            return new StepParam
            {
                Id = Guid.NewGuid().ToString(),
                StepId = stepId,
                Type = type,
                Key = key,
                Value = value
            };
        }

        public static WorkflowStepParam CreateWorklowStepParam(string stepId, StepParamType type, string key, string value)
        {
            return new WorkflowStepParam
            {
                Id = Guid.NewGuid().ToString(),
                WorkflowStepId = stepId,
                Type = type,
                Key = key,
                Value = value
            };
        }

        public static List<WorkflowStepParam> GetResolveFormStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "CategoryId", "categoryId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "Platform", "platform"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormId", "formId"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> GetResolvePreFormStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "CategoryId", "categoryId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "Platform", "platform"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormId", "formId"),
                CreateWorklowStepParam(null, StepParamType.Output, "Type", "type"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> GetTravelStatusReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportData", "reportData"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
               // CreateWorklowStepParam(null, StepParamType.Output, "EmailContent", "emailContent"),
                //CreateWorklowStepParam(null, StepParamType.Output, "EmailHeader", "emailHeader"),
                CreateWorklowStepParam(null, StepParamType.Output, "FileType", "fileType"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationDataToSave", "unifiedApplication")
                //CreateWorklowStepParam(null, StepParamType.Input, "PPSID", "ppsid")
            };
        }

        public static List<WorkflowStepParam> GetResolveFeesStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ViolationAmount", "violationAmount"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentCode", "establishmentCode"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "JobId", "jobId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorSponsorType", "sponsorSponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Output, "Fees", "fees"),
                CreateWorklowStepParam(null, StepParamType.Input, "WorkflowFee", "workflowFee"),
                CreateWorklowStepParam(null, StepParamType.Output, "Amount", "amount")
            };
        }

        public static List<WorkflowStepParam> GetResolveFeesPassportServicesStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportApplication", "passportApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "FeeDetails", "feeDetails"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Output, "Fees", "fees"),
                CreateWorklowStepParam(null, StepParamType.Output, "PaymentUrl", "paymentUrl"),
                CreateWorklowStepParam(null, StepParamType.Output, "Amount", "amount")
            };
        }

        public static List<WorkflowStepParam> GetResolveFormAndFeesStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view")
            };
        }

        public static List<WorkflowStepParam> GetReuseDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorEmail", "email"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "MobileNo", "mobileNumber"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Output, "Data", "data"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
            };
        }

        public static List<WorkflowStepParam> GetReuseLegalAdviceFormDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "LegalAdviceNumber", "legalAdviceNumber"),
                CreateWorklowStepParam(null, StepParamType.Output, "Data", "data"),
                CreateWorklowStepParam(null, StepParamType.Output, "Documents", "documents"),
                CreateWorklowStepParam(null, StepParamType.Output, "LegalAdvice", "legalAdvice")
            };
        }

        public static List<WorkflowStepParam> GetReusePassportFormDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SearchInfo", "searchInfo"),
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Output, "Data", "data"),
                CreateWorklowStepParam(null, StepParamType.Output, "Documents", "documents"),
                CreateWorklowStepParam(null, StepParamType.Reference, "PassportApplication", "passportApplication")
            };
        }

        public static List<WorkflowStepParam> GetFormDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                //CreateWorklowStepParam(null, StepParamType.Output, "ApplicationDataToEmpty", "applicationData"),
                //CreateWorklowStepParam(null, StepParamType.Output, "ApplicationDataToValidate", "applicationDataToValidate"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplicationWithFormData", "unifiedApplication")
            };
        }

        public static List<WorkflowStepParam> GetResolveDocumentsStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentCode", "establishmentCode"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "JobId", "jobId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorSponsorType", "sponsorSponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Output, "Documents", "documents")
            };
        }

        public static List<WorkflowStepParam> GetResolveDocumentsPassportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Reference, "Documents", "documents")
            };
        }

        public static List<WorkflowStepParam> GetBuildResidenceApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "ResidencePPSID", "residencePPSID"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid")
            };
        }

        public static List<WorkflowStepParam> GetBuildNewResidenceApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid"),
                CreateWorklowStepParam(null, StepParamType.Output, "EntryPermitPPSID", "entryPermitPPSID")
            };
        }

        public static List<WorkflowStepParam> GetBuildNewApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid")
            };
        }

        public static List<WorkflowStepParam> GetBuildNewApplicationTravelStatsReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid"),
                 CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
            };
        }

        public static List<WorkflowStepParam> GetBuildEntryPermitApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Output, "EntryPermitPPSID", "entryPermitPPSID"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid")
            };
        }

        public static List<WorkflowStepParam> GetSetDependentsViewStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "ListTravelledDependents", "Dependents"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ParentCategoryId", "parentCategoryId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId")
            };
        }

        public static List<WorkflowStepParam> GetSetReportsDependentsViewStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "ListTravelledDependents", "Dependents"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId")
            };
        }


        public static List<WorkflowStepParam> GetBuildWaitForDependentsViewStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
            };
        }

        public static List<WorkflowStepParam> GetResolveVisaTypeAppTypeStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorSponsorType", "sponsorSponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Output, "VisaType", "visaType"),
                CreateWorklowStepParam(null, StepParamType.Output, "AppType", "appType"),
                CreateWorklowStepParam(null, StepParamType.Output, "AppSubType", "appSubType"),
            };
        }

        public static List<WorkflowStepParam> GetSetApplicationDefaultValuesStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplicationDefaulted", "unifiedApplication")
            };
        }

        public static List<WorkflowStepParam> GetCreateSnsReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "BirthDate", "birthDate"),
                CreateWorklowStepParam(null, StepParamType.Input, "EmiratesId", "emiratesId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportData", "reportData"),
                CreateWorklowStepParam(null, StepParamType.Output, "EmailHeader", "emailHeader"),
                CreateWorklowStepParam(null, StepParamType.Output, "EmailContent", "emailContent"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationDataToSave", "unifiedApplication"),
            };
        }

        public static List<WorkflowStepParam> GetCreateEstablishmentSnsReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentCode", "establishmentCode"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportData", "reportData"),
                CreateWorklowStepParam(null, StepParamType.Output, "EmailHeader", "emailHeader"),
                CreateWorklowStepParam(null, StepParamType.Output, "EmailContent", "emailContent"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationDataToSave", "unifiedApplication"),
            };
        }

        public static List<WorkflowStepParam> GetSendEmailStepParams()
        {
            return new List<WorkflowStepParam>
            {
                 //CreateWorklowStepParam(null, StepParamType.Input, "ToEmailName", "toEmailName"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Configuration, "EmailSubject", "Sas Report"),
               // CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
               // CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportData", "reportData")
                // CreateWorklowStepParam(null, StepParamType.Input, "FileType", "fileType")
            };
        }
        public static List<WorkflowStepParam> GetReadEmailAddressForSasReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportEmailAddress", "reportEmailAddress")

            };
        }

        public static List<WorkflowStepParam> ReadDataEstablishmentSasReport()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportEmailAddress", "reportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Output, "EstablishmentCode", "establishmentCode"),

            };
        }

        public static List<WorkflowStepParam> GetReadEmailAddressForReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportEmailAddress", "reportEmailAddress")
            };
        }
        public static List<WorkflowStepParam> GetValidateDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "ApplicationDataToSave", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "BirthDate", "birthDate"),
                CreateWorklowStepParam(null, StepParamType.Output, "PassportExpiryDate", "passportExpiryDate"),
                CreateWorklowStepParam(null, StepParamType.Output, "YearsOfResidence", "yearsOfResidence")
            };
        }

        public static List<WorkflowStepParam> GetValidateApplicationStepParams()
        {

            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "VisaType", "visaType"),
                CreateWorklowStepParam(null, StepParamType.Input, "AppType", "appType"),
                CreateWorklowStepParam(null, StepParamType.Input, "AppSubType", "appSubType"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Output, "ApplicationDataToSave", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "BirthDate", "birthDate"),
                CreateWorklowStepParam(null, StepParamType.Output, "Relationship", "relationship"),
                CreateWorklowStepParam(null, StepParamType.Output, "CurrentNationality", "currentNationality"),
                CreateWorklowStepParam(null, StepParamType.Output, "PassportExpiryDate", "passportExpiryDate"),
                CreateWorklowStepParam(null, StepParamType.Output, "YearsOfResidence", "yearsOfResidence")
            };
        }

        public static List<WorkflowStepParam> GetSaveDocumentsStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "DocumentIds", "documentIds")
            };
        }

        public static List<WorkflowStepParam> GetSaveApplicationWithReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportData", "reportData"),
                CreateWorklowStepParam(null, StepParamType.Output, "ApplicationId", "applicationId")
            };
        }

        public static List<WorkflowStepParam> GetSaveApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                //CreateWorklowStepParam(null, StepParamType.Input, "ApplicationDataToSave", "applicationDataToSave"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationId", "applicationId")
            };
        }

        public static List<WorkflowStepParam> GetValidateApplicantPassportParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportExpiryDate", "passportExpiryDate")
            };
        }

        public static List<WorkflowStepParam> GetValidateApplicantSponsoredStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo")
            };
        }

        public static List<WorkflowStepParam> GetValidateHealthTestStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "BirthDate", "birthDate")
            };
        }

        public static List<WorkflowStepParam> GetValidateInsideCountryStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "PPSID", "ppsid")
            };
        }

        public static List<WorkflowStepParam> GetValidateResidenceStatusStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo")
            };
        }

        public static List<WorkflowStepParam> GetValidateSponsorInfoStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                 CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType")
            };
        }

        public static List<WorkflowStepParam> GetValidateNumberOfYearsParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "YearsOfResidence", "yearsOfResidence"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType")
            };
        }

        public static List<WorkflowStepParam> GetValidateTravelStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "PPSID", "ppsid"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Output, "CancellationReason", "cancellationReason")
            };
        }

        public static List<WorkflowStepParam> GetValidateViolationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
                CreateWorklowStepParam(null, StepParamType.Output, "ViolationAmount", "violationAmount"),
                CreateWorklowStepParam(null, StepParamType.Output, "NotificationMessage", "notificationMessage")
            };
        }

        public static List<WorkflowStepParam> GetValidateSponsorWifesCountStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "Relationship", "relationship")
            };
        }

        public static List<WorkflowStepParam> GetValidateSponsorFileNumberStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType")
            };
        }

        public static List<WorkflowStepParam> GetCheckExistingApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "CurrentNationality", "currentNationality"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportNo", "passportNo")
            };
        }

        public static List<WorkflowStepParam> GetPayStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "AuthenticatedSystemId", "authenticatedSystemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "Amount", "amount"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ViolationAmount", "violationAmount"),
                //CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Input, "Fees", "fees"),
                CreateWorklowStepParam(null, StepParamType.Reference, "PaymentUrl", "paymentUrl"),
                //CreateWorklowStepParam(null, StepParamType.Input, "WorkflowFee", "workflowFee")

            };
        }

        public static List<WorkflowStepParam> GetMarkPaymentStatusStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "PPSID", "ppsId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "ViolationAmount", "violationAmount"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "AuthenticatedSystemId", "authenticatedSystemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "Request", "request"),
                CreateWorklowStepParam(null, StepParamType.Output, "Error", "error"),
                CreateWorklowStepParam(null, StepParamType.Output, "AppStatus", "appStatus"),
                CreateWorklowStepParam(null, StepParamType.Output, "PaymentCompleteUrl", "paymentCompleteUrl"),
                CreateWorklowStepParam(null, StepParamType.Output, "BatchId", "batchId")
            };
        }

        public static List<WorkflowStepParam> GetSubmitZajelApplicationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "MobileNumber", "mobileNumber"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "Fees", "fees"),
                CreateWorklowStepParam(null, StepParamType.Output, WorkflowConstants.WorkflowParameterKeys.UniqueId, WorkflowConstants.WorkflowParameterJsonKeys.UniqueId)
            };
        }

        public static List<WorkflowStepParam> GetSetDisclaimerViewParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "ResourceKey", "resourceKey")
            };
        }

        public static List<WorkflowStepParam> GetSetViewParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view")
            };
        }

        public static List<WorkflowStepParam> GetWaitForApplicationParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData")
            };
        }

        public static List<WorkflowStepParam> GetWaitForDocumentsParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "DocumentIds", "documentIds")
            };
        }

        public static List<WorkflowStepParam> GetWaitForActionParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "Action", "action")
            };
        }

        public static List<WorkflowStepParam> GetWaitForPaymentStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "Request", "request")
            };
        }

        public static List<WorkflowStepParam> GetWaitForDisclaimerApprovalParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "DisclaimerApproved", "disclaimerApproved"),
                CreateWorklowStepParam(null, StepParamType.Input, "SearchInfo", "searchInfo")
            };
        }

        public static List<WorkflowStepParam> GetResidenceTravelStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Reference, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidencePPSID", "ResidencePPSID"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplicationWithTravelStatus", "unifiedApplication")
            };
        }

        public static List<WorkflowStepParam> GetEntryPermitTravelStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Reference, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "EntryPermitPPSID", "entryPermitPPSID"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
            };
        }

        public static List<WorkflowStepParam> GetFilterVisaTypeParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "CategoryId", "categoryId"),
                CreateWorklowStepParam(null, StepParamType.Output, "UpdatedFormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> GetFilterPreferredLanguagesParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Output, "UpdatedFormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> GetFilterRelationshipsParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Output, "UpdatedFormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> GetFilterPassportTypeParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Output, "UpdatedFormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> FetchIndividualProfileByPassportInfoParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Output, "PermitNo", "permitNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "Platform", "platform"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormId", "formId")
            };
        }

        public static List<WorkflowStepParam> GetValidateEntryPermitStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo")
            };
        }

        public static List<WorkflowStepParam> GetSendEmailTravelReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                //CreateWorklowStepParam(null, StepParamType.Input, "ToEmailName", "toEmailName"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Configuration, "EmailSubject", "Travel Report"),
               // CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
               // CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportData", "reportData"),
                CreateWorklowStepParam(null, StepParamType.Input, "FileType", "fileType")
            };
        }


        public static List<WorkflowStepParam> GetSendEmailEstabSasReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                //CreateWorklowStepParam(null, StepParamType.Input, "ToEmailName", "toEmailName"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Configuration, "EmailSubject", "Establishment Sas Report"),
               // CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
               // CreateWorklowStepParam(null, StepParamType.Input, "UserId", "userId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ReportData", "reportData")
                // CreateWorklowStepParam(null, StepParamType.Input, "FileType", "fileType")
            };
        }

        public static List<WorkflowStepParam> GetBuildTransferResidenceDependentsViewStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "ListTravelledDependents", "Dependents"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId")
            };
        }

        public static List<WorkflowStepParam> GetSaveLegalAdviceStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Input, "DocumentIds", "documentIds"),
                CreateWorklowStepParam(null, StepParamType.Input, "Action", "action"),
                CreateWorklowStepParam(null, StepParamType.Input, "LegalAdvice", "legalAdvice"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Reference, "LegalRequestId", "legalRequestId")
            };
        }

        public static List<WorkflowStepParam> GetPassportFormDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Reference, "DocumentIds", "documentIds"),
                CreateWorklowStepParam(null, StepParamType.Reference, "PassportApplication", "passportApplication")
            };
        }

        public static List<WorkflowStepParam> GetSavePassportRequestStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportApplication", "passportApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "Action", "action"),
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "FeeDetails", "feeDetails")
            };
        }

        public static List<WorkflowStepParam> GetSetPassportRenewListDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "ListData", "listData"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportApplication", "passportApplication")
            };
        }

        public static List<WorkflowStepParam> SavePassportRenewRequestStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportApplication", "passportApplication")
            };
        }

        public static List<WorkflowStepParam> GetPassportServicesFormButtonsStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Output, "AdditionalButtons", "additionalButtons"),
                CreateWorklowStepParam(null, StepParamType.Input, "PassportApplication", "passportApplication")
            };
        }

        public static List<WorkflowStepParam> GetCheckRelationshipStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "GenderId", "genderId"),
                CreateWorklowStepParam(null, StepParamType.Input, "VisaType", "visaType"),
                CreateWorklowStepParam(null, StepParamType.Input, "Relationship", "relationship"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData")
            };
        }

        public static List<WorkflowStepParam> GetSponsorFileCheckStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "PlatformId", "platformId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Reference, "WorkflowFee", "workflowFee"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormId", "formId"),
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Output, "IsSponsorFileOpen", "isSponsorFileOpen"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormConfiguration", "formConfiguration")
            };
        }

        public static List<WorkflowStepParam> GetPassportRenewGroupingStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "Platform", "platform"),
                CreateWorklowStepParam(null, StepParamType.Reference, "SearchInfo", "searchInfo"),
                CreateWorklowStepParam(null, StepParamType.Input, "Action", "action"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Reference, "DocumentIds", "documentIds"),
                CreateWorklowStepParam(null, StepParamType.Reference, "PassportApplication", "passportApplication"),
                CreateWorklowStepParam(null, StepParamType.Reference, "Documents", "documents"),
                CreateWorklowStepParam(null, StepParamType.Output, "View", "view"),
                CreateWorklowStepParam(null, StepParamType.Reference, "AdditionalButtons", "additionalButtons"),
                CreateWorklowStepParam(null, StepParamType.Output, "Data", "data"),
                CreateWorklowStepParam(null, StepParamType.Output, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Reference, "ListData", "listData")
            };
        }

        public static List<WorkflowStepParam> GetCheckFormValidationStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData")
            };
        }
    }

    internal class StepNames
    {
        public static readonly string SponsorFileCheck = "Emaratech.Services.Channels.Workflows.Steps.SponsorFileCheck, Emaratech.Services.Channels.Workflows";
        public static readonly string FilterFilterPreferredLanguages = "Emaratech.Services.Channels.Workflows.Steps.FilterPreferredLanguages, Emaratech.Services.Channels.Workflows";
        public static readonly string FilterVisaType = "Emaratech.Services.Channels.Workflows.Steps.FilterVisaType, Emaratech.Services.Channels.Workflows";
        public static readonly string FilterRelationships = "Emaratech.Services.Channels.Workflows.Steps.FilterRelationships, Emaratech.Services.Channels.Workflows";
        public static readonly string FilterPassportType = "Emaratech.Services.Channels.Workflows.Steps.FilterPassportType, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildNewApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildNewApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildResidenceApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildResidenceApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildEntryPermitApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildEntryPermitApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildTransfertResidenceNewPassportApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildTransfertResidenceNewPassportApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string GetResidenceTravelStatus = "Emaratech.Services.Channels.Workflows.Steps.GetResidenceTravelStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string GetEntryPermitTravelStatus = "Emaratech.Services.Channels.Workflows.Steps.GetEntryPermitTravelStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveVisaTypeAppType = "Emaratech.Services.Channels.Workflows.Steps.ResolveVisaTypeAppType, Emaratech.Services.Channels.Workflows";
        public static readonly string SetApplicationDefaultValues = "Emaratech.Services.Channels.Workflows.Steps.SetApplicationDefaultValues, Emaratech.Services.Channels.Workflows";
        public static readonly string SetListView = "Emaratech.Services.Channels.Workflows.Steps.SetListView, Emaratech.Services.Channels.Workflows";
        public static readonly string SetAnyDocumentView = "Emaratech.Services.Channels.Workflows.Steps.SetAnyDocumentView, Emaratech.Services.Channels.Workflows";
        public static readonly string SetDisclaimerView = "Emaratech.Services.Channels.Workflows.Steps.SetDisclaimerView, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolvePreForm = "Emaratech.Services.Channels.Workflows.Steps.ResolvePreForm, Emaratech.Services.Channels.Workflows";
        public static readonly string SetPreFormView = "Emaratech.Services.Channels.Workflows.Steps.SetPreFormView, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveForm = "Emaratech.Services.Channels.Workflows.Steps.ResolveForm, Emaratech.Services.Channels.Workflows";
        public static readonly string SetFormView = "Emaratech.Services.Channels.Workflows.Steps.SetFormView, Emaratech.Services.Channels.Workflows";
        public static readonly string GetFormData = "Emaratech.Services.Channels.Workflows.Steps.GetFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string GetFormData2 = "Emaratech.Services.Channels.Workflows.Steps.GetFormData2, Emaratech.Services.Channels.Workflows";
        public static readonly string ReuseFormData = "Emaratech.Services.Channels.Workflows.Steps.ReuseFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string ReuseLegalAdviceFormData = "Emaratech.Services.Channels.Workflows.Steps.ReuseLegalAdviceFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string ReusePassportRenewFormData = "Emaratech.Services.Channels.Workflows.Steps.ReusePassportRenewFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string ReusePassportNewFormData = "Emaratech.Services.Channels.Workflows.Steps.ReusePassportNewFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveDocuments = "Emaratech.Services.Channels.Workflows.Steps.ResolveDocuments, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveDocumentsPassportRenew = "Emaratech.Services.Channels.Workflows.Steps.ResolveDocumentsPassportRenew, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveDocumentsPassportNew = "Emaratech.Services.Channels.Workflows.Steps.ResolveDocumentsPassportNew, Emaratech.Services.Channels.Workflows";
        public static readonly string SetDocumentView = "Emaratech.Services.Channels.Workflows.Steps.SetDocumentView, Emaratech.Services.Channels.Workflows";
        public static readonly string SaveDocuments = "Emaratech.Services.Channels.Workflows.Steps.SaveDocuments, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveFees = "Emaratech.Services.Channels.Workflows.Steps.ResolveFees, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveFeesPassportServices = "Emaratech.Services.Channels.Workflows.Steps.ResolveFeesPassportServices, Emaratech.Services.Channels.Workflows";
        public static readonly string SetFeeView = "Emaratech.Services.Channels.Workflows.Steps.SetFeeView, Emaratech.Services.Channels.Workflows";
        public static readonly string SetFormAndPayView = "Emaratech.Services.Channels.Workflows.Steps.SetFormAndPayView, Emaratech.Services.Channels.Workflows";
        public static readonly string CheckExistingApplication = "Emaratech.Services.Channels.Workflows.Steps.CheckExistingApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateData = "Emaratech.Services.Channels.Workflows.Steps.ValidateData, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateApplicationWithoutAppType = "Emaratech.Services.Channels.Workflows.Steps.ValidateApplicationWithoutAppType, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateApplication = "Emaratech.Services.Channels.Workflows.Steps.ValidateApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateApplicantPassport = "Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateApplicantSponsored = "Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantSponsored, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateHealthTestInfo = "Emaratech.Services.Channels.Workflows.Steps.ValidateHealthTestInfo, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateInsideCountry = "Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateResidenceStatus = "Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateSponsorFileNumber = "Emaratech.Services.Channels.Workflows.Steps.ValidateSponsorFileNumber, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateSponsorInfo = "Emaratech.Services.Channels.Workflows.Steps.ValidateSponsorInfo, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateNumberOfYears = "Emaratech.Services.Channels.Workflows.Steps.ValidateNumberOfYears, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateSponsorWifesCount = "Emaratech.Services.Channels.Workflows.Steps.ValidateSponsorWifesCount, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateTravelInfo = "Emaratech.Services.Channels.Workflows.Steps.ValidateTravelInfo, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateViolationInfo = "Emaratech.Services.Channels.Workflows.Steps.ValidateViolationInfo, Emaratech.Services.Channels.Workflows";
        public static readonly string SaveApplication = "Emaratech.Services.Channels.Workflows.Steps.SaveApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string SaveApplicationWithReport = "Emaratech.Services.Channels.Workflows.Steps.SaveApplicationWithReport, Emaratech.Services.Channels.Workflows";
        public static readonly string SaveLegalAdvice = "Emaratech.Services.Channels.Workflows.Steps.SaveLegalAdvice, Emaratech.Services.Channels.Workflows";
        public static readonly string GetPassportNewData = "Emaratech.Services.Channels.Workflows.Steps.GetPassportNewData, Emaratech.Services.Channels.Workflows";
        public static readonly string GetPassportRenewData = "Emaratech.Services.Channels.Workflows.Steps.GetPassportRenewData, Emaratech.Services.Channels.Workflows";
        public static readonly string GetPassportRenewData2 = "Emaratech.Services.Channels.Workflows.Steps.GetPassportRenewData2, Emaratech.Services.Channels.Workflows";
        public static readonly string GetPassportRenewData3 = "Emaratech.Services.Channels.Workflows.Steps.GetPassportRenewData3, Emaratech.Services.Channels.Workflows";
        public static readonly string SetPassportRenewListData = "Emaratech.Services.Channels.Workflows.Steps.SetPassportRenewListData, Emaratech.Services.Channels.Workflows";
        public static readonly string SavePassportRenewRequest = "Emaratech.Services.Channels.Workflows.Steps.SavePassportRenewRequest, Emaratech.Services.Channels.Workflows";
        public static readonly string SavePassportNewRequest = "Emaratech.Services.Channels.Workflows.Steps.SavePassportNewRequest, Emaratech.Services.Channels.Workflows";
        public static readonly string MakePayment = "Emaratech.Services.Channels.Workflows.Steps.Pay, Emaratech.Services.Channels.Workflows";
        public static readonly string MarkPaymentStatus = "Emaratech.Services.Channels.Workflows.Steps.MarkPaymentStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForApplication = "Emaratech.Services.Channels.Workflows.Steps.WaitForApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForDocuments = "Emaratech.Services.Channels.Workflows.Steps.WaitForDocuments, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForPaymentStatus = "Emaratech.Services.Channels.Workflows.Steps.WaitForPaymentStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForDisclaimerApproval = "Emaratech.Services.Channels.Workflows.Steps.WaitForDisclaimerApproval, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForAction = "Emaratech.Services.Channels.Workflows.Steps.WaitForAction, Emaratech.Services.Channels.Workflows";
        public static readonly string CreateSnsReport = "Emaratech.Services.Channels.Workflows.Steps.CreateSnsReport, Emaratech.Services.Channels.Workflows";
        public static readonly string CreateEstablishmentSasReport = "Emaratech.Services.Channels.Workflows.Steps.CreateEstablishmentSasReport, Emaratech.Services.Channels.Workflows";
        public static readonly string SendEmail = "Emaratech.Services.Channels.Workflows.Steps.SendEmailSasReport, Emaratech.Services.Channels.Workflows";
        public static readonly string ReadEmailAddressForSasReport = "Emaratech.Services.Channels.Workflows.Steps.ReadEmailAddressForSasReport, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildNewResidenceApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildNewResidenceApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string FetchIndividualProfileByPassportInfo = "Emaratech.Services.Channels.Workflows.Steps.FetchIndividualProfileByPassportInfo, Emaratech.Services.Channels.Workflows";
        public static readonly string ValidateEntryPermitStatus = "Emaratech.Services.Channels.Workflows.Steps.ValidateEntryPermitStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string SubmitZajelApplication = "Emaratech.Services.Channels.Workflows.Steps.SubmitZajelApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string CheckRelationship = "Emaratech.Services.Channels.Workflows.Steps.CheckRelationship, Emaratech.Services.Channels.Workflows";
        public static readonly string PassportRenewGrouping = "Emaratech.Services.Channels.Workflows.Steps.PassportRenewGrouping, Emaratech.Services.Channels.Workflows";

        public static readonly string BuildTravelHistoryReport = "Emaratech.Services.Channels.Workflows.Steps.GenerateTravelStatusReport, Emaratech.Services.Channels.Workflows";
        public static readonly string SetDependentsView = "Emaratech.Services.Channels.Workflows.Steps.SetDependentsView, Emaratech.Services.Channels.Workflows";
        public static readonly string SendEmailTravelReport = "Emaratech.Services.Channels.Workflows.Steps.SendEmailTravelReport, Emaratech.Services.Channels.Workflows";
        public static readonly string ReadEmailAddressForReport = "Emaratech.Services.Channels.Workflows.Steps.ReadEmailAddressForReport, Emaratech.Services.Channels.Workflows";
        public static readonly string ReadDataEstablishmentSasReport = "Emaratech.Services.Channels.Workflows.Steps.ReadDataEstablishmentSasReport, Emaratech.Services.Channels.Workflows";

        public static readonly string WaitForDependentsView = "Emaratech.Services.Channels.Workflows.Steps.WaitForDependents, Emaratech.Services.Channels.Workflows";
        public static readonly string GetPassportServicesFormButtons = "Emaratech.Services.Channels.Workflows.Steps.GetPassportServicesFormButtons, Emaratech.Services.Channels.Workflows";
        public static readonly string SetTransferResidenceDependentsView = "Emaratech.Services.Channels.Workflows.Steps.SetTransferResidenceDependentsView, Emaratech.Services.Channels.Workflows";
        public static readonly string CheckFormValidation = "Emaratech.Services.Channels.Workflows.Steps.CheckFormValidation, Emaratech.Services.Channels.Workflows";
        public static readonly string SetReportsDependentsView = "Emaratech.Services.Channels.Workflows.Steps.SetReportDependentsView, Emaratech.Services.Channels.Workflows";
    }
}
