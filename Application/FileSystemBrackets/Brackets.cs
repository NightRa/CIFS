using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.IEnumerableUtil;

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
    }
}
