using System;
using System.Net.Sockets;
using Communication.Messages;
using Utils.DoubleUtil;
using Utils.SocketUtils;

namespace Communication
{
    public sealed class Comunicator
    {
        public string Ip { get; }
        public int Port { get; }
        public Action<string> Log { get; } 

        public Comunicator(string ip, int port, Action<string> log)
        {
            Ip = ip;
            Port = port;
            Log = log;
            Log("Initilized a communicator with for " + ip + " with port " + port);
        }
        
        public MessagingResult SendAndRecieveMessage(byte[] data, TimeSpan timeout)
        {
            try
            {
                Log("Trying to connect to server " + Ip + " at port " + Port);
                TcpClient client = new TcpClient(Ip, Port);
                var socket = client.Client;
                Log("Connection established!");
                Log("Seting write timeout to " + timeout.TotalMilliseconds.ToInt() + "ms");
                socket.SendTimeout = timeout.TotalMilliseconds.ToInt();
                Log("Writing data to stream: " + data.Length + "bytes");
                socket.Send(data);
                socket.Disconnect(false);
                Log("Ended writing");
                Log("Seting read timeout to " + timeout.TotalMilliseconds.ToInt() + "ms");
                socket.ReceiveTimeout = timeout.TotalMilliseconds.ToInt();
                Log("Reading data from stream");
                data = socket.ReadToEnd();
                Log("Ended reading " + data.Length + "bytes");
                client.Close();
                Log("Closing connection");
                return new MessagingResult.Success(data);
            }
            catch (Exception e)
            {
                return new MessagingResult.Failure(e.ToString());
            }
        }
    }
}
