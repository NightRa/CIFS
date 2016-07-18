﻿namespace Dokany.Model.PathUtils
{
    public class Bracket
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

        protected bool Equals(Bracket other)
        {
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Bracket) obj);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}
