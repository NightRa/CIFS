using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Utils.SocketUtils
{
    public static class SocketExtensions
    {
        public static byte[] ReadToEnd(this Socket socket)
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
