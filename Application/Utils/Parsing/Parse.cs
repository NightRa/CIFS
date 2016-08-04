using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Parsing
{
    public static class Parse
    {
        public static ParsingResult<TResult> Return<TResult>(TResult result)
        {
            return new Result<TResult>(result);
        }
        public static ParsingResult<TResult> Error<TResult>(string error)
        {
            return new Error<TResult>(error);
        }
        public static ParsingResult<IEnumerable<TResult>> Flatten<TResult>(this IEnumerable<ParsingResult<TResult>> @this)
        {
            List<TResult> results = new List<TResult>();

            foreach (var parse in @this)
            {
                if (parse.IsResult)
                    results.Add(parse.ResultUnsafe);
                else
                    return parse.Convert<IEnumerable<TResult>>();
            }
            return Return(results.AsEnumerable());
        }
    }
}
