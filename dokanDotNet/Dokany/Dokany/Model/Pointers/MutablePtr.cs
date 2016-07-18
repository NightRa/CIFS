
namespace Dokany.Model.Pointers
{
    public class MutablePtr
    {
        public byte[] Bits { get; }

        public MutablePtr(byte[] bits)
        {
            Bits = bits;
        }
    }
}
