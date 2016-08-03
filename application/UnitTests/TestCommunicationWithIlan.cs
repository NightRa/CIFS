using System;
using System.Linq;
using Communication;
using Communication.DokanMessaging.Clone;
using Communication.DokanMessaging.CreateFile;
using Communication.DokanMessaging.CreateFolder;
using Communication.DokanMessaging.Delete;
using Communication.DokanMessaging.Flush;
using Communication.DokanMessaging.Follow;
using Communication.DokanMessaging.GetInnerEntries;
using Communication.DokanMessaging.Move;
using Communication.DokanMessaging.ReadFile;
using Communication.DokanMessaging.RootHash;
using Communication.DokanMessaging.Stat;
using Communication.DokanMessaging.WriteFile;
using Communication.Messages;
using Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.ArrayUtil;
using Utils.Parsing;
using Utils.StringUtil;

namespace UnitTests
{
    [TestClass]
    public class TestCommunicationWithIlan
    {
        private CommunicationAgent communicator = new CommunicationAgent("77.138.132.84", 8008, _ => { });

        [TestMethod]
        public void TestCreateFile()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var request = new CreateFileRequest(path);
            var maybeResponse = communicator.GetResponse(request, CreateFileResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsFalse(maybeResponse.ResultUnsafe.PathToParentDoesntExist);
            Assert.IsTrue(maybeResponse.ResultUnsafe.IsNameCollision);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsReadOnlyFolder);
        }
        [TestMethod]
        public void TestCreateFile3()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var request = new CreateFileRequest(path);
            var maybeResponse = communicator.GetResponse(request, CreateFileResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsTrue(maybeResponse.ResultUnsafe.PathToParentDoesntExist);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsNameCollision);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsReadOnlyFolder);
        }

        [TestMethod]
        public void TestCreateFolder()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var request = new CreateFolderRequest(path);
            var maybeResponse = communicator.GetResponse(request, CreateFolderResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsTrue(maybeResponse.ResultUnsafe.IsReadOnly);
            Assert.IsFalse(maybeResponse.ResultUnsafe.NameCollosion);
        }
        [TestMethod]
        public void TestCreateFolder2()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var request = new CreateFolderRequest(path);
            var maybeResponse = communicator.GetResponse(request, CreateFolderResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsReadOnly);
            Assert.IsFalse(maybeResponse.ResultUnsafe.NameCollosion);
            Assert.IsTrue(maybeResponse.ResultUnsafe.PathToParentDoesntExist);
        }

        [TestMethod]
        public void TestDelete()
        {
            var path = @"\Users\Yuval\אזגבגדגדכדגכ\CIFS\CIFS\icon.ico";
            var request = new DeleteRequest(path);
            var maybeResponse = communicator.GetResponse(request, DeleteResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsReadOnly);
            Assert.IsTrue(maybeResponse.ResultUnsafe.PathDoesntExist);
        }

        [TestMethod]
        public void TestFlush()
        {
            var request = new FlushRequest();
            var maybeResponse = communicator.GetResponse(request, FlushResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
        }

        [TestMethod]
        public void TestLs()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var request = new GetInnerEntriesRequest(path);
            var maybeResponse = communicator.GetResponse(request, GetInnerEntriesResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsTrue(maybeResponse.ResultUnsafe.DoesFolderExist);
            Assert.AreEqual(1, maybeResponse.ResultUnsafe.Infos.Length);
            Assert.IsTrue(maybeResponse.ResultUnsafe.Infos[0].IsFile);
            Assert.IsFalse(maybeResponse.ResultUnsafe.Infos[0].IsFolder);
            Assert.IsTrue(maybeResponse.ResultUnsafe.Infos[0].IsReadOnly);
            Assert.AreEqual(666, maybeResponse.ResultUnsafe.Infos[0].Length);
            Assert.AreEqual("אאאaaa", maybeResponse.ResultUnsafe.Infos[0].Name);
        }

        [TestMethod]
        public void TestMove()
        {
            var from = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var to = @"C:\Users\דגכגדכגדכ\Programming\CIFS\CIFS\icon.ico";
            var request = new MoveRequest(from, to);
            var maybeResponse = communicator.GetResponse(request, MoveResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsFalse(maybeResponse.ResultUnsafe.SrcDoesntExist);
            Assert.IsTrue(maybeResponse.ResultUnsafe.SrcOrDesrReadOnly);
        }

        [TestMethod]
        public void TestRead()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            long offset = 111888111888;
            var amountToRead = 8000;
            var request = new ReadFileRequest(path, offset, amountToRead);
            var maybeResponse = communicator.GetResponse(request, ReadFileResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsTrue(maybeResponse.ResultUnsafe.DoesFileExist);
            Assert.IsTrue(maybeResponse.ResultUnsafe.BytesRead.ArrayEquals(new byte[]{1, 7, 9}, (a, b) => a==b));
        }

        [TestMethod]
        public void TestWrite()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var bytes = new byte[] {7, 10, 100};
            long offset = 222333444555;
            var request = new WriteFileRequest(path, bytes, offset);
            var maybeResponse = communicator.GetResponse(request, WriteFileResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsTrue(maybeResponse.ResultUnsafe.IsWriteSuccess);
            Assert.IsFalse(maybeResponse.ResultUnsafe.FileDoesntExist);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsFileReadonly);
        }

        [TestMethod]
        public void TestStat()
        {
            var path = @"C:\Users\Yuval\Programming\CIFS\CIFS\icon.ico";
            var request = new StatRequest(path);
            var maybeResponse = communicator.GetResponse(request, StatResponse.Parse);
            Assert.IsTrue(maybeResponse.IsResult);
            Assert.IsTrue(maybeResponse.ResultUnsafe.EntryExists);
            Assert.IsTrue(maybeResponse.ResultUnsafe.IsFile);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsFolder);
            Assert.IsFalse(maybeResponse.ResultUnsafe.IsReadOnly);
            Assert.AreEqual(20 ,maybeResponse.ResultUnsafe.FileLength);
        }
        [TestMethod]
        public void TestRootKey()
        {
            var expectedHash = "Qmf1hYpfyrAeLT1sdkkNauump9W9vNke8wxdBtrbjtUbdu";
            var request = new RootHashRequest();
            var maybeResponse = communicator.GetResponse(request, RootHashResponse.Parse);
            if (maybeResponse.IsError)
                Assert.Fail("Parsing error: " + maybeResponse.ErrorUnsafe);
            Assert.AreEqual(expectedHash, maybeResponse.ResultUnsafe.RootHash);
        }
        [TestMethod]
        public void TestClone()
        {
            var path1 = "f7bbda7ed735f7b0652743bc6a765dcccf0114deaaa1ed22da1378943d9f25cc";
            var path2 = "f7bbda7ed735f7b0652732423443bc6a765dcccf0114deaaa1ed22da1378943d9f25cc";
            var request = new CloneRequest(path1, path2);
            var maybeResponse = communicator.GetResponse(request, CloneResponse.Parse);
            if (maybeResponse.IsError)
                Assert.Fail("Parsing error: " + maybeResponse.ErrorUnsafe);
        }
        [TestMethod]
        public void TestFollow()
        {
            var path = "f7bbda7ed735f7b0652743bc6a765dcccf0114deaaa1ed22da1378943d9f25cc";
            var request = new FollowRequest(path);
            var maybeResponse = communicator.GetResponse(request, FollowResponse.Parse);
            if (maybeResponse.IsError)
                Assert.Fail("Parsing error: " + maybeResponse.ErrorUnsafe);
        }

        [TestMethod]
        public void TestConnection()
        {
            var timeout = TimeSpan.FromSeconds(10);
            var message = new byte[] {1, 2, 3, 4};
            var result = communicator.Comunicator.SendAndRecieveMessage(message, timeout);
            var responseFailure = result as RecieveMessageResult.RecieveFailure;
            var responseSuccess = result as RecieveMessageResult.RecieveSuccess;
            if (responseFailure != null)
                Assert.Fail(responseFailure.FailureMessage);
            Assert.IsTrue(responseSuccess != null);
            Assert.IsTrue(responseSuccess.Data.ArrayEquals(message, (a,b) => a == b));
        }
    }
}
