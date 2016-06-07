using System;

namespace Dokany.Util.OptionUtil
{
    public sealed class SomeOption<T> : Option<T>
    {
        public T ValueUnsafe { get; }
        public bool IsSome => true;
        public bool IsNone => false;

        public SomeOption(T value)
        {
            ValueUnsafe = value;
        }

        public Option<S> Map<S>(Func<T, S> map)
        {
            return new SomeOption<S>(map(ValueUnsafe));
        }

        public S MapOrDefault<S>(Func<T, S> map, S @default)
        {
            return map(ValueUnsafe);
        }

        public Option<S> FlatMap<S>(Func<T, Option<S>> map)
        {
            return map(ValueUnsafe);
        }

        public void Iter(Action<T> action)
        {
            action(ValueUnsafe);
        }
    }
}
