using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Utils;
using Utils.Binary;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace Dokan.Messaging.CreateFile
{
    public sealed class CreateFileResponse
    {
        public static byte TypeNum => CreateFileRequest.TypeNum;
        public bool IsReadOnlyFolder { get; }
        public bool IsNameCollision { get; }
        public bool DoesParentFolderExist { get; }
        public CreateFileResponse(bool isReadOnlyFolder, bool isNameCollision, bool doesParentFolderExist)
        {
            IsReadOnlyFolder = isReadOnlyFolder;
            IsNameCollision = isNameCollision;
            DoesParentFolderExist = doesParentFolderExist;
        }

        public static ParsingResult<CreateFileResponse> Parse(byte[] bytes)
        {
            var index = new Box<int>(0);
            return
                bytes
                    .GetByte(index)
                    .FlatMap(num =>
                        num.HasToBe(TypeNum)
                            .FlatMap(_ => bytes.GetByte(index)
                                .Map(x =>
                                {
                                    bool isReadOnly = x == 1;
                                    bool isNameCollision = x == 2;
                                    bool doesParentFolderExist = x == 3;
                                    return new CreateFileResponse(isReadOnly, isNameCollision, doesParentFolderExist);
                                })));
        }
    }
}
