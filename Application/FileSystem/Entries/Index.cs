using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utils;
using Utils.GeneralUtils;
using Utils.OptionUtil;
using Utils.Parsing;
using Utils.StringUtil;

namespace FileSystem.Entries
{
    public sealed class Index
    {

        public Folder MainFolder { get; }

        public Index(Folder mainFolder)
        {
            this.MainFolder = mainFolder;
        }

        private bool Equals(Index other)
        {
            return MainFolder.Equals(other.MainFolder);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Index && Equals((Index)obj);
        }

        public override int GetHashCode()
        {
            return MainFolder.GetHashCode();
        }

        public override string ToString()
        {
            return "Index:" + Environment.NewLine + MainFolder.ToString().AddTabs();
        }

        public static Index Default()
        {
            return new Index(Folder.Empty);
        }

        public byte[] ToBytes()
        {
            return MainFolder.ToBytes();
        }
        public static ParsingResult<Index> Parse(byte[] bytes, Box<int> index)
        {
            return Folder.Parse(bytes, index).Map(folder => new Index(folder));
        }
    }
}
