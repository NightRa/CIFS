

using System.Runtime.InteropServices.ComTypes;

namespace Dokany.Util.OptionUtil
{
    public static class Opt
    {
        public static Option<T> None<T>() => new NoneOption<T>();
        public static Option<T> Some<T>(T value) => new SomeOption<T>(value);

        public static T OrElse<T>(this Option<T> @this, T @else)
        {
            if (@this.IsNone)
                return @else;
            return @this.ValueUnsafe;
        }
    }
}
