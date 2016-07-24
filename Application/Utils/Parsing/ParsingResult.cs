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

        public abstract ParsingResult<TOther> Map<TOther>(Func<TResult, TOther> mapFunc);
        public abstract ParsingResult<TOther> FlatMap<TOther>(Func<TResult, ParsingResult<TOther>> mapFunc);


    }

    public static class ParsingResult
    {
        public static ParsingResult<TResult> Parse<TResult>(Func<TResult> parse)
        {
            try
            {
                return new Result<TResult>(parse());
            }
            catch (IndexOutOfRangeException e)
            {
                return new Error<TResult>("Parsing method exceeded from array:" + NewLine + e);
            }
            catch (Exception e)
            {
                return new Error<TResult>("An unexpected exception while parsing:" + NewLine + e);
            }
        }

        public static ParsingResult<TResult> Error<TResult>(string error)
        {
            return new Error<TResult>(error);
        }

        public static ParsingResult<TResult> Pure<TResult>(TResult result) => new Result<TResult>(result);
        public static ParsingResult<TResult> Flatten<TResult>(this ParsingResult<ParsingResult<TResult>> @this)
        {
            return @this.FlatMap(x => x);
        }
        public static ParsingResult<IEnumerable<TResult>> Flatten<TResult>(this IEnumerable<ParsingResult<TResult>> @this)
        {
            var values = @this.ToArray();
            if (values.Any(pr => pr.IsError))
            {
                var errorValue = values.First(pr => pr.IsError).ErrorUnsafe;
                return Error<IEnumerable<TResult>>("Array parsing error of element: " + NewLine + errorValue);
            }
            return Pure(values.Select(pr => pr.ResultUnsafe));
        }
    }
}
 