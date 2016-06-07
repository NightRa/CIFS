using System.Collections.Generic;
using System.Linq;
using Dokany.Util;
using Dokany.Util.OptionUtil;
using static System.StringSplitOptions;
using static Dokany.Util.OptionUtil.Opt;

namespace Dokany.Model.PathUtils
{
    public sealed class EntryAccessBrackets
    {
        public readonly Brackets bracketsUptoParentFolder;
        public readonly Bracket entryBracket;

        public EntryAccessBrackets(Brackets bracketsUptoParentFolder, Bracket entryBracket)
        {
            this.bracketsUptoParentFolder = bracketsUptoParentFolder;
            this.entryBracket = entryBracket;
        }

        public Brackets AsBrackets()
        {
            var bracktList = new LinkedList<Bracket>(bracketsUptoParentFolder);
            bracktList.AddLast(entryBracket);
            return bracktList.AsBrackets();
        }

        public static Option<EntryAccessBrackets> FromPath(string path)
        {
            var brackets = path.Split("\\".Singleton(), RemoveEmptyEntries);
            var length = brackets.Length;
            if (length == 0)
                return None< EntryAccessBrackets>();
            var access = new EntryAccessBrackets(brackets.RevTail().AsBrackets(), brackets.Last().AsBracket());
            return Some(access);
        } 

        public override string ToString()
        {
            return this.AsBrackets().ToString();
        }
    }
}
