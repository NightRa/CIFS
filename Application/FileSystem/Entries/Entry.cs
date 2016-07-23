using System.Security.AccessControl;
using DokanNet;

namespace FileSystem.Entries
{
    public abstract class Entry
    {
        public abstract FileInformation GetInfo(string name);

        public abstract FileSystemSecurity GetSecurityInfo();
    }
}
