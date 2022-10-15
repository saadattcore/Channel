using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Emaratech.Services.Channels.Contracts.Rest;
using Emaratech.Services.Channels.Contracts.Rest.Models;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using Emaratech.Services.WcfCommons.Client;
using Emaratech.Services.Workflows.Api;
using log4net;
using Newtonsoft.Json;
using NLog.Internal;
using RestSharp;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using System.Threading;

namespace Emaratech.Services.Channels
{
    public class Handler : IHttpHandler
    {
        private readonly IServiceFactory factory;
        private static readonly ILog Log = LogManager.GetLogger(typeof(Handler));

        public Handler(IServiceFactory factory)
        {
            this.factory = factory;
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var payData = new RestPayApplication()
                {
                    Request = context.Request["request"],
                    WorkflowToken = context.Request["workflow"]
                };
                var workflowApi = factory.GetWorkflowsApi();
                var accessToken = workflowApi.GetInstanceItem(payData.WorkflowToken, "accessToken");

                Log.Debug($"Going to call channel api rest client");

                RestSharp.IRestClient client = new RestClient(ConfigurationManager.AppSettings["ChannelApi"] + "/services/pay/noqodi");
                var rr = new RestRequest();
                rr.Method = Method.POST;
                rr.AddHeader("Authorization", "Bearer " + accessToken);
                rr.AddParameter("application/json", JsonConvert.SerializeObject(payData), ParameterType.RequestBody);
                var result = client.Execute<RestPaymentResponse>(rr).Data;

                Log.Debug($"Payment URL received is {result.PaymentCompleteUrl}");

                context.Response.Redirect(result.PaymentCompleteUrl, true);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Log.Error(e);
                context.Response.Redirect(ConfigurationManager.AppSettings["Payments.FailureUrl"], true);
            }

        }

        public bool IsReusable => false;
    }
}