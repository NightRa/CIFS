
using Dokany.Model.PathUtils;
using Dokany.Util;

namespace Dokany.Model.Pointers
{
    public struct RemotePath :IDeepCopiable<RemotePath>
    {
        public readonly MutablePtr root;
        public readonly Brackets path;

        public RemotePath(MutablePtr root, Brackets path)
        {
            this.root = root;
            this.path = path;
        }

        public RemotePath DeepCopy()
        {
            return this;
        }
    }
}
