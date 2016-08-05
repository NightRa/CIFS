using Utils.IEnumerableUtil;

namespace Communication.Messages
{
    public abstract class MessagingResult
    {
        public sealed class Failure : MessagingResult
        {
            public string FailureMessage { get; }

            public Failure(string failureMessage)
            {
                FailureMessage = failureMessage;
            }

            public override string ToString()
            {
                return "Failure: " + FailureMessage;
            }
        }

        public sealed class Success : MessagingResult
        {
            public byte[] Data{ get; }
            public Success(byte[] data)
            {
                Data = data;
            }

            public override string ToString()
            {
                return $"Success: " + Data.MkString(",");
            }
        }
    }
}
