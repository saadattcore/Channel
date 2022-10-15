using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Emaratech.Services.Channels.Contracts.Errors;
using Emaratech.Services.Channels.Contracts.Rest.Models.Application;
using SwaggerWcf.Attributes;

namespace Emaratech.Services.Channels.Contracts.Rest.Models.Dashboard.Individual
{
    [SwaggerWcfDefinition(Name = "RestDependentsActionsList")]
    [DataContract]
    public class RestDependentActionsList : RestApplicationActions
    {
        [DataMember(Name = "status")]
        public bool Status { get; set; }

        [DataMember(Name = "errorCode")]
        public string ErrorCode { get; set; }

        public string IsInside { get; set; }

        public RestDependentActionsList(string action, string errorCode)
        {
            Action = action;
            if (string.IsNullOrEmpty(errorCode))
                ErrorCode = ConstantMessageCodes.NoActionAllowedDefault;
            else
                ErrorCode = errorCode;
            Status = false;
        }

        public RestDependentActionsList()
        {

        }
    }
}
