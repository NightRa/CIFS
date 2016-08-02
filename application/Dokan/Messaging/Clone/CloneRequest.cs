using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.ArrayUtil;
using Utils.Binary;

namespace Dokan.Messaging.Clone
{
    public sealed class CloneRequest : IBinary
    {
        public static byte TypeNum => 11;
        public string HashPath { get; }

        public CloneRequest(string hashPath)
        {
            HashPath = hashPath;
        }

        public byte[] ToBytes()
        {
            return
                TypeNum
                .Singleton()
                .Concat(HashPath.ToBytes())
                .ToArray();
        }
    }
}
