using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Emaratech.Services.Channels.SSO.Middleware.runtime.extensions;
using NLog;

namespace Emaratech.Services.Channels.SSO.Middleware.runtime.behaviors
{
    public class SsoEndpointBehavior : IEndpointBehavior
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Validate(ServiceEndpoint endpoint)
        {
            Logger.Trace("MyEndpointBehavior.Validate.");
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            Logger.Trace("MyEndpointBehavior.AddBindingParameters.");
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Logger.Trace("MyEndpointBehavior.ApplyDispatchBehavior.");
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new SsoMessageInspector());
            foreach (var operation in endpointDispatcher.DispatchRuntime.Operations)
            {
                operation.Formatter = new SsoMessageFormatter(operation.Formatter);
                operation.Invoker = new SsoOperationInvoker(operation.Invoker);
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            Logger.Trace("MyEndpointBehavior.ApplyClientBehavior.");
        }
    }
}