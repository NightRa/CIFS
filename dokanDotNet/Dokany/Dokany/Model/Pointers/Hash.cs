using System;
using System.IO;
using System.Linq;
using DokanNet;
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
            unchecked
            {
                return Math.Abs(bits.Aggregate(1, (acc, @byte) => acc * @byte));
            }
        }
    }
}
