using System;
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
using Utils.GeneralUtils;
using Utils.RandomUtils;

namespace UnitTests
{
    [TestClass]
    public class MessagingEncoding
    {
        private readonly Random Rnd = new Random();

        [TestMethod]
        public void TestCreateFile()
        {
            var path = Rnd.NextString(200);
            var request =  new CreateFileRequest(path);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(6, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestCreateFolder()
        {
            var path = Rnd.NextString(200);
            var request =  new CreateFolderRequest(path);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(7, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestDelete()
        {
            var path = Rnd.NextString(200);
            var request =  new DeleteRequest(path);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(8, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestFlush()
        {
            var request =  new FlushRequest();
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(1, bytes.GetByte(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestLs()
        {
            var path = Rnd.NextString(200);
            var request = new GetInnerEntriesRequest(path);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(3, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestMove()
        {
            var pathFrom = Rnd.NextString(200);
            var pathTo = Rnd.NextString(200);
            var request = new MoveRequest(pathFrom, pathTo);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(9, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(pathFrom, bytes.GetString(index).ResultUnsafe);
            Assert.AreEqual(pathTo, bytes.GetString(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestReadFile()
        {
            long offset = 0x637492039483349;
            int amountToRead = 0x39483349;
            var path = Rnd.NextString(200);
            var request = new ReadFileRequest(path, offset, amountToRead);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(4, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
            Assert.AreEqual(offset, bytes.GetLong(index).ResultUnsafe);
            Assert.AreEqual(amountToRead, bytes.GetInt(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestWriteFile()
        {
            long offset = 0x637492039483349;
            int amountToWrite = 0x2343;
            var path = Rnd.NextString(200);
            var data = new byte[amountToWrite];
            lock (Rnd)
                Rnd.NextBytes(data);
            var request = new WriteFileRequest(path, data, offset);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(5, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
            Assert.AreEqual(offset, bytes.GetLong(index).ResultUnsafe);
            Assert.AreEqual(amountToWrite, bytes.GetInt(index).ResultUnsafe);
            Assert.IsTrue(data.ArrayEquals(bytes.GetBytes(index, amountToWrite).ResultUnsafe, (a, b) => a == b));
        }
        [TestMethod]
        public void TestRootHash()
        {
            var request = new RootHashRequest();
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(0, bytes.GetByte(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestStat()
        {
            var path = Rnd.NextString(200);
            var request = new StatRequest(path);
            var bytes = request.ToBytes();
            var index = new Box<int>(0);
            Assert.AreEqual(2, bytes.GetByte(index).ResultUnsafe);
            Assert.AreEqual(path, bytes.GetString(index).ResultUnsafe);
        }
    }
}
