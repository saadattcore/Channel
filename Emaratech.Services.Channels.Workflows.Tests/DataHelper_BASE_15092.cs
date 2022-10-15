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
                        wsp.WorkflowStepId = step.Id;
                    }
                }

                step.SpecificPairs = kvp.Value;

                List<StepParam> stepParam = new List<StepParam>
                {
                    CreateStepParam(step.Id, StepParamType.Configuration, "Class", kvp.Key)
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
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Type = StepType.Execute
            };
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

        public static List<WorkflowStepParam> GetResolveFeesStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ViolationAmount", "violationAmount"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Output, "Fees", "fees"),
                CreateWorklowStepParam(null, StepParamType.Output, "WorkflowFee", "workflowFee"),
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
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Output, "Data", "data")
            };
        }

        public static List<WorkflowStepParam> GetFormDataStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
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
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Output, "Documents", "documents")
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
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "ResidencePPSID", "ResidencePPSID"),
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
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorTypeId", "sponsorTypeId"),
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
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid")
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
                CreateWorklowStepParam(null, StepParamType.Output, "EntryPermitPPSID", "entryPermitPPSID"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "PPSID", "ppsid")
            };
        }

        public static List<WorkflowStepParam> GetResolveVisaTypeAppTypeStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "CategoryId", "categoryId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorType", "sponsorType"),
                CreateWorklowStepParam(null, StepParamType.Input, "EstablishmentType", "establishmentType"),
                CreateWorklowStepParam(null, StepParamType.Output, "VisaType", "visaType"),
                CreateWorklowStepParam(null, StepParamType.Output, "AppType", "appType"),
                CreateWorklowStepParam(null, StepParamType.Output, "AppSubType", "appSubType"),
                CreateWorklowStepParam(null, StepParamType.Output, "ServiceCorrespondingToCategory", "serviceId")
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
                CreateWorklowStepParam(null, StepParamType.Input, "SasReportEmailAddress", "sasReportEmailAddress"),
                CreateWorklowStepParam(null, StepParamType.Output, "ReportData", "reportData"),
                CreateWorklowStepParam(null, StepParamType.Output, "EmailHeader", "emailHeader"),
                CreateWorklowStepParam(null, StepParamType.Output, "EmailContent", "emailContent"),
            };
        }

        public static List<WorkflowStepParam> GetSendEmailStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "EmailHeader", "emailHeader"),
                CreateWorklowStepParam(null, StepParamType.Input, "EmailContent", "emailContent")
            };
        }
        public static List<WorkflowStepParam> GetReadEmailAddressForSasReportStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationData", "applicationData"),
                CreateWorklowStepParam(null, StepParamType.Output, "SasReportEmailAddress", "sasReportEmailAddress")
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
                CreateWorklowStepParam(null, StepParamType.Output, "ApplicationDataToSave", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Output, "BirthDate", "birthDate"),
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
                CreateWorklowStepParam(null, StepParamType.Output, "ApplicationId", "applicationId")
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
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
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
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication")
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
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication")
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
                CreateWorklowStepParam(null, StepParamType.Output, "PaymentUrl", "paymentUrl"),
                //CreateWorklowStepParam(null, StepParamType.Input, "WorkflowFee", "workflowFee")

            };
        }

        public static List<WorkflowStepParam> GetMarkPaymentStatusStepParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "ApplicationId", "applicationId"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "AuthenticatedSystemId", "authenticatedSystemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "Request", "request"),
                CreateWorklowStepParam(null, StepParamType.Output, "Error", "error"),
                CreateWorklowStepParam(null, StepParamType.Output, "PaymentCompleteUrl", "paymentCompleteUrl"),
                CreateWorklowStepParam(null, StepParamType.Output, "BatchId", "batchId")
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

        public static List<WorkflowStepParam> GetWaitForPaymentStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "Request", "request")
            };
        }

        public static List<WorkflowStepParam> GetResidenceTravelStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidencePPSID", "ResidencePPSID"),
                CreateWorklowStepParam(null, StepParamType.Input, "ResidenceNo", "residenceNo"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplicationWithTravelStatus", "unifiedApplication")
            };
        }

        public static List<WorkflowStepParam> GetEntryPermitTravelStatusParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
                CreateWorklowStepParam(null, StepParamType.Input, "EntryPermitPPSID", "entryPermitPPSID"),
                CreateWorklowStepParam(null, StepParamType.Input, "PermitNo", "permitNo"),
                CreateWorklowStepParam(null, StepParamType.Output, "UnifiedApplicationWithTravelStatus", "unifiedApplication")
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

        public static List<WorkflowStepParam> GetFilterRelationshipsParams()
        {
            return new List<WorkflowStepParam>
            {
                CreateWorklowStepParam(null, StepParamType.Input, "FormConfiguration", "formConfiguration"),
                CreateWorklowStepParam(null, StepParamType.Input, "UserType", "userType"),
                CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                CreateWorklowStepParam(null, StepParamType.Input, "ServiceId", "serviceId"),
                CreateWorklowStepParam(null, StepParamType.Input, "UnifiedApplication", "unifiedApplication"),
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
    }

    internal class StepNames
    {
        public static readonly string FilterVisaType = "Emaratech.Services.Channels.Workflows.Steps.FilterVisaType, Emaratech.Services.Channels.Workflows";
        public static readonly string FilterRelationships = "Emaratech.Services.Channels.Workflows.Steps.FilterRelationships, Emaratech.Services.Channels.Workflows";
        public static readonly string FilterPassportType = "Emaratech.Services.Channels.Workflows.Steps.FilterPassportType, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildNewApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildNewApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildResidenceApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildResidenceApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildEntryPermitApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildEntryPermitApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string GetResidenceTravelStatus = "Emaratech.Services.Channels.Workflows.Steps.GetResidenceTravelStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string GetEntryPermitTravelStatus = "Emaratech.Services.Channels.Workflows.Steps.GetEntryPermitTravelStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveVisaTypeAppType = "Emaratech.Services.Channels.Workflows.Steps.ResolveVisaTypeAppType, Emaratech.Services.Channels.Workflows";
        public static readonly string SetApplicationDefaultValues = "Emaratech.Services.Channels.Workflows.Steps.SetApplicationDefaultValues, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolvePreForm = "Emaratech.Services.Channels.Workflows.Steps.ResolvePreForm, Emaratech.Services.Channels.Workflows";
        public static readonly string SetPreFormView = "Emaratech.Services.Channels.Workflows.Steps.SetPreFormView, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveForm = "Emaratech.Services.Channels.Workflows.Steps.ResolveForm, Emaratech.Services.Channels.Workflows";
        public static readonly string SetFormView = "Emaratech.Services.Channels.Workflows.Steps.SetFormView, Emaratech.Services.Channels.Workflows";
        public static readonly string GetFormData = "Emaratech.Services.Channels.Workflows.Steps.GetFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string GetFormData2 = "Emaratech.Services.Channels.Workflows.Steps.GetFormData2, Emaratech.Services.Channels.Workflows";
        public static readonly string ReuseFormData = "Emaratech.Services.Channels.Workflows.Steps.ReuseFormData, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveDocuments = "Emaratech.Services.Channels.Workflows.Steps.ResolveDocuments, Emaratech.Services.Channels.Workflows";
        public static readonly string SetDocumentView = "Emaratech.Services.Channels.Workflows.Steps.SetDocumentView, Emaratech.Services.Channels.Workflows";
        public static readonly string SaveDocuments = "Emaratech.Services.Channels.Workflows.Steps.SaveDocuments, Emaratech.Services.Channels.Workflows";
        public static readonly string ResolveFees = "Emaratech.Services.Channels.Workflows.Steps.ResolveFees, Emaratech.Services.Channels.Workflows";
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
        public static readonly string MakePayment = "Emaratech.Services.Channels.Workflows.Steps.Pay, Emaratech.Services.Channels.Workflows";
        public static readonly string MarkPaymentStatus = "Emaratech.Services.Channels.Workflows.Steps.MarkPaymentStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForApplication = "Emaratech.Services.Channels.Workflows.Steps.WaitForApplication, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForDocuments = "Emaratech.Services.Channels.Workflows.Steps.WaitForDocuments, Emaratech.Services.Channels.Workflows";
        public static readonly string WaitForPaymentStatus = "Emaratech.Services.Channels.Workflows.Steps.WaitForPaymentStatus, Emaratech.Services.Channels.Workflows";
        public static readonly string CreateSnsReport = "Emaratech.Services.Channels.Workflows.Steps.CreateSnsReport, Emaratech.Services.Channels.Workflows";
        public static readonly string SendEmail = "Emaratech.Services.Channels.Workflows.Steps.SendEmail, Emaratech.Services.Channels.Workflows";
        public static readonly string ReadEmailAddressForSasReport = "Emaratech.Services.Channels.Workflows.Steps.ReadEmailAddressForSasReport, Emaratech.Services.Channels.Workflows";
        public static readonly string BuildNewResidenceApplication = "Emaratech.Services.Channels.Workflows.Steps.BuildNewResidenceApplication, Emaratech.Services.Channels.Workflows";
    }
}
