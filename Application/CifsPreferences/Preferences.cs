using System.Linq;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;
using static System.Environment;
using static Constants.Global;

namespace CifsPreferences
{
    public sealed class Preferences
    {
        public bool OpenOnStartup { get; }
        public char DriverChar { get; }
        public string IndexIp { get; }
        public Preferences(bool openOnStartup, char driverChar, string indexIp)
        {
            OpenOnStartup = openOnStartup;
            DriverChar = driverChar;
            IndexIp = indexIp;
        }

        public static Preferences Default()
        {
            return new Preferences(openOnStartup: false, driverChar: DefaultDriverChar, indexIp: "127.0.0.1");
        }

        private bool Equals(Preferences other)
        {
            return OpenOnStartup == other.OpenOnStartup && DriverChar == other.DriverChar && string.Equals(IndexIp, other.IndexIp);
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
                var hashCode = OpenOnStartup.GetHashCode();
                hashCode = (hashCode*397) ^ DriverChar.GetHashCode();
                hashCode = (hashCode*397) ^ (IndexIp != null ? IndexIp.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"OpenOnStartup: {OpenOnStartup}," + NewLine +
                   $"DriverChar: {DriverChar}," + NewLine + 
                   $"IndexIp: {IndexIp}";
        }

        public byte[] ToBytes()
        {
            return
                OpenOnStartup
                .ToBytes()
                .Concat(DriverChar.ToBytes())
                .Concat(IndexIp.ToBytes())
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
                    .FlatMap(driverChar =>
                        bytes
                        .GetString(index)
                        .Map(indexIp =>
                        new Preferences(openOnStartup, driverChar, indexIp))));
        }
        
        public Preferences WithOpenOnStartup(bool newOpenOnStartup)
        {
            return new Preferences(newOpenOnStartup, this.DriverChar, this.IndexIp);
        }

        public Preferences WithDriverChar(char newDriverChar)
        {
            return new Preferences(this.OpenOnStartup, newDriverChar, this.IndexIp);
        }
        public Preferences WithIp(string newIp)
        {
            return new Preferences(this.OpenOnStartup, this.DriverChar, newIp);
        }


    }
}
