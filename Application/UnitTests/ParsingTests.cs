using System;
using System.Collections.Generic;
using CifsPreferences;
using FileSystem.Entries;
using FileSystem.Pointers;
using FileSystemBrackets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils;
using Utils.Binary;
using Utils.DictionaryUtil;
using Utils.GeneralUtils;
using Utils.Parsing;

namespace UnitTests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        public void TestReturn()
        {
            var pure = Parse.Return(77);
            Assert.IsTrue(pure.IsResult);
            Assert.IsFalse(pure.IsError);
            Assert.AreEqual(77 ,pure.ResultUnsafe);
        }
        [TestMethod]
        public void TestError()
        {
            var pure = Parse.Error<int>("hello");
            Assert.IsFalse(pure.IsResult);
            Assert.IsTrue(pure.IsError);
            Assert.AreEqual("hello", pure.ErrorUnsafe);
        }
        [TestMethod]
        public void TestParseInt()
        {
            var num = 0x12345678;
            var bytes = num.ToBytes();
            Assert.AreEqual(4, bytes.Length);
            Assert.AreEqual(0x12, bytes[0]);
            Assert.AreEqual(0x34, bytes[1]);
            Assert.AreEqual(0x56, bytes[2]);
            Assert.AreEqual(0x78, bytes[3]);
            var index = new Box<int>(0);
            var parsedNum = bytes.GetInt(index);
            Assert.AreEqual(4, index.Value);
            Assert.AreEqual(num, parsedNum.ResultUnsafe);
        }
        [TestMethod]
        public void TestParseChar1()
        {
            char ch = 'D';
            var bytes = ch.ToBytes();
            var parse = bytes.GetChar(new Box<int>(0));
            Assert.IsTrue(parse.IsResult);
            Assert.AreEqual(ch, parse.ResultUnsafe);
        }
        [TestMethod]
        public void TestParseChar2()
        {
            char ch = '8';
            var bytes = ch.ToBytes();
            var parse = bytes.GetChar(new Box<int>(0));
            Assert.IsTrue(parse.IsResult);
            Assert.AreEqual(ch, parse.ResultUnsafe);
        }
        [TestMethod]
        public void TestParseChar3()
        {
            char ch = 'א';
            var bytes = ch.ToBytes();
            var parse = bytes.GetChar(new Box<int>(0));
            Assert.IsTrue(parse.IsResult);
            Assert.AreEqual(ch, parse.ResultUnsafe);
        }
        [TestMethod]
        public void TestParseString()
        {
            var str = "sadfsdsdfsdgdsg675iet7i6r7iw45";
            var bytes = str.ToBytes();
            var parse = bytes.GetString(new Box<int>(0));
            Assert.IsTrue(parse.IsResult);
            Assert.AreEqual(str, parse.ResultUnsafe);
        }
        [TestMethod]
        public void TestParseDictionary()
        {
            var d = new Dictionary<int, string>();
            d.Add(4, "etrרקאעגעגכasa");
            d.Add(999, "bosfvdxfbsbגכדכגדכo");
            var bytes = d.ToBytes(IntBinary.ToBytes, StringBinary.ToBytes);
            var maybeDict = bytes.GetDictionary(new Box<int>(0), IntBinary.GetInt, StringBinary.GetString);
            Assert.IsTrue(d.EqualDictionary(maybeDict.ResultUnsafe));
        }
        [TestMethod]
        public void TestParsePreferences()
        {
            var p = new Preferences(true, 'G', "123.4.5.6");
            var bytes = p.ToBytes();
            var maybeP = Preferences.Parse(bytes, new Box<int>(0));
            Assert.IsTrue(maybeP.IsResult);
            var parsed = maybeP.ResultUnsafe;
            Assert.AreEqual(p.DriverChar, parsed.DriverChar);
            Assert.AreEqual(p.OpenOnStartup, parsed.OpenOnStartup);
        }
        [TestMethod]
        public void TestParseIndex1()
        {
            var files = new Dictionary<Bracket, FileHash>();
            var follows = new Dictionary<Bracket, RemotePath>();
            var folders = new Dictionary<Bracket, Folder>();
            
            var index = new Index(new Folder(files, follows, folders));
            var bytes = index.ToBytes();
            var maybeI = Index.Parse(bytes, new Box<int>(0));
            Assert.IsTrue(maybeI.IsResult);
            var parsed = maybeI.ResultUnsafe;
            Assert.IsTrue(index.Equals(parsed));
        }
        [TestMethod]
        public void TestParseIndex2()
        {
            var files = new Dictionary<Bracket, FileHash>();
            var follows = new Dictionary<Bracket, RemotePath>();
            var folders = new Dictionary<Bracket, Folder>();
            var rnd = new Random(0x592FE901);
            var length = 25;
            files["dsfsdf".AsBracket()] = new FileHash(Hash.Random(length, rnd));
            files["גכעקרעקרכ.?.asd102".AsBracket()] = new FileHash(Hash.Random(length, rnd));
            var path = new Brackets(new []{"asda".AsBracket(), "adfsdf".AsBracket()});
            follows["aslkdmחםדסמגש()!4332$".AsBracket()] = new RemotePath(new MutablePtr(Hash.Random(length, rnd).Bits), path);
            folders["amsclsa".AsBracket()] = Folder.Empty;
            folders["dsfvfsvs".AsBracket()] = Folder.Empty;
            folders["dsfvfsvs".AsBracket()].Folders["ךלשדצגש".AsBracket()] = Folder.Empty;
            folders["dsfvfsvs".AsBracket()].Files["asdasc,l.Q'".AsBracket()] = new FileHash(Hash.Random(length, rnd));

            var index = new Index(new Folder(files, follows, folders));
            var bytes = index.ToBytes();
            var maybeI = Index.Parse(bytes, new Box<int>(0));
            Assert.IsTrue(maybeI.IsResult);
            var parsed = maybeI.ResultUnsafe;
            Assert.IsTrue(index.Equals(parsed));
        }
    }
}
