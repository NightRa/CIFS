using System;

namespace Dokany.Util.OptionUtil
{
    public sealed class NoneOptionException : Exception
    {
        public NoneOptionException(string message) : base(message)
        {
        }
    }
}
