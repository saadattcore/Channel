using System;
using System.ServiceModel.Configuration;

namespace Emaratech.Services.Channels.SSO.Middleware.runtime.behaviors
{
    public class SsoCheckExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new SsoEndpointBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(SsoEndpointBehavior);
            }
        }
    }
}