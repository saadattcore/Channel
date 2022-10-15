using System;
using System.Diagnostics.PerformanceData;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Emaratech.Services.Channels.SSO.Middleware;
using NLog;

namespace Emaratech.Services.Channels.SSO.Middleware.runtime.extensions
{
    class SsoOperationInvoker : IOperationInvoker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IOperationInvoker _originalInvoker;
        public delegate object OperationDelegate(object instance, object[] inputs, out object[] outputs);
        private readonly OperationDelegate _delegateInstance;

        public SsoOperationInvoker(IOperationInvoker argOriginalInvoker)
        {
            this._originalInvoker = argOriginalInvoker;
            _delegateInstance = (object instance, object[] inputs, out object[] outputs) =>
            {
                Logger.Trace("Skipping async method invocation");
                outputs = null;
                return null;
            };
        }

        public object[] AllocateInputs()
        {
            Logger.Trace("MyOperationInvoker.AllocateInputs");
            return this._originalInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            object retValue = null;
            Logger.Trace("MyOperationInvoker.Invoke");

            var isSupressed = IsMethodCallSupressed();

            if (isSupressed)
            {
                outputs = null;
            }
            else
            {
                retValue = this._originalInvoker.Invoke(instance, inputs, out outputs);
            }
            return retValue;
        }

        private bool IsMethodCallSupressed()
        {
            bool isSupressCasted = false;
            if (OperationContext.Current.IncomingMessageProperties.ContainsKey(Constants.SupressPropertyName))
            {
                var isSupress = OperationContext.Current.IncomingMessageProperties[Constants.SupressPropertyName];
                isSupressCasted = (isSupress is bool) ? (bool) isSupress : false;
            }
            return isSupressCasted;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            var outputs = new object[] {};
            var isSupressed = IsMethodCallSupressed();
            var retValue = isSupressed ? _delegateInstance.BeginInvoke(instance, inputs, out outputs, null, null) : _originalInvoker.InvokeBegin(instance, inputs, callback, state);
            return retValue;
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            var isSupressed = IsMethodCallSupressed();
            var retValue = isSupressed ? _delegateInstance.EndInvoke(out outputs, result) : _originalInvoker.InvokeEnd(instance, out outputs, result);
            return retValue;
        }

        public bool IsSynchronous => _originalInvoker.IsSynchronous;
    }
}