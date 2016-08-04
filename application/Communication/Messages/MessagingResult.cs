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
        }

        public sealed class Success : MessagingResult
        {
            public byte[] Data{ get; }
            public Success(byte[] data)
            {
                Data = data;
            }
        }
    }
}
