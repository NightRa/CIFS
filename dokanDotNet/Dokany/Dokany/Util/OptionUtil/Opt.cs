

namespace Dokany.Util.OptionUtil
{
    public static class Opt
    {
        public static Option<T> None<T>() => new NoneOption<T>();
        public static Option<T> Some<T>(T value) => new SomeOption<T>(value);
    }
}
