using System;

namespace Utils.Parsing
{
    internal sealed class Result<TResult> : ParsingResult<TResult>
    {
        public override bool IsError => false;
        public override TResult ResultUnsafe { get; }

        public override string ErrorUnsafe
        {
            get { throw new InvalidOperationException("Requests error on Result"); }
        }

        public Result(TResult resultUnsafe)
        {
            ResultUnsafe = resultUnsafe;
        }

        public override ParsingResult<TOther> Map<TOther>(Func<TResult, TOther> mapFunc)
        {
            return ParsingResult.Parse(() => mapFunc(ResultUnsafe));
        }

        public override ParsingResult<TOther> FlatMap<TOther>(Func<TResult, ParsingResult<TOther>> mapFunc)
        {
            var value = ParsingResult.Parse(() => mapFunc(ResultUnsafe));
            if (value.IsError)
                return new Error<TOther>(value.ErrorUnsafe);
            return value.ResultUnsafe;
        }

        public override string ToString()
        {
            return ResultUnsafe.ToString();
        }
    }
}