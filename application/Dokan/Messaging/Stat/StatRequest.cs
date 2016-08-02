using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Dokan.Messaging.Stat
{
    public sealed class StatRequest : IBinary
    {
        public static byte TypeNum => 2;
        public string FilePath { get; }

        public StatRequest(string filePath)
        {
            FilePath = filePath;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(FilePath.ToBytes())
                .ToArray();
        }
    }
}
