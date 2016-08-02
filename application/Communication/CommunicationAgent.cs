using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Agents;
using Agents.IndexRequest;
using Communication.Messages;
using Utils.Binary;
using Utils.Parsing;
using static System.Environment;
using static System.Text.Encoding;

namespace Communication
{
    public sealed class CommunicationAgent
    {
        private static TimeSpan SendMessageTimeout => TimeSpan.FromSeconds(0.1);
        private static TimeSpan RecieveMessageTimeout => TimeSpan.FromSeconds(4);
        private Comunicator Comunicator { get; }
        public Action<string> Log { get; }
        public CommunicationAgent(string ip, int port, Action<string> log)
        {
            Comunicator = new Comunicator(ip, port, log);
            Log = log;
        }

        private static readonly object ProtectCode = new object();
        public ParsingResult<TResponse> GetResponse<TRequest, TResponse>(TRequest request, Parser<TResponse> parseResponse)
            where TRequest : IBinary
        {
            Log("Writing request");
            lock (ProtectCode)
            {
                var result = Comunicator.SendMessage(request.ToBytes(), SendMessageTimeout);
                var failureResult = result as SendMessageResult.FailureResult;
                var successResult = result as SendMessageResult.SuccessResult;
                if (failureResult != null)
                    return Parse.Error<TResponse>("Couldn't send message.. " + failureResult.FailureMessage);
                if (successResult != null)
                {
                    var response = Comunicator.RecieveMessage(RecieveMessageTimeout);
                    var responseFailure = response as RecieveMessageResult.RecieveFailure;
                    var responseSuccess = response as RecieveMessageResult.RecieveSuccess;
                    if (responseFailure != null)
                        return Parse.Error<TResponse>("Couldn't send message.. " + responseFailure.FailureMessage);
                    if (responseSuccess != null)
                        return parseResponse(responseSuccess.Data);
                }
            }
            return Parse.Error<TResponse>("Programming error!!!!! " + NewLine + Environment.StackTrace);
        }
    }
}