using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystem.Entries;
using static Constants.Global;

namespace CifsPreferences
{
    public sealed class Preferences
    {
        public bool OpenOnStartup { get; }
        public char DriverChar { get; }

        public Preferences(bool openOnStartup, char driverChar)
        {
            OpenOnStartup = openOnStartup;
            DriverChar = driverChar;
        }

        public static Preferences Default()
        {
            return new Preferences(openOnStartup: true, driverChar: DefaultDriverChar);
        }

        public byte[] AsBytes()
        {
            throw new NotImplementedException();
           // OpenOnStartup
               // .AsBytes()
                //.Concat(DriverChar.AsByte())
        }
    }
}
