using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Workflows.Steps.SendEmailNs;
using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using Emaratech.Services.Workflows.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Tests.SasReport
{
    [TestClass]
    public class TestSendEmailStep
    {
        private List<WorkflowStepParam> GetSendEmailStepParams()
        {
            return new List<WorkflowStepParam>
            {
                DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "EmailHeader", "emailHeader"),
                DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "EmailContent", "emailContent"),
            };
        }

        // [TestMethod] // Commented out, integration test.
        public async Task TestExecute()
        {
            string deserialized = "{\"DestinationList\":[{\"Address\":\"dejan.novakovic@emaratech.ae\",\"Name\":\"Dejan Novakovic\"}],\"Subject\":\"SAS report\"}";
            var wfStepParams = new Dictionary<string, List<WorkflowStepParam>>();

            wfStepParams.Add("Emaratech.Services.Channels.Workflows.Steps.SendEmail, Emaratech.Services.Channels.Workflows", this.GetSendEmailStepParams());
            
            string jsonTemplate = @"{0}
                    'emailHeader': {1},
                    'emailContent' : 'Zeeshan was here (again)!!!'
                {2}";
            string json = String.Format(jsonTemplate,"{", deserialized, "}");

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(wfStepParams);
            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow);

            Assert.IsNotNull(graph.Steps);
            Assert.AreEqual(1, graph.Steps.Count());

            var context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            var ins = new WorkflowInstance(context, graph);
            ins.Initialize();
            var state = await ins.Execute();

            Assert.AreEqual(WorkflowStepState.Done, state);
        }

        [TestMethod]
        public async Task TestSerializeInputData()
        {
            var emData = EmailHeader.Create("Dejan Novakovic", "dejan.novakovic@emaratech.ae", "SAS report");

            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(emData);

            Assert.IsTrue(serialized.Contains("Address\":\"dejan.novakovic@emaratech.ae"));
            await Task.FromResult(0);
        }

        [TestMethod]
        public async Task TestDeserializeInputData()
        {
            string deserialized = "{\"DestinationList\":[{\"Address\":\"dejan.novakovic@emaratech.ae\",\"Name\":\"Dejan Novakovic\"}],\"Subject\":\"SAS report\"}";
            var inputData = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailHeader>(deserialized);
            
            Assert.AreEqual("SAS report",inputData.Subject);
            var firstDestination = inputData.DestinationList.First();
            Assert.AreEqual("Dejan Novakovic", firstDestination.Name);
            Assert.AreEqual("dejan.novakovic@emaratech.ae", firstDestination.Address);

            await Task.FromResult(0);
        }

    }
}