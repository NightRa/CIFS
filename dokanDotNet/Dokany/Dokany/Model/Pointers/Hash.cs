using System;
using System.Linq;
using Dokany.Util;

namespace Dokany.Model.Pointers
{
    public class Hash 
    {
        public byte[] Bits { get; }

        public Hash(byte[] bits)
        {
            Bits = bits;
        }
        

        public override string ToString()
        {
            return BitConverter.ToString(Bits).Replace("-", string.Empty); ;
        }

        public static Hash Random(int byteLength, Random rnd)
        {
            var bytes = new byte[byteLength];
            rnd.NextBytes(bytes);
            return new Hash(bytes);
        }

        protected bool Equals(Hash other)
        {
            return Bits.ArrayEquals(other.Bits, (b1, b2) => b1 == b2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Hash) obj);
        }

        public override int GetHashCode()
        {
            return Bits?.GetHashCode() ?? 0;
        }
    }
}
