using Dokany.Util;

namespace Dokany.Model.Entries
{
    public sealed class Index : IDeepCopiable<Index>
    {
        public readonly Folder mainFolder;

        public Index(Folder mainFolder)
        {
            this.mainFolder = mainFolder;
        }

        private bool Equals(Index other)
        {
            return mainFolder.Equals(other.mainFolder);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Index && Equals((Index) obj);
        }

        public override int GetHashCode()
        {
            return mainFolder.GetHashCode();
        }

        public Index DeepCopy()
        {
            return new Index(mainFolder.DeepCopy());
        }
    }
}
