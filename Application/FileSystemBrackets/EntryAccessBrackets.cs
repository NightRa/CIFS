using System.Collections.Generic;
using System.Linq;
using Utils.ArrayUtil;
using Utils.IEnumerableUtil;
using Utils.OptionUtil;
using static Utils.OptionUtil.Opt;
using static System.StringSplitOptions;

namespace FileSystemBrackets
{
    public sealed class EntryAccessBrackets
    {
        public Brackets BracketsUptoParentFolder { get; }
        public Bracket EntryBracket { get; }

        public EntryAccessBrackets(Brackets bracketsUptoParentFolder, Bracket entryBracket)
        {
            this.BracketsUptoParentFolder = bracketsUptoParentFolder;
            this.EntryBracket = entryBracket;
        }

        public Brackets AsBrackets()
        {
            var bracktList = new LinkedList<Bracket>(BracketsUptoParentFolder);
            bracktList.AddLast(EntryBracket);
            return bracktList.AsBrackets();
        }

        public static Option<EntryAccessBrackets> FromPath(string path)
        {
            var brackets = path.Split('\\'.Singleton(), RemoveEmptyEntries);
            var length = brackets.Length;
            if (length == 0)
                return None<EntryAccessBrackets>();
            var access = new EntryAccessBrackets(brackets.Reverse().Tail().Reverse().AsBrackets(), brackets.Last().AsBracket());
            return Some(access);
        }

        public override string ToString()
        {
            return this.AsBrackets().ToString();
        }
    }
}
