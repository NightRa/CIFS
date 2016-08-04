using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utils.ArrayUtil;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.IEnumerableUtil;
using Utils.Parsing;

namespace FileSystemBrackets
{
    public sealed class Brackets : IEnumerable<Bracket>
    {
        private Bracket[] BracketArray { get; }

        public Brackets(IEnumerable<Bracket> brackets)
        {
            this.BracketArray = brackets.ToArray();
        }

        public Bracket this[int index] => BracketArray[index];
        public int Length => BracketArray.Length;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Bracket> GetEnumerator()
        {
            for (int i = 0; i < BracketArray.Length; i++)
                yield return BracketArray[i];
        }

        public override string ToString()
        {
            return BracketArray.MkString("\\");
        }
        public byte[] ToBytes()
        {
            return BracketArray.ToBytes(b => b.ToBytes());
        }

        public static ParsingResult<Brackets> Parse(byte[] bytes, Box<int> index)
        {
            return
                bytes
                .GetArray(index, Bracket.Parse)
                .Map(s => new Brackets(s));
        }

        private bool Equals(Brackets other)
        {
            return BracketArray.ArrayEquals(other.BracketArray, (b1, b2) => b1.Equals(b2));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Brackets && Equals((Brackets) obj);
        }

        public override int GetHashCode()
        {
            return (BracketArray != null ? BracketArray.GetHashCode() : 0);
        }
    }
}
