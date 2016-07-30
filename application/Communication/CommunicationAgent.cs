using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Agents;
using Agents.IndexRequest;
using Communication.Messages;
using static System.Text.Encoding;

namespace Communication
{
    public sealed class CommunicationAgent
    {
        private Comunicator Comunicator { get; }
        public Mail<IndexRequestMessage> Inbox { get; }
        public Action<string> Log { get; }
        public CommunicationAgent(string ip, int port, Mail<IndexRequestMessage> inbox, Action<string> log)
        {
            Comunicator = new Comunicator(ip, port, log);
            Inbox = inbox;
            Log = log;
        }

       // public Task<SendMessageResult> Write(by) 
    }
}