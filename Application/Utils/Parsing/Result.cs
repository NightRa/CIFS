using System;

namespace Utils.Parsing
{
    public sealed class Result<TResult> : ParsingResult<TResult>
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

        public override ParsingResult<TOther> Convert<TOther>()
        {
            throw new InvalidOperationException("Cannot convert " + typeof(TResult) + " to " + typeof(TOther));
        }

        public override ParsingResult<TOther> Map<TOther>(Func<TResult, TOther> mapFunc)
        {
            return new Result<TOther>(mapFunc(ResultUnsafe));
        }

        public override ParsingResult<TOther> FlatMap<TOther>(Func<TResult, ParsingResult<TOther>> mapFunc)
        {
            return mapFunc(ResultUnsafe);
        }

        public override string ToString()
        {
            return "Result: " + ResultUnsafe.ToString();
        }
    }
}