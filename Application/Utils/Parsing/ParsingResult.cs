using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using static System.Environment;

namespace Utils.Parsing
{
    public abstract class ParsingResult<TResult>
    {
        public abstract bool IsError { get; }
        public bool IsResult => !IsError;
        public abstract TResult ResultUnsafe { get; }
        public abstract string ErrorUnsafe { get; }

        public abstract ParsingResult<TOther> Convert<TOther>();
        public abstract ParsingResult<TOther> Map<TOther>(Func<TResult, TOther> mapFunc);
        public abstract ParsingResult<TOther> FlatMap<TOther>(Func<TResult, ParsingResult<TOther>> mapFunc);


    }

}
 