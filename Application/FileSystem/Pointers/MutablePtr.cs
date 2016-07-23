using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSystem.Pointers
{
    public sealed class MutablePtr
    {
        public byte[] Bits { get; }

        public MutablePtr(byte[] bits)
        {
            Bits = bits;
        }
    }
}
