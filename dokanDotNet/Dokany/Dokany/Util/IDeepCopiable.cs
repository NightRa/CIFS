namespace Dokany.Util
{
    public interface IDeepCopiable<out T>
    {
        T DeepCopy();
    }
}
