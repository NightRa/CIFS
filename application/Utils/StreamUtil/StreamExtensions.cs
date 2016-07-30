using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils.StreamUtil
{
    public static class StreamExtensions
    {
        public static byte[] ReadToEnd(this Stream @this)
        {
            List<byte> data = new List<byte>(1024);
            while (true)
            {
                var readByte = @this.ReadByte();
                if (readByte == -1)
                    return data.ToArray();
                data.Add((byte)readByte);
            }
        }
    }
}
