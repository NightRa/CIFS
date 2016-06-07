using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dokany.Util;

namespace Dokany.Model.PathUtils
{
    public sealed class Brackets : IEnumerable<Bracket>
    {
        private readonly Bracket[] brackets;

        public Brackets(IEnumerable<Bracket> brackets)
        {
            this.brackets = brackets.ToArray();
        }

        public Bracket this[int index] => brackets[index];
        public int Length => brackets.Length;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Bracket> GetEnumerator()
        {
            for (int i = 0; i < brackets.Length; i++)
                yield return brackets[i];
        }

        public override string ToString()
        {
            return brackets.MkString("\\");
        }
    }
}
