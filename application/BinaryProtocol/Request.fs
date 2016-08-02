namespace BinaryProtocol

type Request =
    | RootHashRequest
    | FlushRequest
    | StatRequest of path : string
    | GetInnerEntriesRequest of path : string
    | ReadFileRequest of path : string
    | WriteFileRequest of path : string
    | CreateFileRequest of path : string
    | CreateFolderRequest of path : string
    | DeleteEntryRequest of path : string
    | DeleteFolderRequest of path : string
    | FollowRequest of hashPath : string
    | CloneRequest of hashPath : string
        member this.RequestType : byte =
            match this with
            | RootHashRequest           -> 0uy
            | FlushRequest              -> 1uy
            | StatRequest _             -> 2uy
            | GetInnerEntriesRequest _  -> 3uy
            | ReadFileRequest _         -> 4uy
            | WriteFileRequest _        -> 5uy
            | CreateFileRequest _       -> 6uy
            | CreateFolderRequest _     -> 7uy
            | DeleteEntryRequest _      -> 8uy
            | DeleteFolderRequest _     -> 9uy
            | FollowRequest _           -> 10uy
            | CloneRequest _            -> 11uy
            
        member this.ToBytes : byte array =
            [|this.RequestType|]
            |> Array.append 
                (
                match this with
                | _ -> [||]
                )
