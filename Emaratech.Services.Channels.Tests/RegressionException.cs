using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emaratech.Services.Channels.Tests
{
    class RegressionException : Exception
    {
        public RegressionException(IRegressionTestStep step, string message)
            : base(message + nameof(step))
        {
        }
    }
}
