using System.Collections.Generic;
using System.Linq;
using Utils.ArrayUtil;
using static System.StringSplitOptions;

namespace FileSystemBrackets
{
    public static class BracketsUtils
    {
        public static Bracket AsBracket(this string @this)
        {
            return new Bracket(@this);
        }
        public static Brackets AsBrackets(this string @this)
        {
            return @this.Split('\\'.Singleton(), RemoveEmptyEntries).AsBrackets();
        }
        public static Brackets AsBrackets(this IEnumerable<string> @this)
        {
            return @this.Select(s => s.AsBracket()).AsBrackets();
        }
        public static Brackets AsBrackets(this IEnumerable<Bracket> @this)
        {
            return new Brackets(@this);
        }
    }
}
