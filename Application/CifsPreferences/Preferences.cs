using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileSystem.Entries;
using Utils;
using Utils.Binary;
using Utils.OptionUtil;
using Utils.Parsing;
using static Constants.Global;
using static Utils.OptionUtil.Opt;

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

        public byte[] ToBytes()
        {
            return OpenOnStartup
                .ToBytes()
                .Concat(DriverChar.ToBytes())
                .ToArray();
        }

        public static ParsingResult<Preferences> Parse(byte[] bytes, Box<int> index)
        {
            return
                bytes
                .ToBool(index)
                .FlatMap(openOnStartup =>
                    bytes
                    .ToChar(index)
                    .Map(driverChar =>
                        new Preferences(openOnStartup, driverChar)));
        }
    }
}
