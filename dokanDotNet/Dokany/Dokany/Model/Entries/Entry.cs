using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;

namespace Dokany.Model
{
    public abstract class Entry
    {
        public abstract FileInformation GetInfo(string name);
    }
}
