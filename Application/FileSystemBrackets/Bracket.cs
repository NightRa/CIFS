using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileSystemBrackets
{
    public sealed class Bracket
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
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Bracket)obj);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}
