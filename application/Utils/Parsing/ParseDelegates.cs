using Utils.GeneralUtils;

namespace Utils.Parsing
{
    public delegate ParsingResult<TResult> Parser<TResult>(byte[] bytes);
    public delegate ParsingResult<TResult> ParseFunc<TResult>(byte[] bytes, Box<int> box);
    public delegate byte[] EncodeFunc<in TResult>(TResult value);
}
