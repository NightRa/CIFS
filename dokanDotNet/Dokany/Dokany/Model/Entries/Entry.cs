using System.Security.AccessControl;
using DokanNet;

namespace Dokany.Model.Entries
{
    public abstract class Entry
    {
        public abstract FileInformation GetInfo(string name);

        public abstract FileSystemSecurity GetSecurityInfo();
    }
}
