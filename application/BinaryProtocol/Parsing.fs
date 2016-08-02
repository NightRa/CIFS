namespace BinaryProtocol

[<AutoOpen>]
module Parsing =
    open Utils.Parsing
    open System.Runtime.CompilerServices

    type ParseMonad () =
        member x.Bind<'a,'b> (r : ParsingResult<'a>, map : 'a -> ParsingResult<'b>) : ParsingResult<'b> =
            match r.IsError with
                 | true -> ParsingResult.Error r.ErrorUnsafe
                 | false -> map r.ResultUnsafe
        member x.Return (a : 'a) = ParsingResult.Pure a
        member x.ReturnFrom (a : ParsingResult<'a>) = a
        member x.Zero () = ParsingResult.Error<_> ""

    let parse = new ParseMonad ()

    [<Extension>]
    type ByteArrayExtensions1 () =
        [<Extension>]
        static member Get (this : byte[], index : int) =
            parse {
                if index < 0 || index >= this.Length then
                    return! ParsingResult.Error <| sprintf "Index out of range, index: %d, range: 0..%d" index (this.Length - 1)
                else
                    return this.[index]
            }

    [<Extension>]
    type ByteArrayExtensions2 () =
        [<Extension>]
        static member ToBool (this : byte[], index : int) =
            parse {
                let! b = this.Get index
                match b with
                | 0uy -> return false
                | 1uy -> return true
                | x   -> return! ParsingResult.Error <| sprintf "Boolean value is %d..." x
            }
        [<Extension>]
        static member ToInt64 (this : byte[], index : int) =
            parse {
                let! b0 = this.Get (index + 0)
                let! b1 = this.Get (index + 1)
                let! b2 = this.Get (index + 2)
                let! b3 = this.Get (index + 3)
                let! b4 = this.Get (index + 4)
                let! b5 = this.Get (index + 5)
                let! b6 = this.Get (index + 6)
                let! b7 = this.Get (index + 7)
                return
                    (b0.Int64 <<< 56) +
                    (b1.Int64 <<< 48) +
                    (b2.Int64 <<< 40) +
                    (b3.Int64 <<< 32) +
                    (b4.Int64 <<< 24) +
                    (b5.Int64 <<< 16) +
                    (b6.Int64 <<< 8) +
                    (b7.Int64 <<< 0)
            }
        [<Extension>]
        static member ToInt32 (this : byte[], index : int) =
            parse {
                let! b0 = this.Get (index + 0)
                let! b1 = this.Get (index + 1)
                let! b2 = this.Get (index + 2)
                let! b3 = this.Get (index + 3)
                return
                    (b0.Int32 <<< 24) +
                    (b1.Int32 <<< 16) +
                    (b2.Int32 <<< 8) +
                    (b3.Int32 <<< 0)
            }
    [<Extension>]
    type ByteArrayExtensions3 () =
        [<Extension>]
        static member ToString (this : byte[], index : int) =
            parse {
                let! count = this.ToInt32 index
                let chs = System.Text.UTF8Encoding.UTF8.GetChars (this, index, count)
                return new System.String(chs)
            }
                

