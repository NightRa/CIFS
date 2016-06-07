using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dokany.Util;

namespace Dokany.Model.PathUtils
{
    public struct Bracket : IDeepCopiable<Bracket>
    {
        public string Value { get; }

        public Bracket(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public bool Equals(Bracket other)
        {
            return string.Equals(Value, other.Value);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Bracket && Equals((Bracket) obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public Bracket DeepCopy()
        {
            return new Bracket(Value);
        }
    }
}
