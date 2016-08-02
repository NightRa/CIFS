using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.Binary;
using Utils.GeneralUtils;

namespace UnitTests
{
    [TestClass]
    public class GeneralParsing
    {
        [TestMethod]
        public void TestInt()
        {
            var index = new Box<int>(0);
            var num = 0x5793FE92;
            Assert.AreEqual(num, num.ToBytes().GetInt(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestLong()
        {
            var index = new Box<int>(0);
            var num = 0x6D509F025793FE92;
            Assert.AreEqual(num, num.ToBytes().GetLong(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestCh()
        {
            var index = new Box<int>(0);
            var ch = '~';
            Assert.AreEqual(ch, ch.ToBytes().GetChar(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestString()
        {
            var index = new Box<int>(0);
            var str = @"C:\Users\Yuval\Desktop\תמונות\IMG_20160509_150936.jpg";
            Assert.AreEqual(str, str.ToBytes().GetString(index).ResultUnsafe);
        }
        [TestMethod]
        public void TestBool()
        {
            var index = new Box<int>(0);
            var b = false;
            Assert.AreEqual(b, b.ToBytes().GetBool(index).ResultUnsafe);
        }
    }
}
