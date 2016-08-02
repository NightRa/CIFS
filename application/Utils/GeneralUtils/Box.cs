namespace Utils.GeneralUtils
{
    public sealed class Box<T>
    {
        public T Value { get; set; }

        public Box(T value)
        {
            Value = value;
        }
    }
}
