using System;
using System.Collections.Generic;
using System.Linq;
using Communication.DokanMessaging.CreateFile;
using Communication.DokanMessaging.CreateFolder;
using Communication.DokanMessaging.Delete;
using Communication.DokanMessaging.Flush;
using Communication.DokanMessaging.GetInnerEntries;
using Communication.DokanMessaging.Move;
using Communication.DokanMessaging.ReadFile;
using Communication.DokanMessaging.RootHash;
using Communication.DokanMessaging.Stat;
using Communication.DokanMessaging.WriteFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.ArrayUtil;
using Utils.Binary;
using Utils.RandomUtils;

namespace UnitTests
{
    [TestClass]
    public class MessagingParsing
    {
        private readonly Random Rnd = new Random();
        [TestMethod]
        public void TestCreateFile1()
        {
            var typeNum = ((byte)6).Singleton();
            var doesntExist = ((byte)0).Singleton();
            var bytes = typeNum.Concat(doesntExist).ToArray();
            var parsingResult = CreateFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.DoesParentFolderExist);
            Assert.IsFalse(res.IsNameCollision);
            Assert.IsFalse(res.IsReadOnlyFolder);
        }
        [TestMethod]
        public void TestCreateFile2()
        {
            var typeNum = ((byte)6).Singleton();
            var doesntExist = ((byte)1).Singleton();
            var bytes = typeNum.Concat(doesntExist).ToArray();
            var parsingResult = CreateFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.DoesParentFolderExist);
            Assert.IsFalse(res.IsNameCollision);
            Assert.IsTrue(res.IsReadOnlyFolder);
        }
        [TestMethod]
        public void TestCreateFile3()
        {
            var typeNum = ((byte)6).Singleton();
            var doesntExist = ((byte)2).Singleton();
            var bytes = typeNum.Concat(doesntExist).ToArray();
            var parsingResult = CreateFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.DoesParentFolderExist);
            Assert.IsTrue(res.IsNameCollision);
            Assert.IsFalse(res.IsReadOnlyFolder);
        }
        [TestMethod]
        public void TestCreateFile4()
        {
            var typeNum = ((byte)6).Singleton();
            var doesntExist = ((byte)3).Singleton();
            var bytes = typeNum.Concat(doesntExist).ToArray();
            var parsingResult = CreateFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.DoesParentFolderExist);
            Assert.IsFalse(res.IsNameCollision);
            Assert.IsFalse(res.IsReadOnlyFolder);
        }
        [TestMethod]
        public void TestCreateFolder1()
        {
            var typeNum = ((byte)7).Singleton();
            var ok = ((byte)0).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = CreateFolderResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.IsReadOnly);
            Assert.IsFalse(res.NameCollosion);
        }
        [TestMethod]
        public void TestCreateFolder2()
        {
            var typeNum = ((byte)7).Singleton();
            var ok = ((byte)1).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = CreateFolderResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.IsReadOnly);
            Assert.IsFalse(res.NameCollosion);
        }
        [TestMethod]
        public void TestCreateFolder3()
        {
            var typeNum = ((byte)7).Singleton();
            var ok = ((byte)0).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = CreateFolderResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.IsReadOnly);
            Assert.IsFalse(res.NameCollosion);
        }
        [TestMethod]
        public void TestDelete1()
        {
            var typeNum = ((byte)8).Singleton();
            var ok = ((byte)0).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = DeleteResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.IsReadOnly);
            Assert.IsFalse(res.PathDoesntExist);
        }
        [TestMethod]
        public void TestDelete2()
        {
            var typeNum = ((byte)8).Singleton();
            var ok = ((byte)1).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = DeleteResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.IsReadOnly);
            Assert.IsFalse(res.PathDoesntExist);
        }
        [TestMethod]
        public void TestDelete3()
        {
            var typeNum = ((byte)8).Singleton();
            var ok = ((byte)2).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = DeleteResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.IsReadOnly);
            Assert.IsTrue(res.PathDoesntExist);
        }
        [TestMethod]
        public void TestFlush()
        {
            var typeNum = ((byte)1).Singleton();
            var parsingResult = FlushResponse.Parse(typeNum);
            Assert.IsTrue(parsingResult.IsResult);
        }
        [TestMethod]
        public void TestLs1()
        {
            var typeNum = ((byte)3).Singleton();
            var noSuchEntry = ((byte)1).Singleton();
            var bytes = typeNum.Concat(noSuchEntry).ToArray();
            var parsingResult = GetInnerEntriesResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            Assert.IsFalse(parsingResult.ResultUnsafe.DoesFolderExist);
        }
        [TestMethod]
        public void TestLs2()
        {
            var typeNum = ((byte)3).Singleton();
            var entryExists = ((byte)0).Singleton();
            var numEntries = 1.ToBytes();
            var isFile = ((byte)0).Singleton();
            var isReadOnly = ((byte)1).Singleton();
            var size = 0x11111111111.ToBytes();
            var name = "hello!".ToBytes();
            var bytes =
                typeNum
                .Concat(entryExists)
                .Concat(numEntries)
                .Concat(isFile)
                .Concat(isReadOnly)
                .Concat(size)
                .Concat(name)
                .ToArray();
            var parsingResult = GetInnerEntriesResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            Assert.IsTrue(parsingResult.ResultUnsafe.Infos.Length == 1);
            Assert.IsFalse(parsingResult.ResultUnsafe.Infos[0].IsReadOnly);
            Assert.IsTrue(parsingResult.ResultUnsafe.Infos[0].IsFile);
            Assert.IsFalse(parsingResult.ResultUnsafe.Infos[0].IsFolder);
            Assert.AreEqual(0x11111111111, parsingResult.ResultUnsafe.Infos[0].Length);
            Assert.AreEqual("hello!", parsingResult.ResultUnsafe.Infos[0].Name);
        }
        [TestMethod]
        public void TestMove1()
        {
            var typeNum = ((byte)9).Singleton();
            var ok = ((byte)0).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = MoveResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.SrcDoesntExist);
            Assert.IsFalse(res.SrcOrDesrReadOnly);
        }
        [TestMethod]
        public void TestMove2()
        {
            var typeNum = ((byte)9).Singleton();
            var ok = ((byte)1).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = MoveResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.SrcDoesntExist);
            Assert.IsTrue(res.SrcOrDesrReadOnly);
        }
        [TestMethod]
        public void TestMove3()
        {
            var typeNum = ((byte)9).Singleton();
            var ok = ((byte)2).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = MoveResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.SrcDoesntExist);
            Assert.IsFalse(res.SrcOrDesrReadOnly);
        }
        [TestMethod]
        public void TestReadFile1()
        {
            var typeNum = ((byte)4).Singleton();
            var doesntExist = ((byte)1).Singleton();
            var bytes = typeNum.Concat(doesntExist).ToArray();
            var parsingResult = ReadFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.DoesFileExist);
        }
        [TestMethod]
        public void TestReadFile2()
        {
            var typeNum = ((byte)4).Singleton();
            var exist = ((byte)0).Singleton();
            var data = Rnd.NextBytes(200);
            var bytes =
                typeNum
                .Concat(exist)
                .Concat(data.Length.ToBytes())
                .Concat(data)
                .ToArray();
            var parsingResult = ReadFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.DoesFileExist);
            Assert.IsTrue(data.ArrayEquals(res.BytesRead, (a, b) => a == b));
        }
        [TestMethod]
        public void TestWriteFile1()
        {
            var typeNum = ((byte)5).Singleton();
            var ok = ((byte)0).Singleton();
            var bytes = typeNum.Concat(ok).ToArray();
            var parsingResult = WriteFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.IsWriteSuccess);
            Assert.IsFalse(res.IsFileReadonly);
            Assert.IsFalse(res.FileDoesntExist);
        }
        [TestMethod]
        public void TestWriteFile2()
        {
            var typeNum = ((byte)5).Singleton();
            var readOnly = ((byte)1).Singleton();
            var bytes = typeNum.Concat(readOnly).ToArray();
            var parsingResult = WriteFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.IsWriteSuccess);
            Assert.IsTrue(res.IsFileReadonly);
            Assert.IsFalse(res.FileDoesntExist);
        }
        [TestMethod]
        public void TestWriteFile3()
        {
            var typeNum = ((byte)5).Singleton();
            var doesntExist = ((byte)2).Singleton();
            var bytes = typeNum.Concat(doesntExist).ToArray();
            var parsingResult = WriteFileResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsFalse(res.IsWriteSuccess);
            Assert.IsFalse(res.IsFileReadonly);
            Assert.IsTrue(res.FileDoesntExist);
        }
        [TestMethod]
        public void TestRootHash()
        {
            var typeNum = ((byte) 0).Singleton();
            var rootHashBytes = Rnd.NextBytes(256/8);
            var bytes = typeNum.Concat(rootHashBytes).ToArray();
            var parsingResult = RootHashResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            var res = parsingResult.ResultUnsafe;
            Assert.IsTrue(res.RootHash.ArrayEquals(rootHashBytes, (a, b) => a == b));
        }
        [TestMethod]
        public void TestStat1()
        {
            var typeNum = ((byte)2).Singleton();
            var noSuchEntry = ((byte) 1).Singleton();
            var bytes = typeNum.Concat(noSuchEntry).ToArray();
            var parsingResult = StatResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            Assert.IsFalse(parsingResult.ResultUnsafe.EntryExists);
        }
        [TestMethod]
        public void TestStat2()
        {
            var typeNum = ((byte)2).Singleton();
            var entryExists = ((byte)0).Singleton();
            var isFolder = ((byte) 1).Singleton();
            var isReadOnly = ((byte) 0).Singleton();
            var size = 0x11111111111.ToBytes();
            var bytes =
                typeNum
                .Concat(entryExists)
                .Concat(isFolder)
                .Concat(isReadOnly)
                .Concat(size)
                .ToArray();
            var parsingResult = StatResponse.Parse(bytes);
            Assert.IsTrue(parsingResult.IsResult);
            Assert.IsTrue(parsingResult.ResultUnsafe.EntryExists);
            Assert.IsTrue(parsingResult.ResultUnsafe.IsFolder);
            Assert.IsTrue(parsingResult.ResultUnsafe.IsReadOnly);
        }
    }
}
