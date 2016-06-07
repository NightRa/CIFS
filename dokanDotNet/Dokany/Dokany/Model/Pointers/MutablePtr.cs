
namespace Dokany.Model.Pointers
{
    public struct MutablePtr
    {
        public readonly byte[] bits;

        public MutablePtr(byte[] bits)
        {
            this.bits = bits;
        }
    }
}
