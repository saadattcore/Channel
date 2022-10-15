using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using Emaratech.Services.Workflows.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;

namespace Emaratech.Services.Channels.Workflows.Tests.SasReport
{
    [TestClass]
    public class TestCreateSnsReportStep
    {
        private List<WorkflowStepParam> GetCreateSnsReportStepParams()
        {
            return new List<WorkflowStepParam>
                {
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "SystemId", "systemId"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "SponsorNo", "sponsorNo"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "EmiratesId", "emiratesId"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "BirthDate", "birthDate"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Input, "ReportEmailAddress", "reportEmailAddress"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Output, "ReportData", "reportData"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Output, "EmailHeader", "emailHeader"),
                    DataHelper.CreateWorklowStepParam(null, StepParamType.Output, "EmailContent", "emailContent"),
                    DataHelper.CreateWorklowStepParam(null , StepParamType.Reference , "ApplicationDataToSave" , "unifiedApplication")
                };
        }

       // [TestMethod] // Integration test, so commented out.
        public async Task TestExecute()
        {
            var wfStepParams = new Dictionary<string, List<WorkflowStepParam>>();

            wfStepParams.Add(
                "Emaratech.Services.Channels.Workflows.Steps.CreateSnsReport, Emaratech.Services.Channels.Workflows",
                this.GetCreateSnsReportStepParams());

            string json = @"{
                    'systemId': '0E56E4FE722847BCBD4F2569E2C87E14',
                    'sponsorNo': '20120137018043',
                    'emiratesId': '784198175372089',
                    'birthDate': '4/6/1987 6:11:27 PM',
                    'reportEmailAddress' : 'saadat.khan@emaratech.ae'

                }";


            try
            {
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
            catch (Exception e)
            {

            }



           
        }
    }
}