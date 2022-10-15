using System.Collections.Generic;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using Emaratech.Services.Workflows.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Tests.SasReport
{
    [TestClass]
    public class TestSnsReportWorkflow
    {
         [TestMethod] // Commented out, integration test.
        public async Task TesEntireFlow_IntegrationTest()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            // Resolve form step
            stepsByclassName.Add(StepNames.BuildNewApplication, DataHelper.GetBuildNewApplicationStepParams());
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());
            stepsByclassName.Add(StepNames.ValidateApplicationWithoutAppType, DataHelper.GetValidateDataStepParams());
            stepsByclassName.Add(StepNames.ReadEmailAddressForSasReport, DataHelper.GetReadEmailAddressForSasReportStepParams());
            stepsByclassName.Add(StepNames.CreateSnsReport, DataHelper.GetCreateSnsReportStepParams());
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());
            //stepsByclassName.Add(StepNames.SendEmail, DataHelper.GetSendEmailStepParams());
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());
            stepsByclassName.Add(StepNames.SendEmail, DataHelper.GetSendEmailStepParams());

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'sponsorNo': '20120137018043',
                'emiratesId': '784198175372089',
                'birthDate': '4/6/1987 6:11:27 PM',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '367D5E45BECF4F67B96E8954A3CEEA03',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '230AAC5F9259481F95A92AAC60F5D705',
                'sponsorType': '3',
                'establishmentType': '1',
                'residenceNo': '20120117002247',
                'sponsorNo': '20120137018043',
                'userId': 'F64C71CEF82D43E78DBD2BD6C56502AB'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);
            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow);

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            WorkflowInstance ins = new WorkflowInstance(context, graph);
            ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            // Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = @"{""Application"": {""SponsorDetails"": {""SponsorEmail"": ""zeeshan.azad@emaratech.ae""}}}";

            state = await ins.Resume();
            context.GlobalParameters["request"] = "virtualbank$000000";

            state = await ins.Resume();
            Assert.AreEqual(WorkflowStepState.Done, state);
        }

        [TestMethod]
        public async Task TesEntireFlow_IntegrationTest_SASEstablishment()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            // Resolve form step
            stepsByclassName.Add(StepNames.BuildNewApplication, DataHelper.GetBuildNewApplicationStepParams());
            stepsByclassName.Add(StepNames.ResolveForm, DataHelper.GetResolveFormStepParams());
            stepsByclassName.Add(StepNames.SetFormView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.ReuseFormData, DataHelper.GetReuseDataStepParams());
            stepsByclassName.Add(StepNames.WaitForApplication, DataHelper.GetWaitForApplicationParams());
            stepsByclassName.Add(StepNames.GetFormData, DataHelper.GetFormDataStepParams());
            stepsByclassName.Add(StepNames.ValidateApplicationWithoutAppType, DataHelper.GetValidateDataStepParams());
            stepsByclassName.Add(StepNames.ReadDataEstablishmentSasReport, DataHelper.ReadDataEstablishmentSasReport());
            stepsByclassName.Add(StepNames.CreateEstablishmentSasReport, DataHelper.GetCreateEstablishmentSnsReportStepParams());
            stepsByclassName.Add(StepNames.SaveApplication, DataHelper.GetSaveApplicationStepParams());
            stepsByclassName.Add(StepNames.ResolveFees, DataHelper.GetResolveFeesStepParams());
            stepsByclassName.Add(StepNames.SetFeeView, DataHelper.GetSetViewParams());
            stepsByclassName.Add(StepNames.MakePayment, DataHelper.GetPayStepParams());
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());
            stepsByclassName.Add(StepNames.SendEmail, DataHelper.GetSendEmailEstabSasReportStepParams());


            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'emiratesId': '784198175372089',
                'birthDate': '4/6/1987 6:11:27 PM',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': '14537B0958C14DA998C05D24963DC331',
                'platform': '00000000000000000000000000000002',
                'email': 'rrr@ff.com',
                'mobileNumber': '44444433',
                'userType': '93BECEED174543698C4E3F9A300C1878',
                'sponsorType': '3',
                'establishmentType': '1',
                'residenceNo': '',
                'sponsorNo': '',
                'userId': 'F64C71CEF82D43E78DBD2BD6C56502AB'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);
            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow);

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            WorkflowInstance ins = new WorkflowInstance(context, graph);
            ins.Initialize();
            WorkflowStepState state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.InputRequired, state);
            Assert.IsNotNull(context.GlobalParameters["data"].ToObject<object>());
            // Assert.IsNotNull(context.GlobalParameters["formId"].ToObject<string>());
            Assert.IsNotNull(context.GlobalParameters["formConfiguration"].ToObject<object>());
            Assert.AreEqual("form", context.GlobalParameters["view"].ToString());

            context.GlobalParameters["applicationData"] = @"{""Application"": {""SponsorDetails"": {""SponsorEmail"": ""zeeshan.azad@emaratech.ae"", ""EstablishmentId"":""21145231"" }}}";

            state = await ins.Resume();
            context.GlobalParameters["request"] = "virtualbank$000000";

            state = await ins.Resume();
            Assert.AreEqual(WorkflowStepState.Done, state);
        }
    }
}