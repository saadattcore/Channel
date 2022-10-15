using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using Emaratech.Services.Workflows.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Tests
{
    [TestClass]
    public class ServiceHelperTests
    {
        [TestInitialize]
        public void LoadWorkflowContext()
        {
            var stepsByclassName = new Dictionary<string, List<WorkflowStepParam>>();
            stepsByclassName.Add(StepNames.MarkPaymentStatus, DataHelper.GetMarkPaymentStatusStepParams());

            string json = @"{
                'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                'authenticatedSystemId': '08867038D3934BCA804CD4074735B260',
                'serviceId': 'FB6820583AFD4EF0AB5A5DB9DB289603',
                'platform': '00000000000000000000000000000002',
                'mobileNumber': '44444433',
                'email': 'rrr@ff.com',
                'residenceNo': '20120117002247'
            }";

            WorkflowModel workflow = DataHelper.BuildSimpleWorkflow(stepsByclassName);

            WorkflowGraph graph = new WorkflowGraph(new WorkflowStepFactory(new StepLoader()), workflow, new ConditionFactory());

            Assert.IsNotNull(graph.Steps);

            WorkflowContext context = new WorkflowContext { GlobalParameters = JObject.Parse(json) };
            DefaultWorkflowInstance ins = new DefaultWorkflowInstance(context, graph);
            ins.Initialize().Wait();


        }
    }
}
