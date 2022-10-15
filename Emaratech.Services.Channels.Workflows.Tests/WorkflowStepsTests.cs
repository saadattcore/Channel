using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using Emaratech.Services.Workflows.Engine;
using System.Linq;
using System.Threading.Tasks;
//using Emaratech.Services.Channels.Workflows.Steps;
using Newtonsoft.Json.Linq;
//using Emaratech.Services.Channels.Workflows.Models;
using Emaratech.Services.Workflows.Engine.Interfaces;
using System.Text;
using System.Xml;
using log4net.Config;

namespace Emaratech.Services.Channels.Workflows.Tests
{
    class StepLoader : IStepTypeLoader
    {
        public Type Load(string typeName, string path)
        {
            return Type.GetType(typeName);
        }

        public Type Load(string typeName, string path, string workflowId, int version)
        {
            return Type.GetType(typeName);
        }
    }

    [TestClass]
    public class WorkflowStepsTests
    {
        [TestInitialize]
        void Init()
        {
            XmlConfigurator.Configure();
        }

        [TestMethod]
        public async Task CancelResidenceWorkflow_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Get vision data
            stepsByclassName.Add(StepNames.BuildResidenceApplication, DataHelper.GetBuildResidenceApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Set form and pay view step
            stepsByclassName.Add(StepNames.SetFormAndPayView, DataHelper.GetSetViewParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            // Get travel status
            stepsByclassName.Add(StepNames.GetResidenceTravelStatus, DataHelper.GetResidenceTravelStatusParams());

            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            //// Validate travel infos
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateTravelInfo, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateTravelStepParams());

            //// Validate residence status
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

            //// Validate health test
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateHealthTestInfo, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateHealthTestStepParams());

            //// Validate sponsor info
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateSponsorInfo, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateSponsorInfoStepParams());

            //// Validate passport info
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

