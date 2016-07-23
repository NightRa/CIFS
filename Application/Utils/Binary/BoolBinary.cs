using Utils.ArrayUtil;
using Utils.OptionUtil;
using static Utils.OptionUtil.Opt;

namespace Utils.Binary
{
    public static class BoolBinary
    {
        public static byte[] ToBytes(this bool @this)
        {
            byte bit = (byte)(@this ? 1 : 0);
            return bit.Singleton();
        }

        public static Option<bool> FromBytes(this byte[] @this, ref int index)
        {
            switch (@this[index++])
            {
                case 0:
                    return Some(false);
                case 1:
                    return Some(true);
                default:
                    return None<bool>();
            }
        } 
    }
}
