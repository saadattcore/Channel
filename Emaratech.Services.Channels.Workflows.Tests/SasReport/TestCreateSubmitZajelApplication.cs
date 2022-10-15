using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Workflows.Contracts.DataAccess.Models;
using Emaratech.Services.Workflows.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Emaratech.Services.Channels.Workflows.Tests.SasReport
{
    [TestClass]
    public class TestCreateSubmitZajelApplication
    {
        private List<WorkflowStepParam> GetCreateSubmitZajelApplication()
        {
            return new List<WorkflowStepParam>
            {
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.SystemId, WorkflowConstants.WorkflowParameterJsonKeys.SystemId),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.ApplicationId, WorkflowConstants.WorkflowParameterJsonKeys.ApplicationId),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.ContactNo, WorkflowConstants.WorkflowParameterJsonKeys.ContactNo),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.Landline, WorkflowConstants.WorkflowParameterJsonKeys.Landline),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.SponsorName, WorkflowConstants.WorkflowParameterJsonKeys.SponsorName),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.Area, WorkflowConstants.WorkflowParameterJsonKeys.Area),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.Address, WorkflowConstants.WorkflowParameterJsonKeys.Address),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.PoBox, WorkflowConstants.WorkflowParameterJsonKeys.PoBox),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.OdrStatus, WorkflowConstants.WorkflowParameterJsonKeys.OdrStatus),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.DeliveryMode, WorkflowConstants.WorkflowParameterJsonKeys.DeliveryMode),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.ApplicationType, WorkflowConstants.WorkflowParameterJsonKeys.ApplicationType),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.FileNo, WorkflowConstants.WorkflowParameterJsonKeys.FileNo),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Input, WorkflowConstants.WorkflowParameterKeys.ProductType, WorkflowConstants.WorkflowParameterJsonKeys.ProductType),
                DataHelper.CreateWorklowStepParam(
                    null, StepParamType.Output, WorkflowConstants.WorkflowParameterKeys.UniqueId, WorkflowConstants.WorkflowParameterJsonKeys.UniqueId),
            };
        }

        [TestMethod]
        public async Task TestExecute()
        {
            var wfStepParams = new Dictionary<string, List<WorkflowStepParam>>();

            wfStepParams.Add(
                "Emaratech.Services.Channels.Workflows.Steps.SubmitZajelApplication, Emaratech.Services.Channels.Workflows",
                this.GetCreateSubmitZajelApplication());

            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.SystemId}': 'D1DCB9A89FB94527811640E345FC5CA7',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.ApplicationId}': '7166FD40672E427D88BB6FF2649468CA',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.ContactNo}': '8849813',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.Landline}': '3813131',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.SponsorName}': 'Popeye',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.Area}': 'Dubai Marina',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.Address}': 'Wall Street, 32',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.PoBox}': '62321',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.OdrStatus}': '26',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.DeliveryMode}': 'Dt',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.ApplicationType}': 'Standard',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.FileNo}': '52df52',");
            sb.Append($"'{WorkflowConstants.WorkflowParameterJsonKeys.ProductType}': 'EntryPermitDouble',");
            sb.Append("}");

            var json = sb.ToString();


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

                var outputUniqueId = context.GlobalParameters[WorkflowConstants.WorkflowParameterJsonKeys.UniqueId].ToObject<string>();
                Assert.IsNotNull(outputUniqueId);

                Assert.AreEqual(WorkflowStepState.Done, state);
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"Failed in createing SubmitZajelApplicationStep with message {ex.Message}");
            }

        }
    }
}