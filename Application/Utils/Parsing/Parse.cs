using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Parsing
{
    public delegate ParsingResult<T> ParseFunc<T>(byte[] bytes, Box<int> box);
    public delegate byte[] EncodeFunc<in T>(T value);
}
