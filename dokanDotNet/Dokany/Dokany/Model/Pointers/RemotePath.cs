
using Dokany.Model.PathUtils;

namespace Dokany.Model.Pointers
{
    public struct RemotePath
    {
        public MutablePtr Root { get; }
        public Brackets Path { get; }

        public RemotePath(MutablePtr root, Brackets path)
        {
            Root = root;
            Path = path;
        }
    }
}
