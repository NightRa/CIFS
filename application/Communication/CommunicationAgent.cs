using System;
using Communication.Messages;
using Utils.Binary;
using Utils.Parsing;
using static System.Environment;

namespace Communication
{
    public sealed class CommunicationAgent
    {
        private static TimeSpan SendMessageTimeout => TimeSpan.FromSeconds(3);
        private static TimeSpan RecieveMessageTimeout => TimeSpan.FromSeconds(4);
        public Comunicator Comunicator { get; }
        public Action<string> Log { get; }
        public CommunicationAgent(string ip, int port, Action<string> log)
        {
            Comunicator = new Comunicator(ip, port, log);
            Log = log;
        }

        private static readonly object ProtectCode = new object();

        public ParsingResult<TResponse> GetResponse<TRequest, TResponse>(TRequest request,
            Parser<TResponse> parseResponse)
            where TRequest : IBinary
        {
            Log("Writing request");
            lock (ProtectCode)
            {
                var respond = Comunicator.SendAndRecieveMessage(request.ToBytes(), SendMessageTimeout);
                var responseFailure = respond as RecieveMessageResult.RecieveFailure;
                var responseSuccess = respond as RecieveMessageResult.RecieveSuccess;
                if (responseFailure != null)
                    return Parse.Error<TResponse>("Couldn't send message.. " + responseFailure.FailureMessage);
                if (responseSuccess != null)
                    return parseResponse(responseSuccess.Data);
            }
            return Parse.Error<TResponse>("Programming error!!!!! " + NewLine + Environment.StackTrace);
        }
    }
}