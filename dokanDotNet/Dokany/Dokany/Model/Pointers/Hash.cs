using System;
using System.Linq;
using Dokany.Util;

namespace Dokany.Model.Pointers
{
    public struct Hash 
    {
        public readonly byte[] bits;

        public Hash(byte[] bits)
        {
            this.bits = bits;
        }
        

        public override string ToString()
        {
            return BitConverter.ToString(bits).Replace("-", string.Empty); ;
        }

        public static Hash Random(int byteLength)
        {
            lock (Global.Rand)
            {
                var bytes = new byte[byteLength];
                Global.Rand.NextBytes(bytes);
                return new Hash(bytes);
            }
        }

        public bool Equals(Hash other)
        {
            return bits.ArrayEquals(other.bits, (b1, b2) => b1 == b2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Hash && Equals((Hash) obj);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var b in bits.Reverse().Take(4))
                hash = (hash << 8) + b;
            return hash;
        }
    }
}
