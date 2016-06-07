using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokany.Util.OptionUtil
{
    public sealed class NoneOptionException : Exception
    {
        public NoneOptionException(string message) : base(message)
        {
        }
    }
}
