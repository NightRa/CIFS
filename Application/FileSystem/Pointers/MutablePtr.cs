using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystemBrackets;
using Utils;
using Utils.ArrayUtil;
using Utils.Binary;
using Utils.Parsing;

namespace FileSystem.Pointers
{
    public sealed class MutablePtr
    {
        public byte[] Bits { get; }

        public MutablePtr(byte[] bits)
        {
            Bits = bits;
        }

        private bool Equals(MutablePtr other)
        {
            return Bits.ArrayEquals(other.Bits, (b1, b2) => b1.Equals(b2));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is MutablePtr && Equals((MutablePtr) obj);
        }

        public override int GetHashCode()
        {
            return (Bits != null ? Bits.GetHashCode() : 0);
        }

        public byte[] ToBytes()
        {
            return Bits.ToBytes();
        }

        public static ParsingResult<MutablePtr> Parse(byte[] bytes, Box<int> index)
        {
            return bytes.ToByteArray(index).Map(b => new MutablePtr(b));
        }
    }
}
