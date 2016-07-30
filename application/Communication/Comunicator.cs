using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Communication.Messages;
using Utils.DoubleUtil;
using Utils.StreamUtil;

namespace Communication
{
    internal sealed class Comunicator
    {
        private string Ip { get; }
        private int Port { get; }
        public Action<string> Log { get; } 

        public Comunicator(string ip, int port, Action<string> log)
        {
            Ip = ip;
            Port = port;
            Log = log;
            Log("Initilized a communicator with for " + ip + " with port " + port);
        }

        internal SendMessageResult SendMessage(byte[] data, TimeSpan timeout)
        {
            try
            {
                Log("Trying to connect to server " + Ip + " at port " + Port);
                TcpClient client = new TcpClient(Ip, Port);
                Log("Connected!");
                NetworkStream stream = client.GetStream();
                Log("Seting timeout to " + timeout.TotalMilliseconds.ToInt() + "ms");
                stream.WriteTimeout = timeout.TotalMilliseconds.ToInt();
                Log("Writing data to stream: " + data.Length + "bytes");
                stream.Write(data, 0, data.Length);
                Log("Ended writing");
                stream.Close();
                client.Close();
                Log("Closing connection");
                return new SendMessageResult.SuccessResult();
            }
            catch (Exception e)
            {
                return new SendMessageResult.FailureResult(e.ToString());
            }
        }

        internal RecieveMessageResult RecieveMessage(TimeSpan timeout)
        {
            try
            {
                Log("Trying to connect to server " + Ip + " at port " + Port);
                TcpClient client = new TcpClient(Ip, Port);
                Log("Connected!");
                NetworkStream stream = client.GetStream();
                Log("Seting timeout to " + timeout.TotalMilliseconds.ToInt() + "ms");
                stream.ReadTimeout = timeout.TotalMilliseconds.ToInt();
                Log("Reading data from stream");
                byte[] data = stream.ReadToEnd();
                Log("Ended reading " + data.Length + "bytes");
                stream.Close();
                client.Close();
                Log("Closing connection");
                return new RecieveMessageResult.RecieveSuccess(data);
            }
            catch (Exception e)
            {
                return new RecieveMessageResult.RecieveFailure(e.ToString());
            }
        }
    }
}
