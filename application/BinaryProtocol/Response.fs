namespace BinaryProtocol
open Utils.Parsing

type Response =
    | RootHashResponse of hash : byte array
    | FlushResponse 
    | StatResponse of exists : bool * isFolder : bool * length : int64
    | GetInnerEntriesResponse of entries : array<string * int64>
    | ReadFileResponse of path : string
    | WriteFileResponse of path : string
    | CreateFileResponse of path : string
    | CreateFolderResponse of path : string
    | DeleteEntryResponse of path : string
    | DeleteFolderResponse of path : string
    | FollowResponse of hashPath : string
    | CloneResponse of hashPath : string
    member this.FromBytes (bytes : byte array) : Response ParsingResult =
        parse {
            let! firstByte = bytes.Get 0
            match firstByte with
            | 0uy  -> 
                let rest = bytes.[1..]
                return RootHashResponse rest            
            | 1uy  -> return FlushResponse                 
            | 2uy  -> 
                let! exists = bytes.ToBool 1
                let! isFolder = bytes.ToBool 2
                let! length = bytes.ToInt64 3
                return StatResponse (exists, isFolder, length)                
            | 3uy  -> return GetInnerEntriesResponse [|("a.txt", 50L)|]     
            | 4uy  -> return ReadFileResponse ""            
            | 5uy  -> return WriteFileResponse ""           
            | 6uy  -> return CreateFileResponse ""          
            | 7uy  -> return CreateFolderResponse ""        
            | 8uy  -> return DeleteEntryResponse ""         
            | 9uy  -> return DeleteFolderResponse ""        
            | 10uy -> return FollowResponse ""
            | 11uy -> return CloneResponse ""
            | e    -> return! ParsingResult.Error <| sprintf "response type not valid: %d" e
        }