            //// Validate applicant sponsored
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantSponsored, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantSponsoredStepParams());

            //// Validate violation
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateViolationInfo, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateViolationStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            //Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            // Wait for payment status data
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            // Mark payment status step
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // Submit zajel application step
            stepsByclassName.Add(StepNames.SubmitZajelApplication, DataHelper.GetSubmitZajelApplicationStepParams());
            
            // systemId: eDNRD
            // serviceId: 
            //   - Residence cancel inside country : FB6820583AFD4EF0AB5A5DB9DB289603
            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': 'FB6820583AFD4EF0AB5A5DB9DB289603',
                'categoryId': '5BF70296722248D9B407346749922C66',
                'platform': '00000000000000000000000000000002',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                'mobileNumber': '44444433',
                'email': 'rrr@ff.com',
                 'jobId':'1434021',
                'residenceNo': '20120117002247'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            //Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            //Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            //Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            //Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            //Assert.AreEqual("formAndPay", context.GlobalParameters["view"].ToString());
            //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\":{\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"},\"ApplicantDetails\":{\"CurrentNationalityId\":\"2\",\"VisaNumber\":\"201/2011/7002247\"},\"SponsorDetails\":{\"SponsorEmail\":\"test@test.com\"}}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "pjyK7AAO9y+QRHIcYHRE3j71lQY8k7QzJJdzXA++EimeqZyyzkdF3ZEtGAVLWpxk1+Snb17hhFKHJEFB2vPWvXkY5VeLPN6Snn8xqRGCWg86G5pFW3eFEumkFIb8OQCn6IYfYFNVV1lUkR9WkpbwRXNS8FaeWPl53ni+iDW/TXBVK0AGk9/rBLT8yXdtjWjPPBmBF1Il0iiGO5yUxaBTtjjSSebYVztD4f9OBCiryG+6GtFRkiPSCT5X2/YWgxOxkHLHJiSoxCE3U1lVoWl5CkcDKyaqyiGFzI6WU2sMYBgCFkLy6F2KqKViOQKM/NrIuYfrkB4yIJNSNoahr95MMivARfNPZW+bZND1B78s3PIUfsN3UvIowjUOG2r6PqQhHwGo8vJYSZMTXBzY/oo6U0fYT280MJrSOKdqx7hvD7AB9OiLq3oVqm3Do0L49ndvNXOu4EO0ny8P9jgsCiYgjoFV6b4+y4z9BEN5v7/tamd6YNhWhKDEi0QZYxovWbWRGh7t79W/GCV+EbFPRJxuGzE9FWyucIl9kxkXTgXN4CQ2oLHWnGGh6ul1gMoOowPRyhsnjmEGqeAHtHgp3F/vEzskCoKhwNx09ZIbxp+kG9g2PGHPSK38BGLTEu/sMgola04npLuXBLkuF/Myr1+QRJgPhWfGF65UZ7jl1XKh3oKK40Fdevdlze45WvFVfJht51NxeKyiVXDrNAyxiR+A+qKnDa6ARXuy8cm6zpiuTOsdkNvF0noyWV9e2JGyBbfyFmVQH4P2JkY49mmOqb8yVL8eOh9FKSJIK/YYoOc1rqy9knvayy2hevzCcakqF6O6VlPeHokmpwcodjy9ypCaLXm8XI4590NcxxFgFNIa4rORopO97jflNgUXYGUwNLTFkH6u/4C21UN16CY2I9HsMzmARh1rDEvr0WxZAsc9jpHcuKrn98QPdX0dcCXRJeKZzZ5QYRGZnRuZ8O7fx9A5BdpgOIiKLZxfbaKJGVwDAzFo0UmknifDcL7R0DGXZod4D36BP5VWyO7C4dOUGVktHcM6uQx3sh4qo/1pDFNDT0XGxTaFUsJ8bpSuJKPes/RNq7XLAF1lJ7hJWfPfe2Q4z8XhiqWIzDJVztkQa9LD8ex6jrvY5b74QhIETLwQ/xwX/eBaA0r7dcTkjKqdypL4VilFWvR24I4isV9tI2EJ9ZeT/t68NPmlxB2n3lkqgkLyAoHHoz5o0Re/xILyX7cE/ANdBxLqtqEQABSFGxnZfa/qb4RUSt7dJSY6g5KLJRe+zPzAKaybKlzENwc2RgvZg7PFzaTKY5QDqBVtH4N6bk5cvlEPKcwe34d19jBjyBKdI2OlgaHFignavV4YtFb4yJ9iGrXHiXKYqfARBBH15s8Gf8YPYlfvw+hwW+dlGXGR53Dq2aSgSE7ZVI2GL0+EsImK1o8yWKAl3a6GtKxJxZTRdbYWqUnxZRVgSjJoYykdGCmnFIE8ixESU5dydkE8MJ/KCeFQFwYo3Ak3XRlLWivY9C7DMOiEtuIqYdCRdjkb";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task RenewResidenceWorkflow_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Set dependents view step
            stepsByclassName.Add(StepNames.SetDependentsView, DataHelper.GetSetDependentsViewStepParams());

            // Get vision data
            stepsByclassName.Add(StepNames.BuildResidenceApplication, DataHelper.GetBuildResidenceApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.SponsorFileCheck, DataHelper.GetSponsorFileCheckStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Set form view step
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            // Get travel status
            stepsByclassName.Add(StepNames.GetResidenceTravelStatus, DataHelper.GetResidenceTravelStatusParams());

            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            // Validate residence status
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

            //// Validate passport info
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

            //// Validate inside country
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateInsideCountryStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            // Resolve documents step
            stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

            // Set document view step
            stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

            // Wait for documents
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

            // Save documents step
            stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Set fee view step
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());

            //Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            // Wait for payment status
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            // Mark payment status step
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // systemId: eDNRD
            // serviceId: 
            //   - Residence renew family : 745F4FA7B13B49F392BAB81C980DB7E8
            // Res no:
            //   - 20120117235180
            //   - 784197084063821
            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '745F4FA7B13B49F392BAB81C980DB7E8',
                'parentCategoryId': 'D2FCCA0D10B34596AD3C51A23CA32448',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                'sponsorType': '3',
                'establishmentType': '1',
                'residenceNo': '20120117002247',
                'sponsorNo': '20120137018043'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
            //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"XX0374910\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "12", "22" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "pjyK7AAO9y+QRHIcYHRE3j71lQY8k7QzJJdzXA++EimeqZyyzkdF3ZEtGAVLWpxk1+Snb17hhFKHJEFB2vPWvXkY5VeLPN6Snn8xqRGCWg86G5pFW3eFEumkFIb8OQCn6IYfYFNVV1lUkR9WkpbwRXNS8FaeWPl53ni+iDW/TXBVK0AGk9/rBLT8yXdtjWjPPBmBF1Il0iiGO5yUxaBTtjjSSebYVztD4f9OBCiryG+6GtFRkiPSCT5X2/YWgxOxkHLHJiSoxCE3U1lVoWl5CkcDKyaqyiGFzI6WU2sMYBgCFkLy6F2KqKViOQKM/NrIuYfrkB4yIJNSNoahr95MMivARfNPZW+bZND1B78s3PIUfsN3UvIowjUOG2r6PqQhHwGo8vJYSZMTXBzY/oo6U0fYT280MJrSOKdqx7hvD7AB9OiLq3oVqm3Do0L49ndvNXOu4EO0ny8P9jgsCiYgjoFV6b4+y4z9BEN5v7/tamd6YNhWhKDEi0QZYxovWbWRGh7t79W/GCV+EbFPRJxuGzE9FWyucIl9kxkXTgXN4CQ2oLHWnGGh6ul1gMoOowPRyhsnjmEGqeAHtHgp3F/vEzskCoKhwNx09ZIbxp+kG9g2PGHPSK38BGLTEu/sMgola04npLuXBLkuF/Myr1+QRJgPhWfGF65UZ7jl1XKh3oKK40Fdevdlze45WvFVfJht51NxeKyiVXDrNAyxiR+A+qKnDa6ARXuy8cm6zpiuTOsdkNvF0noyWV9e2JGyBbfyFmVQH4P2JkY49mmOqb8yVL8eOh9FKSJIK/YYoOc1rqy9knvayy2hevzCcakqF6O6VlPeHokmpwcodjy9ypCaLXm8XI4590NcxxFgFNIa4rORopO97jflNgUXYGUwNLTFkH6u/4C21UN16CY2I9HsMzmARh1rDEvr0WxZAsc9jpHcuKrn98QPdX0dcCXRJeKZzZ5QYRGZnRuZ8O7fx9A5BdpgOIiKLZxfbaKJGVwDAzFo0UmknifDcL7R0DGXZod4D36BP5VWyO7C4dOUGVktHcM6uQx3sh4qo/1pDFNDT0XGxTaFUsJ8bpSuJKPes/RNq7XLAF1lJ7hJWfPfe2Q4z8XhiqWIzDJVztkQa9LD8ex6jrvY5b74QhIETLwQ/xwX/eBaA0r7dcTkjKqdypL4VilFWvR24I4isV9tI2EJ9ZeT/t68NPmlxB2n3lkqgkLyAoHHoz5o0Re/xILyX7cE/ANdBxLqtqEQABSFGxnZfa/qb4RUSt7dJSY6g5KLJRe+zPzAKaybKlzENwc2RgvZg7PFzaTKY5QDqBVtH4N6bk5cvlEPKcwe34d19jBjyBKdI2OlgaHFignavV4YtFb4yJ9iGrXHiXKYqfARBBH15s8Gf8YPYlfvw+hwW+dlGXGR53Dq2aSgSE7ZVI2GL0+EsImK1o8yWKAl3a6GtKxJxZTRdbYWqUnxZRVgSjJoYykdGCmnFIE8ixESU5dydkE8MJ/KCeFQFwYo3Ak3XRlLWivY9C7DMOiEtuIqYdCRdjkb";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task CancelEntryPermitWorkflow_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Get vision data
            stepsByclassName.Add(StepNames.BuildEntryPermitApplication, DataHelper.GetBuildEntryPermitApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Set form and pay view step
            stepsByclassName.Add(StepNames.SetFormAndPayView, DataHelper.GetSetViewParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            // Get travel status
            stepsByclassName.Add(StepNames.GetEntryPermitTravelStatus, DataHelper.GetEntryPermitTravelStatusParams());

            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());


            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            //Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            // Wait for payment status data
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            // Mark payment status step
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // Entry permit cancel: F5DD2E421EF648869A01160053124F5D
            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': 'F5DD2E421EF648869A01160053124F5D',
                'categoryId': '80C044D06B3947D792F32829C3653BEE',
                'platform': '00000000000000000000000000000002',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                'mobileNumber': '44444433',
                'email': 'rrr@ff.com',
                'jobId':'1',
                'permitNo': '2010708021132'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();

            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            Assert.AreEqual("formAndPay", context.GlobalParameters["view"].ToString());
            Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\":{\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"},\"ApplicantDetails\":{\"CurrentNationalityId\":\"2\",\"VisaNumber\":\"20120023046338\"},\"SponsorDetails\":{\"SponsorEmail\":\"test@test.com\"}}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "pjyK7AAO9y+QRHIcYHRE3j71lQY8k7QzJJdzXA++EimeqZyyzkdF3ZEtGAVLWpxk1+Snb17hhFKHJEFB2vPWvXkY5VeLPN6Snn8xqRGCWg86G5pFW3eFEumkFIb8OQCn6IYfYFNVV1lUkR9WkpbwRXNS8FaeWPl53ni+iDW/TXBVK0AGk9/rBLT8yXdtjWjPPBmBF1Il0iiGO5yUxaBTtjjSSebYVztD4f9OBCiryG+6GtFRkiPSCT5X2/YWgxOxkHLHJiSoxCE3U1lVoWl5CkcDKyaqyiGFzI6WU2sMYBgCFkLy6F2KqKViOQKM/NrIuYfrkB4yIJNSNoahr95MMivARfNPZW+bZND1B78s3PIUfsN3UvIowjUOG2r6PqQhHwGo8vJYSZMTXBzY/oo6U0fYT280MJrSOKdqx7hvD7AB9OiLq3oVqm3Do0L49ndvNXOu4EO0ny8P9jgsCiYgjoFV6b4+y4z9BEN5v7/tamd6YNhWhKDEi0QZYxovWbWRGh7t79W/GCV+EbFPRJxuGzE9FWyucIl9kxkXTgXN4CQ2oLHWnGGh6ul1gMoOowPRyhsnjmEGqeAHtHgp3F/vEzskCoKhwNx09ZIbxp+kG9g2PGHPSK38BGLTEu/sMgola04npLuXBLkuF/Myr1+QRJgPhWfGF65UZ7jl1XKh3oKK40Fdevdlze45WvFVfJht51NxeKyiVXDrNAyxiR+A+qKnDa6ARXuy8cm6zpiuTOsdkNvF0noyWV9e2JGyBbfyFmVQH4P2JkY49mmOqb8yVL8eOh9FKSJIK/YYoOc1rqy9knvayy2hevzCcakqF6O6VlPeHokmpwcodjy9ypCaLXm8XI4590NcxxFgFNIa4rORopO97jflNgUXYGUwNLTFkH6u/4C21UN16CY2I9HsMzmARh1rDEvr0WxZAsc9jpHcuKrn98QPdX0dcCXRJeKZzZ5QYRGZnRuZ8O7fx9A5BdpgOIiKLZxfbaKJGVwDAzFo0UmknifDcL7R0DGXZod4D36BP5VWyO7C4dOUGVktHcM6uQx3sh4qo/1pDFNDT0XGxTaFUsJ8bpSuJKPes/RNq7XLAF1lJ7hJWfPfe2Q4z8XhiqWIzDJVztkQa9LD8ex6jrvY5b74QhIETLwQ/xwX/eBaA0r7dcTkjKqdypL4VilFWvR24I4isV9tI2EJ9ZeT/t68NPmlxB2n3lkqgkLyAoHHoz5o0Re/xILyX7cE/ANdBxLqtqEQABSFGxnZfa/qb4RUSt7dJSY6g5KLJRe+zPzAKaybKlzENwc2RgvZg7PFzaTKY5QDqBVtH4N6bk5cvlEPKcwe34d19jBjyBKdI2OlgaHFignavV4YtFb4yJ9iGrXHiXKYqfARBBH15s8Gf8YPYlfvw+hwW+dlGXGR53Dq2aSgSE7ZVI2GL0+EsImK1o8yWKAl3a6GtKxJxZTRdbYWqUnxZRVgSjJoYykdGCmnFIE8ixESU5dydkE8MJ/KCeFQFwYo3Ak3XRlLWivY9C7DMOiEtuIqYdCRdjkb";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task EntryPermitNewWorkflow_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Build application
            stepsByclassName.Add(StepNames.BuildEntryPermitApplication, DataHelper.GetBuildEntryPermitApplicationStepParams());
            
            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Reuse form data step
            //stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Filter visa type step
            stepsByclassName.Add(StepNames.FilterVisaType, DataHelper.GetFilterVisaTypeParams());

            // Filter passport type step
            stepsByclassName.Add(StepNames.FilterPassportType, DataHelper.GetFilterPassportTypeParams());

            // Filter relationship step
            stepsByclassName.Add(StepNames.FilterRelationships, DataHelper.GetFilterRelationshipsParams());

            // Set form view step
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Check form validations
            //stepsByclassName.Add(StepNames.CheckFormValidation, DataHelper.GetCheckFormValidationStepParams());
            
            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());
            
            // Validate data
            //stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            // Check existing app
            //stepsByclassName.Add(StepNames.CheckExistingApplication, DataHelper.GetCheckExistingApplicationStepParams());

            stepsByclassName.Add(StepNames.CheckRelationship, DataHelper.GetCheckRelationshipStepParams());

            //// Validate passport info
            //stepsByclassName.Add(StepNames.ValidateApplicantPassport, DataHelper.GetValidateApplicantPassportParams());

            //// Validate number of wifes
            //stepsByclassName.Add(StepNames.ValidateSponsorWifesCount, DataHelper.GetValidateSponsorWifesCountStepParams());

            //// Save apps step
            //stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            // Resolve documents step
            stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

            // Set document view step
            stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

            // Wait for documents
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

            //// Save documents step
            //stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

            //// Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Set fee view step
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());

            //Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            // Wait for payment status
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            // Mark payment status step
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'categoryId': '58F5CB451952496E809C997C5C574270',
                'serviceId': '94AE97BD791A43729D7F10D9710DAD8A',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                 'applicationId':'0',
                 'jobId':'1434021',
                'sponsorNo': '20120137018043'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();
            //Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            //Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            //Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
            //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

            //context.GlobalParameters["applicationData"] = "{\"Application\":{\"SponsorDetails\":{\"IsSponsorFileOpen\":\"1\"}}}";
            //state = await ins.Resume();


            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"ReligionId\":\"12\",\"ProfessionId\":\"9900130\",\"AddressOutsideCity\":\"511\",\"PassportNo\":\"21324\",\"PassportTypeId\":\"1\",\"PassportIssueDate\":\"2016-4-27\",\"PassportExpiryDate\":\"2019-4-27\",\"PassportPlaceE\":\"ASDASD\",\"PassportIssueCountryId\":\"501\",\"AddressOutsideCountryId\":\"601\",\"AddressOutside1\":\"ASDASD\",\"AddressOutside2\":\"\",\"AddressOutsideTelNo\":\"123123123\",\"FirstNameE\":\"Adsds\",\"MiddleNameE\":\"Adds\",\"LastNameE\":\"Adss\",\"MotherNameE\":\"Aeewwd\",\"CurrentNationalityId\":\"503\",\"BirthDate\":\"2002-2-28\",\"BirthPlaceE\":\"ASD\",\"BirthCountryId\":\"503\",\"MaritalStatusId\":\"2\",\"SexId\":\"2\",\"IsInsideUAE\":\"11BA89BAD4A449DC84725F98F47D1801\",\"Street\":\"ASDASD\",\"POBox\":\"123123\"},\"SponsorDetails\":{\"SponsorRelationId\":\"2\",\"SponsorEmail\":\"faraz.zafar@emaratech.ae\",\"Salary\":\"12312\"},\"ApplicationDetails\":{\"VisaTypeId\":\"3\"},\"AddressOutsideUAE\":{\"City\":\"ASDA\"}}}";
            state = await ins.Resume();

            //Assert.AreEqual(WorkflowStepState.InputRequired, state);
            //Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
            //Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "12", "22" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "pjyK7AAO9y+QRHIcYHRE3j71lQY8k7QzJJdzXA++EimeqZyyzkdF3ZEtGAVLWpxk1+Snb17hhFKHJEFB2vPWvXkY5VeLPN6Snn8xqRGCWg86G5pFW3eFEumkFIb8OQCn6IYfYFNVV1lUkR9WkpbwRXNS8FaeWPl53ni+iDW/TXBVK0AGk9/rBLT8yXdtjWjPPBmBF1Il0iiGO5yUxaBTtjjSSebYVztD4f9OBCiryG+6GtFRkiPSCT5X2/YWgxOxkHLHJiSoxCE3U1lVoWl5CkcDKyaqyiGFzI6WU2sMYBgCFkLy6F2KqKViOQKM/NrIuYfrkB4yIJNSNoahr95MMivARfNPZW+bZND1B78s3PIUfsN3UvIowjUOG2r6PqQhHwGo8vJYSZMTXBzY/oo6U0fYT280MJrSOKdqx7hvD7AB9OiLq3oVqm3Do0L49ndvNXOu4EO0ny8P9jgsCiYgjoFV6b4+y4z9BEN5v7/tamd6YNhWhKDEi0QZYxovWbWRGh7t79W/GCV+EbFPRJxuGzE9FWyucIl9kxkXTgXN4CQ2oLHWnGGh6ul1gMoOowPRyhsnjmEGqeAHtHgp3F/vEzskCoKhwNx09ZIbxp+kG9g2PGHPSK38BGLTEu/sMgola04npLuXBLkuF/Myr1+QRJgPhWfGF65UZ7jl1XKh3oKK40Fdevdlze45WvFVfJht51NxeKyiVXDrNAyxiR+A+qKnDa6ARXuy8cm6zpiuTOsdkNvF0noyWV9e2JGyBbfyFmVQH4P2JkY49mmOqb8yVL8eOh9FKSJIK/YYoOc1rqy9knvayy2hevzCcakqF6O6VlPeHokmpwcodjy9ypCaLXm8XI4590NcxxFgFNIa4rORopO97jflNgUXYGUwNLTFkH6u/4C21UN16CY2I9HsMzmARh1rDEvr0WxZAsc9jpHcuKrn98QPdX0dcCXRJeKZzZ5QYRGZnRuZ8O7fx9A5BdpgOIiKLZxfbaKJGVwDAzFo0UmknifDcL7R0DGXZod4D36BP5VWyO7C4dOUGVktHcM6uQx3sh4qo/1pDFNDT0XGxTaFUsJ8bpSuJKPes/RNq7XLAF1lJ7hJWfPfe2Q4z8XhiqWIzDJVztkQa9LD8ex6jrvY5b74QhIETLwQ/xwX/eBaA0r7dcTkjKqdypL4VilFWvR24I4isV9tI2EJ9ZeT/t68NPmlxB2n3lkqgkLyAoHHoz5o0Re/xILyX7cE/ANdBxLqtqEQABSFGxnZfa/qb4RUSt7dJSY6g5KLJRe+zPzAKaybKlzENwc2RgvZg7PFzaTKY5QDqBVtH4N6bk5cvlEPKcwe34d19jBjyBKdI2OlgaHFignavV4YtFb4yJ9iGrXHiXKYqfARBBH15s8Gf8YPYlfvw+hwW+dlGXGR53Dq2aSgSE7ZVI2GL0+EsImK1o8yWKAl3a6GtKxJxZTRdbYWqUnxZRVgSjJoYykdGCmnFIE8ixESU5dydkE8MJ/KCeFQFwYo3Ak3XRlLWivY9C7DMOiEtuIqYdCRdjkb";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task NewResidenceWorkflow_Success()
        {
            try
            {
                var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

                // Get vision data
                stepsByclassName.Add(StepNames.BuildNewResidenceApplication,
                    DataHelper.GetBuildNewResidenceApplicationStepParams());

                // Resolve form step
                stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

                // Reuse form data step
                stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

                // Set form view step
                stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

                // Wait for application data
                stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

                // Get form data step
                stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

                // Resolve app type
                stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

                // Get travel status
                stepsByclassName.Add(StepNames.GetEntryPermitTravelStatus, DataHelper.GetEntryPermitTravelStatusParams());

                // Set application default values
                stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

                // Validate data
                stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

                //                Validate residence status
                //                stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

                // Validate data
                stepsByclassName.Add(StepNames.ValidateNumberOfYears, DataHelper.GetValidateNumberOfYearsParams());

                // Validate passport info
                //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

                //// Validate inside country
                //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateInsideCountryStepParams());

                // Save apps step
                stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

                // Resolve documents step
                stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

                // Set document view step
                stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

                // Wait for documents
                stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

                // Save documents step
                stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

                // Resolve fees step
                stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

                // Set fee view step
                stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
                ////Pay step
                stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

                //// Wait for payment status
                stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

                //// Mark payment status step
                stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

                // Submit zajel application step
                stepsByclassName.Add(StepNames.SubmitZajelApplication, DataHelper.GetSubmitZajelApplicationStepParams());


                // systemId: eDNRD
                // serviceId: 
                //   - Residence renew family : 745F4FA7B13B49F392BAB81C980DB7E8
                // Res no:
                //   - 20120117235180
                //   - 784197084063821
                string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'categoryId': 'CDF6CD021B0941A18628E7A63F255830',
                'serviceId': '862FC0387B1D4055801B2E2594FE3530',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                'permitNo': '2010311163171',
                'sponsorNo': '20120137018043'
            }";

                //ServicesHelper.Init(null);
                WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

                WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

                Assert.IsNotNull(graph.Steps);

                WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
                DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
                await ins.Initialize();
                WorkflowStepState state = await ins.Execute();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
                Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
                Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
                //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

                context.GlobalParameters["applicationData"] =
                    "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":\"201/2011/3163171\",\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"XX0374910\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null }}}";
                state = await ins.Resume();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
                Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
                //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

                context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "12", "22" });
                state = await ins.Resume();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
                Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
                Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
                //Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

                context.GlobalParameters["request"] = "virtualbank$00000";
                state = await ins.Resume();

                Assert.AreEqual(WorkflowStepState.Done, state);
                Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
            }
            catch (Exception exp)
            {
                Assert.Fail(exp.Message);
            }
        }

        [TestMethod]
        public async Task NewResidenceWorkflow_Failure()
        {
            try
            {
                var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

                // Get vision data
                stepsByclassName.Add(StepNames.BuildNewResidenceApplication,
                    DataHelper.GetBuildNewResidenceApplicationStepParams());

                // Resolve form step
                stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

                // Reuse form data step
                stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

                // Set form view step
                stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

                // Wait for application data
                stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

                // Get form data step
                stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

                // Resolve app type
                stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

                // Get travel status
                stepsByclassName.Add(StepNames.GetEntryPermitTravelStatus, DataHelper.GetEntryPermitTravelStatusParams());

                // Set application default values
                stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

                // Validate data
                stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

                //                Validate residence status
                //                stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

                // Validate data
                stepsByclassName.Add(StepNames.ValidateNumberOfYears, DataHelper.GetValidateNumberOfYearsParams());

                // Validate passport info
                //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

                //// Validate inside country
                //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateInsideCountryStepParams());

                // Save apps step
                stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

                // Resolve documents step
                stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

                // Set document view step
                stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

                // Wait for documents
                stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

                // Save documents step
                stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

                // Resolve fees step
                stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

                // Set fee view step
                stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
                ////Pay step
                stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

                //// Wait for payment status
                stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

                //// Mark payment status step
                stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

                // systemId: eDNRD
                // serviceId: 
                //   - Residence renew family : 745F4FA7B13B49F392BAB81C980DB7E8
                // Res no:
                //   - 20120117235180
                //   - 784197084063821
                string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '862FC0387B1D4055801B2E2594FE3530',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                'permitNo': '2010311163171',
                'sponsorNo': '20120137018043'
            }";

                //ServicesHelper.Init(null);
                WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

                WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

                Assert.IsNotNull(graph.Steps);

                WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
                DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
                await ins.Initialize();
                WorkflowStepState state = await ins.Execute();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
                Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
                Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
                //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

                context.GlobalParameters["applicationData"] =
                    "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"XX0374910\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null }}}";

                Assert.AreNotEqual(WorkflowStepState.Done, state);
            }
            catch (Exception exp)
            {
                Assert.Fail(exp.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task NewResidenceWorkflow_Exception()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Get vision data
            stepsByclassName.Add(StepNames.BuildNewResidenceApplication,
                DataHelper.GetBuildNewResidenceApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Set form view step
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            // Get travel status
            stepsByclassName.Add(StepNames.GetEntryPermitTravelStatus, DataHelper.GetEntryPermitTravelStatusParams());

            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            //                Validate residence status
            //                stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateNumberOfYears, DataHelper.GetValidateNumberOfYearsParams());

            // Validate passport info
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

            //// Validate inside country
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateInsideCountryStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            // Resolve documents step
            stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

            // Set document view step
            //stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

            // Wait for documents
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

            // Save documents step
            stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Set fee view step
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            ////Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            //// Wait for payment status
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            //// Mark payment status step
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // systemId: eDNRD
            // serviceId: 
            //   - Residence renew family : 745F4FA7B13B49F392BAB81C980DB7E8
            // Res no:
            //   - 20120117235180
            //   - 784197084063821
            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '862FC0387B1D4055801B2E2594FE3530',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                'permitNo': '2010304013188',
                'sponsorNo': '20120137018043'
            }";

            //ServicesHelper.Init(null);
            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();

            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
            //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

            state = await ins.Resume();
            object data = context.GlobalParameters["applicationData"].ToObject<object>();

            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "12", "22" });
            state = await ins.Resume();
        }
        [TestMethod]
        public async Task WorkflowEdit_Success()
        {
            try
            {
                var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

                // Get vision data
                stepsByclassName.Add(StepNames.BuildNewResidenceApplication,
                    DataHelper.GetBuildNewResidenceApplicationStepParams());

                // Resolve form step
                stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

                // Reuse form data step
                stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

                // Set form view step
                stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

                // Wait for application data
                stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

                // Get form data step
                stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

                // Resolve app type
                stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

                // Get travel status
                stepsByclassName.Add(StepNames.GetEntryPermitTravelStatus, DataHelper.GetEntryPermitTravelStatusParams());

                // Set application default values
                stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

                // Validate data
                stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

                //                Validate residence status
                //                stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

                // Validate data
                stepsByclassName.Add(StepNames.ValidateNumberOfYears, DataHelper.GetValidateNumberOfYearsParams());

                // Validate passport info
                //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

                //// Validate inside country
                //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateInsideCountryStepParams());

                // Save apps step
                stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

                // Resolve documents step
                stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

                // Set document view step
                stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

                // Wait for documents
                stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

                // Save documents step
                stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

                // Resolve fees step
                stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

                // Set fee view step
                stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
                ////Pay step
                stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

                //// Wait for payment status
                stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

                //// Mark payment status step
                //stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

                // systemId: eDNRD
                // serviceId: 
                //   - Residence renew family : 745F4FA7B13B49F392BAB81C980DB7E8
                // Res no:
                //   - 20120117235180
                //   - 784197084063821
                string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': 'B4D5063747EB42029035E533BBEB426E',
                'categoryId': '58F5CB451952496E809C997C5C574270',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                'permitNo': '2010311163171',
                'sponsorNo': '20120137018043',
                'applicationId':'1128332'
            }";

                //ServicesHelper.Init(null);
                WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);
                
                WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow,new ConditionFactory());

                Assert.IsNotNull(graph.Steps);

                WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
                DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
               await ins.Initialize();
                WorkflowStepState state = await ins.Execute();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
                Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
                Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
                //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

                context.GlobalParameters["applicationData"] =
                    "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Updated\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"XX0374910\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null }}}";
                state = await ins.Resume();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
                Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
                //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

                context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "12", "22" });
                state = await ins.Resume();

                Assert.AreEqual(WorkflowStepState.InputRequired, state);
                Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
                Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
                Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
                Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
                //Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

                context.GlobalParameters["request"] = "virtualbank$000000";
                state = await ins.Resume();

                Assert.AreEqual(WorkflowStepState.Done, state);
                Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
            }
            catch (Exception exp)
            {
                Assert.Fail(exp.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task WorkflowEdit_FailureApplicationIdNotFound()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Get vision data
            stepsByclassName.Add(StepNames.BuildNewResidenceApplication,
                DataHelper.GetBuildNewResidenceApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Set form view step
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            // Get travel status
            stepsByclassName.Add(StepNames.GetEntryPermitTravelStatus, DataHelper.GetEntryPermitTravelStatusParams());

            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            //                Validate residence status
            //                stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateResidenceStatus, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateResidenceStatusStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateNumberOfYears, DataHelper.GetValidateNumberOfYearsParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateInsideCountry, DataHelper.GetValidateInsideCountryStepParams());

            // Validate passport info
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateApplicantPassport, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateApplicantPassportParams());

            //// Validate inside country
            //stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateInsideCountry, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateInsideCountryStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            // Resolve documents step
            stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

            // Set document view step
            stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

            // Wait for documents
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

            // Save documents step
            stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Set fee view step
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            ////Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            //// Wait for payment status
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            //// Mark payment status step
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // systemId: eDNRD
            // serviceId: 
            //   - Residence renew family : 745F4FA7B13B49F392BAB81C980DB7E8
            // Res no:
            //   - 20120117235180
            //   - 784197084063821
            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '862FC0387B1D4055801B2E2594FE3530',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                'establishmentType': '1',
                'permitNo': '2010304013188',
                'sponsorNo': '20120137018043',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'applicationId':'111111'
            }";

            //ServicesHelper.Init(null);
            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());
            //Assert.IsNull(context.GlobalParameters["applicationData"].ToObject<object>());

            context.GlobalParameters["applicationData"] =
                "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"XX0374910\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "12", "22" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "virtualbank$000000";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task OnArrivalVisaExtensionWorkflow_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Get entry permit no based on passport no, birth year and nationality
            stepsByclassName.Add(StepNames.FetchIndividualProfileByPassportInfo, DataHelper.FetchIndividualProfileByPassportInfoParams());

            // Get vision data
            stepsByclassName.Add(StepNames.BuildEntryPermitApplication, DataHelper.GetBuildEntryPermitApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

            // validate entry permit status
            stepsByclassName.Add(StepNames.ValidateEntryPermitStatus, DataHelper.GetValidateEntryPermitStatusParams());

            // Validate violation
            stepsByclassName.Add("Emaratech.Services.Channels.Workflows.Steps.ValidateViolationInfo, Emaratech.Services.Channels.Workflows", DataHelper.GetValidateViolationStepParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            // Resolve documents step
            stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

            // Set document view step
            stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

            // Wait for documents
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

            // Save documents step
            stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Set fee view step
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());

            //Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            // Wait for payment status
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            // Mark payment status step
            //stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // systemId: eDNRD
            // authenticatedSystemId: UnifiedMobileSystemId
            // serviceId: '50C58BE394364950A590A8DD78730B70' on arrival

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'serviceId': '50C58BE394364950A590A8DD78730B70',
                'platform': '00000000000000000000000000000002',
                'email': 'ahmed@aa.com',
                'mobileNumber': '111111111',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                'residenceNo': '20120117002247',
                'sponsorNo': '20120137018043'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"RJ6892321\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\",\"YearOfBirth\":\"1976\",\"CurrentNationalityId\":\"217\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null,\"SponsorTypeId\":2 }}}";
            state = await ins.Resume();

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"RJ6892321\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\",\"YearOfBirth\":\"1976\",\"CurrentNationalityId\":\"217\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null,\"SponsorTypeId\":2 }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "205" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "virtualbank$000000";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task TravelStatusReportByResNo_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            stepsByclassName.Add(StepNames.BuildTravelHistoryReport, DataHelper.GetTravelStatusReportStepParams());
            //20120097160984 -- 20120127176057 -- 20120127094120 -- 20120067185301
            string json = @"{
                 'residenceNo': '20120127176057'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.Done, state);

        }

        [TestMethod]
        public async Task TravelStatusReportByPermitNo_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            stepsByclassName.Add(StepNames.BuildTravelHistoryReport, DataHelper.GetTravelStatusReportStepParams());
            //20120097160984 -- 20120127176057 -- 20120127094120 -- 20120067185301

            string json = @"{
                 'permitNo': '2010314400247'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.Done, state);
        }

        [TestMethod]
        public async Task TransferResidencePassportWorkflow_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();

            // Get entry permit no based on passport no, birth year and nationality
            //stepsByclassName.Add(StepNames.SetTransferResidenceDependentsView, DataHelper.GetBuildTransferResidenceDependentsViewStepParams());
            //stepsByclassName.Add(StepNames.WaitForDependentsView, DataHelper.GetBuildWaitForDependentsViewStepParams());
            stepsByclassName.Add(StepNames.BuildTransfertResidenceNewPassportApplication, DataHelper.GetBuildResidenceApplicationStepParams());

            // Resolve form step
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());

            // Reuse form data step
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());

            // Get form data step
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());

            // Wait for application data
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());

            // Resolve app type
            stepsByclassName.Add(StepNames.ResolveVisaTypeAppType, DataHelper.GetResolveVisaTypeAppTypeStepParams());

            stepsByclassName.Add(StepNames.GetResidenceTravelStatus, DataHelper.GetResidenceTravelStatusParams());
            // Set application default values
            stepsByclassName.Add(StepNames.SetApplicationDefaultValues, DataHelper.GetSetApplicationDefaultValuesStepParams());

            // validate entry permit status
            stepsByclassName.Add(StepNames.ValidateEntryPermitStatus, DataHelper.GetValidateEntryPermitStatusParams());

            // Validate data
            stepsByclassName.Add(StepNames.ValidateApplication, DataHelper.GetValidateApplicationStepParams());

            // Save apps step
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());

            // Resolve documents step
            stepsByclassName.Add(StepNames.ResolveDocuments, DataHelper.GetResolveDocumentsStepParams());

            // Set document view step
            stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());

            // Wait for documents
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());

            // Save documents step
            stepsByclassName.Add(StepNames.SaveDocuments, DataHelper.GetSaveDocumentsStepParams());

            // Resolve fees step
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());

            // Set fee view step
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());

            //Pay step
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());

            // Wait for payment status
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());

            // Mark payment status step
            //stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            // systemId: eDNRD
            // authenticatedSystemId: UnifiedMobileSystemId
            // serviceId: '50C58BE394364950A590A8DD78730B70' on arrival

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'userId': '5FC039517FA04F028BEA4D0EA96FC5DB',
                'serviceId': '8C7139EFB9A84002B155C4E7651613ED',
                'categoryId': '4401980EF5AF42FEB1EE407A6F1BBAB3',
                'platform': '00000000000000000000000000000002',
                'email': 'ahmed@aa.com',
                'mobileNumber': '111111111',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                 'sponsorType':'2',
                 'sponsorSponsorType':'3',
                'establishmentType': '1',
                'residenceNo': '20120117002247',
                'sponsorNo': '20120137018043'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            //Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"RJ6892321\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\",\"YearOfBirth\":\"1976\",\"CurrentNationalityId\":\"217\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null,\"SponsorTypeId\":2 }}}";
            state = await ins.Resume();

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicationDetails\": {\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"}, \"ApplicantDetails\":{ \"VisaNumber\":null,\"VisaExpiryDate\":\"03/03/2017\",\"ResidenceRequestYear\":\"141441352AE944F7966125F4DAD0CFB5\",\"FullNameE\":\"Mehnaz KANWAL SAJID HUSSAIN\",\"FullNameA\":\"سيسي\",\"BirthDate\":\"01/01/1990\",\"BirthPlaceE\":\"DSDS\",\"BirthPlaceA\":\"ييبيب\",\"PassportNo\":\"RJ6892321\",\"PassportTypeId\":\"2\",\"PassportIssueDate\":\"09/08/2007\",\"PassportExpiryDate\":\"17/03/2014\",\"PassportIssueCountryId\":\"101\",\"EmirateId\":\"2\",\"CityId\":\"202\",\"AreaId\":\"153\",\"POBox\":null,\"Street\":\"sdsd\",\"YearOfBirth\":\"1976\",\"CurrentNationalityId\":\"217\"   },  \"SponsorDetails\":{ \"SponsorRelationId\":\"4\",\"SponsorFullNameE\":\"sdsdsd\",\"SponsorFullNameA\":\"سيسيس\",\"MobileNo\":\"09-343434\",\"SponsorEmail\":null,\"SponsorTypeId\":2 }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["applicationData"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["applicationId"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["documentIds"].ToObject<object>());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "205" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            Assert.IsNotNull(context.GlobalParameters["fees"].ToObject<object>());
            Assert.IsNotNull(context.GlobalParameters["amount"].ToObject<long>());
            Assert.IsNotNull(context.GlobalParameters["paymentUrl"].ToObject<string>());
            //Assert.IsNull(context.GlobalParameters["request"].ToObject<object>());

            context.GlobalParameters["request"] = "virtualbank$000000";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.Done, state);
            Assert.IsNotNull(context.GlobalParameters["batchId"].ToObject<string>());
        }

        [TestMethod]
        public async Task LegalAdvice_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());
            stepsByclassName.Add(StepNames.FilterFilterPreferredLanguages, DataHelper.GetFilterPreferredLanguagesParams());
            stepsByclassName.Add(StepNames.ReuseLegalAdviceFormData, DataHelper.GetReuseLegalAdviceFormDataStepParams());
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());
            stepsByclassName.Add(StepNames.SetAnyDocumentView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());
            stepsByclassName.Add(StepNames.SaveLegalAdvice, DataHelper.GetSaveLegalAdviceStepParams());

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '296C48D82D274DDBB2883A6C5D12A866',
                'categoryId': '9AD4B504D7404AA88170FBB84B6418F2',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                'legalAdviceNumber': '813051282',
                'platform': '00000000000000000000000000000002'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{\"ApplicantType\":3,\"FullName\":\"Damir\",\"SexId\":\"1\",\"NationalityId\":403,\"BirthDate\":\"1985-07-01\",\"PassportNo\":\"N12345\",\"AddressOutsideTelNo\":\"3774747474\",\"PreferredLanguage\":\"2\"},\"ApplicationDetails\":{\"AdviceType\":1,\"Comment\":\"Small coment\"},\"SponsorDetails\":{\"SponsorEmail\":\"Damir@fhdj.dk\"}}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("anyDocument", context.GlobalParameters["view"].ToString());
            // 2283
            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "2301" });
            context.GlobalParameters["action"] = "DRAFT";
            state = await ins.Resume();
        }

        [TestMethod]
        public async Task PassportRenew_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            stepsByclassName.Add(StepNames.SetDisclaimerView, DataHelper.GetSetDisclaimerViewParams());
            stepsByclassName.Add(StepNames.WaitForDisclaimerApproval, DataHelper.GetWaitForDisclaimerApprovalParams());

            stepsByclassName.Add(StepNames.PassportRenewGrouping, DataHelper.GetPassportRenewGroupingStepParams());
                        
            stepsByclassName.Add(StepNames.ResolvePreForm, DataHelper.GetResolvePreFormStepParams());
            stepsByclassName.Add(StepNames.GetPassportServicesFormButtons, DataHelper.GetPassportServicesFormButtonsStepParams());
            //stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());
            //stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());
            stepsByclassName.Add(StepNames.GetPassportRenewData3, DataHelper.GetPassportFormDataStepParams());
            stepsByclassName.Add(StepNames.SavePassportRenewRequest, DataHelper.GetSavePassportRequestStepParams());
            stepsByclassName.Add(StepNames.ResolveFeesPassportServices, DataHelper.GetResolveFeesPassportServicesStepParams());
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());
            
            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '365FC64DC8D8433D88B4D4FE445BD1C6',
                'categoryId': '684454CB65D1434EB87A54BE403FC08F',
                'userType': '64A20C7969634A0EBB740B8B83D19C1A',
                'platform': '00000000000000000000000000000002'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("disclaimer", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["disclaimerApproved"] = true;
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{ \"EmiratesIdNo\":\"111222333444555\",\"PassportFullName\":\"Adsds Est\",\"UnifiedNo\":\"123456\" }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "1939" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("list", context.GlobalParameters["view"].ToString());


            context.GlobalParameters["action"] = "edit-" + context.GlobalParameters["listData"][0]["id"];
            state = await ins.Resume();
            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{ \"EmiratesIdNo\":\"211237\",\"PassportFullName\":\"Test 222\",\"UnifiedNo\":\"767676\" }}}";
            state = await ins.Resume();
            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "1939" });
            state = await ins.Resume();


            context.GlobalParameters["action"] = "new";
            state = await ins.Resume();

            /////
            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{ \"EmiratesIdNo\":\"211237\",\"PassportFullName\":\"Test 222\",\"UnifiedNo\":\"767676\" }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "1939" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("list", context.GlobalParameters["view"].ToString());
            //////
            
            context.GlobalParameters["action"] = "next";
            state = await ins.Resume();

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{\"POBox\":\"444444\",\"MobileNo\":\"054-4738988\",\"FullName\":\"Damir\",\"NearestLandmark\":\"Icon Tower\",\"Street\":\"FFF\",\"EmirateId\":\"2\",\"CityId\":\"202\"},\"ApplicationDetails\":{\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"},\"SponsorDetails\":{\"SponsorEmail\":\"Damir@fhdj.dk\"}}}";
            context.GlobalParameters["action"] = "submit";
            state = await ins.Resume();

            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            state = await ins.Resume();
        }

        [TestMethod]
        public async Task PassportRenewSearch_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            stepsByclassName.Add(StepNames.PassportRenewGrouping, DataHelper.GetPassportRenewGroupingStepParams());
            stepsByclassName.Add(StepNames.ResolvePreForm, DataHelper.GetResolvePreFormStepParams());
            stepsByclassName.Add(StepNames.GetPassportServicesFormButtons, DataHelper.GetPassportServicesFormButtonsStepParams());
            stepsByclassName.Add(StepNames.ReusePassportRenewFormData, DataHelper.GetReusePassportFormDataStepParams());
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());
            stepsByclassName.Add(StepNames.GetPassportRenewData3, DataHelper.GetPassportFormDataStepParams());
            stepsByclassName.Add(StepNames.SavePassportRenewRequest, DataHelper.GetSavePassportRequestStepParams());
            stepsByclassName.Add(StepNames.ResolveFeesPassportServices, DataHelper.GetResolveFeesPassportServicesStepParams());
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '365FC64DC8D8433D88B4D4FE445BD1C6',
                'categoryId': '684454CB65D1434EB87A54BE403FC08F',
                'userType': '64A20C7969634A0EBB740B8B83D19C1A',
                'platform': '00000000000000000000000000000002',
                'searchInfo': {
                    'emiratesId': '000999888777666',
                    'unifiedNumber': '000'
                }
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{ \"EmiratesIdNo\":\"111222333444555\",\"PassportFullName\":\"Adsds Est\",\"UnifiedNo\":\"123456\" }}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "1939" });
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("list", context.GlobalParameters["view"].ToString());
           
            context.GlobalParameters["action"] = "next";
            state = await ins.Resume();

            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = "{\"Application\":{\"ApplicantDetails\":{\"POBox\":\"444444\",\"MobileNo\":\"054-4738988\",\"FullName\":\"Damir\",\"NearestLandmark\":\"Icon Tower\",\"Street\":\"FFF\",\"EmirateId\":\"2\",\"CityId\":\"202\"},\"ApplicationDetails\":{\"ResidencyPickup\":\"FAD96F14B47C4A529DD6B40EAD46A0DF\"},\"SponsorDetails\":{\"SponsorEmail\":\"Damir@fhdj.dk\"}}}";
            context.GlobalParameters["action"] = "submit";
            state = await ins.Resume();

            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            state = await ins.Resume();
        }

        [TestMethod]
        public async Task PassportNew_Success()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            //stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());
            //stepsByclassName.Add(StepNames.ReusePassportNewFormData, DataHelper.GetReusePassportFormDataStepParams());
            //stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());
            stepsByclassName.Add(StepNames.ResolveDocumentsPassportNew, DataHelper.GetResolveDocumentsPassportStepParams());
            stepsByclassName.Add(StepNames.GetPassportServicesFormButtons, DataHelper.GetPassportServicesFormButtonsStepParams());
            stepsByclassName.Add(StepNames.SetDocumentView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.WaitForDocuments, DataHelper.GetWaitForDocumentsParams());
            stepsByclassName.Add(StepNames.GetPassportNewData, DataHelper.GetPassportFormDataStepParams());
            stepsByclassName.Add(StepNames.SavePassportNewRequest, DataHelper.GetSavePassportRequestStepParams());
            stepsByclassName.Add(StepNames.ResolveFeesPassportServices, DataHelper.GetResolveFeesPassportServicesStepParams());
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());
            stepsByclassName.Add(StepNames.WaitForPaymentStatus, DataHelper.GetWaitForPaymentStatusParams());
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': 'B38DBB182DBE46FCB9047AFBB5E6D730',
                'categoryId': '684454CB65D1434EB87A54BE403FC08F',
                'userType': '64A20C7969634A0EBB740B8B83D19C1A',
                'platform': '00000000000000000000000000000002',
                'ssearchInfo': {
                    'emiratesId': '567',
                    'unifiedNumber': '999'
                }
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            await ins.Initialize();
            WorkflowStepState state = await ins.Execute();
            
            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            //Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            //Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            //Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["action"] = "submit";
            context.GlobalParameters["applicationData"] = "{\"Application\":{ \"ApplicationDetails\":{\"ResidencyPickup\":\"C7D97783C4F146F281D4AF91B5A61DED\"}, \"ApplicantDetails\":{ \"POBox\":\"444444\",\"PassportFullName\":\"Damir\",\"MobileNo\":\"054-4738988\",\"FullName\":\"Damir\",\"NearestLandmark\":\"Icon Tower\",\"Street\":\"FFF\",\"EmirateId\":\"1\",\"CityId\":\"712\",\"EmiratesIdNo\":\"543887664334437\",\"UnifiedNo\":\"999\",\"FirstNameE\":\"Adsds\",\"MiddleNameE\":\"Adds\",\"LastNameE\":\"Adss\",\"FirstNameA\":\"سبيبيس\",\"MiddleNameA\":\"بيسبيس\",\"LastNameA\":\"سيس\" }, \"SponsorDetails\":{\"SponsorEmail\":\"Damir@fhdj.dk\"}}}";
            state = await ins.Resume();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.AreEqual("document", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["documentIds"] = JToken.FromObject(new List<string> { "1939" });
            state = await ins.Resume();
            
            Assert.AreEqual("fee", context.GlobalParameters["view"].ToString());
            state = await ins.Resume();
        }
    }
}