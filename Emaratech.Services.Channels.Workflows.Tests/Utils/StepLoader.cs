using System;
using Emaratech.Services.Workflows.Engine.Interfaces;

namespace Emaratech.Services.Channels.Workflows.Tests.Utils
{
    class StepLoader : IStepTypeLoader
    {

        public Type Load(string typeName, string path)
        {
            throw new NotImplementedException();
        }

        public Type Load(string typeName, string path, string workflowId, int version)
        {
            throw new NotImplementedException();
        }
    }
}