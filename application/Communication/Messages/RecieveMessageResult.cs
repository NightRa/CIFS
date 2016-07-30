using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication.Messages
{
    public abstract class RecieveMessageResult
    {
        public sealed class RecieveFailure : RecieveMessageResult
        {
            public string FailureMessage { get; }

            public RecieveFailure(string failureMessage)
            {
                FailureMessage = failureMessage;
            }
        }

        public sealed class RecieveSuccess : RecieveMessageResult
        {

            public byte[] Data{ get; }
            public RecieveSuccess(byte[] data)
            {
                Data = data;
            }
        }
    }
}
