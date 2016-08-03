using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Communication.Messages;
using Utils.DoubleUtil;

namespace Communication
{
    public sealed class Comunicator
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
        
        public RecieveMessageResult SendAndRecieveMessage(byte[] data, TimeSpan timeout)
        {
            try
            {
                Log("Trying to connect to server " + Ip + " at port " + Port);
                TcpClient client = new TcpClient(Ip, Port);
                var socket = client.Client;
                Log("Connected!");
                Log("Seting timeout to " + timeout.TotalMilliseconds.ToInt() + "ms");
                socket.SendTimeout = timeout.TotalMilliseconds.ToInt();
                Log("Writing data to stream: " + data.Length + "bytes");
                socket.Send(data);
                socket.Disconnect(false);
                Log("Ended writing");
                Log("Seting timeout to " + timeout.TotalMilliseconds.ToInt() + "ms");
                socket.ReceiveTimeout = timeout.TotalMilliseconds.ToInt();
                Log("Reading data from stream");
                data = ReadToEnd(socket);
                Log("Ended reading " + data.Length + "bytes");
                client.Close();
                Log("Closing connection");
                return new RecieveMessageResult.RecieveSuccess(data);
            }
            catch (Exception e)
            {
                return new RecieveMessageResult.RecieveFailure(e.ToString());
            }
        }

        private static byte[] ReadToEnd(Socket socket)
        {
            List<byte> bytes = new List<byte>();
            byte[] buffer = new byte[4 * 1024];
            while (true)
            {
                int amount = socket.Receive(buffer);
                if (amount == 0)
                    return bytes.ToArray();
                bytes.AddRange(buffer.Take(amount));
            }
        }
    }
}
