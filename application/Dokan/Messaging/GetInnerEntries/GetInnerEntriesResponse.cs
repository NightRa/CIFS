using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dokan.Messaging.CreateFolder;
using FileSystem.Entries;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;
using static Utils.Parsing.Parse;

namespace Dokan.Messaging.GetInnerEntries
{
    public sealed class GetInnerEntriesResponse
    {
        public static byte TypeNum => GetInnerEntriesRequest.TypeNum;
        public Entry.Info[] Infos { get; }
        public bool DoesFolderExist { get; }
        public GetInnerEntriesResponse(Entry.Info[] infos, bool doesFolderExist)
        {
            Infos = infos;
            DoesFolderExist = doesFolderExist;
        }

        public static ParsingResult<GetInnerEntriesResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .FlatMap(num =>
                        num.HasToBe(TypeNum)
                            .FlatMap(_ =>
                                bytes
                                .GetByte(index)
                                .FlatMap(x =>
                                {
                                    if (x == 1)
                                        return Return(new GetInnerEntriesResponse(new Entry.Info[0], false));
                                    else
                                        return
                                            bytes
                                            .GetArray(index, Entry.Info.Parse)
                                            .Map(infos => new GetInnerEntriesResponse(infos, true));
                                })));
        }
    }
}
