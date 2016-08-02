namespace BinaryProtocol

[<AutoOpen>]
module Utils =
    type System.Byte with
        member x.Int64 = int64 x
        member x.Int32 = int32 x


