using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using NLog;

namespace Emaratech.Services.Channels.SSO.Middleware.runtime.extensions
{
    class SsoMessageFormatter : IDispatchMessageFormatter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        IDispatchMessageFormatter originalFormatter;
        public SsoMessageFormatter(IDispatchMessageFormatter originalFormatter)
        {
            this.originalFormatter = originalFormatter;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            Logger.Trace("MessageFormatter.DeserializeRequest");
            this.originalFormatter.DeserializeRequest(message, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message retValue = null;
            Logger.Trace("MessageFormatter.SerializeReply");

            if (OperationContext.Current.IncomingMessageProperties.ContainsKey(Constants.SupressPropertyName))
            {
                Logger.Trace("Operation invocation has been supressed, so not serialization is taking place");
            }
            else
            {
                retValue = this.originalFormatter.SerializeReply(messageVersion, parameters, result);
            }
            return retValue;

        }
    }
}