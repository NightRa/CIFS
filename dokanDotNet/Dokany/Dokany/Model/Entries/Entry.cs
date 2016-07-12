using DokanNet;

namespace Dokany.Model.Entries
{
    public abstract class Entry
    {
        public abstract FileInformation GetInfo(string name);
    }
}
