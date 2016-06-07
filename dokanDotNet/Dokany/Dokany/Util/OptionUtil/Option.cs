using System;

namespace Dokany.Util.OptionUtil
{
    public interface Option<out T>
    {
        bool IsSome { get; }
        bool IsNone { get; }
        T ValueUnsafe { get; }
        Option<S> Map<S>(Func<T, S> map);
        S MapOrDefault<S>(Func<T, S> map, S @default);
        Option<S> FlatMap<S>(Func<T, Option<S>> map);
        void Iter(Action<T> action);
    }
}
