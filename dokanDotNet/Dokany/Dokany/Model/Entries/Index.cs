using System;
using Dokany.Util;

namespace Dokany.Model.Entries
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
            return obj is Index && Equals((Index) obj);
        }

        public override int GetHashCode()
        {
            return MainFolder.GetHashCode();
        }

        public override string ToString()
        {
            return "Index:" + Environment.NewLine + MainFolder.ToString().AddTabs();
        }
    }
}
