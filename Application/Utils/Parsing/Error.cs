using System;

namespace Utils.Parsing
{
    public sealed class Error<TResult> : ParsingResult<TResult>
    {
        public override bool IsError => true;
        public override TResult ResultUnsafe
        {
            get { throw new InvalidOperationException("Requests error on Result"); }
        }

        public override string ErrorUnsafe { get; }

        public Error(string errorUnsafe)
        {
            ErrorUnsafe = errorUnsafe;
        }

        public override ParsingResult<TOther> Map<TOther>(Func<TResult, TOther> mapFunc)
        {
            return new Error<TOther>(ErrorUnsafe);
        }
        public override ParsingResult<TOther> FlatMap<TOther>(Func<TResult, ParsingResult<TOther>> mapFunc)
        {
            return new Error<TOther>(ErrorUnsafe);
        }

        public override string ToString()
        {
            return "Error" + ErrorUnsafe;
        }
        
        public override ParsingResult<TOther> Convert<TOther>()
        {
            return new Error<TOther>(ErrorUnsafe);
        }


    }
}