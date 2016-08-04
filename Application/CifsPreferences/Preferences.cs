﻿using System;
using System.Linq;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;
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
            return new Preferences(openOnStartup: false, driverChar: DefaultDriverChar);
        }

        public byte[] ToBytes()
        {
            return
                OpenOnStartup
                .ToBytes()
                .Concat(DriverChar.ToBytes())
                .ToArray();
        }

        public static ParsingResult<Preferences> Parse(byte[] bytes, Box<int> index)
        {
            return
                bytes
                .GetBool(index)
                .FlatMap(openOnStartup =>
                    bytes
                    .GetChar(index)
                    .Map(driverChar =>
                        new Preferences(openOnStartup, driverChar)));
        }

        public override string ToString()
        {
            return $"OpenOnStartup: {OpenOnStartup}" + Environment.NewLine + $"DriverChar: {DriverChar}";
        }

        private bool Equals(Preferences other)
        {
            return OpenOnStartup == other.OpenOnStartup && DriverChar == other.DriverChar;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Preferences && Equals((Preferences) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (OpenOnStartup.GetHashCode()*397) ^ DriverChar.GetHashCode();
            }
        }

        public Preferences WithOpenOnStartup(bool newOpenOnStartup)
        {
            return new Preferences(newOpenOnStartup, this.DriverChar);
        }

        public Preferences WithDriverChar(char newDriverChar)
        {
            return new Preferences(this.OpenOnStartup, newDriverChar);
        }
    }
}
