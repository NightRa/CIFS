using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.Messages
{
    public abstract class SendMessageResult
    {
        private SendMessageResult() { }

        public sealed class FailureResult : SendMessageResult
        {
             public string FailureMessage { get; }

            public FailureResult(string failureMessage)
            {
                FailureMessage = failureMessage;
            }
        }
        public sealed class SuccessResult : SendMessageResult
        {}
    }
}
