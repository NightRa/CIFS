using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utils.Binary
{
    public static class Hashing
    {
        private static readonly MD5 Md5Hash = MD5.Create();
        public static UInt64 CryptoHash(this byte[] bytes)
        {
            return BitConverter.ToUInt64(Md5Hash.ComputeHash(bytes), 0);
        }
    }
}
