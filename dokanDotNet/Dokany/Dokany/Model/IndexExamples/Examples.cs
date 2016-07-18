using System.Collections.Generic;
using Dokany.Model.Entries;
using Dokany.Model.PathUtils;
using Dokany.Model.Pointers;
using Dokany.Util;

namespace Dokany.Model.IndexExamples
{
    public static class Examples
    {
        public static Dictionary<T, S> EmptyDict<T, S>()
        {
            return new Dictionary<T, S>();
        }

        public static Folder EmptyFolder()
        {
            var files = EmptyDict<Bracket, FileHash>();
            var follows = EmptyDict<Bracket, RemotePath>();
            var folders = EmptyDict<Bracket, Folder>();
            return new Folder(files, follows, folders);
        }
        public static Index Index1()
        {
            var files = new Dictionary<Bracket, FileHash>
            {
                { "one.txt".AsBracket(), new FileHash(Hash.Random(20, Global.Rand))},
                { "two.txt".AsBracket(), new FileHash(Hash.Random(20, Global.Rand))}
            };
            var follows = EmptyDict<Bracket, RemotePath>();
            var folders = EmptyDict<Bracket, Folder>();
            var mainFolder = new Folder(files, follows, folders);
            return new Index(mainFolder);
        }

        public static Index Index2()
        {
            var files = EmptyDict<Bracket, FileHash>();
            var follows = EmptyDict<Bracket, RemotePath>();
            var folders = new Dictionary<Bracket, Folder>
            {
                {"one".AsBracket(), EmptyFolder()},
                {"two".AsBracket(), EmptyFolder()}
            };
            var mainFolder = new Folder(files, follows, folders);
            folders["one".AsBracket()].Folders.Add("main".AsBracket(), mainFolder);
            return new Index(mainFolder);
        }

        public static Index Index3()
        {
            var files = new Dictionary<Bracket, FileHash>
            {
                { "first.txt".AsBracket(), new FileHash(Hash.Random(20, Global.Rand))},
                { "second.txt".AsBracket(), new FileHash(Hash.Random(20, Global.Rand))}
            };
            var follows = EmptyDict<Bracket, RemotePath>();
            var folders = new Dictionary<Bracket, Folder>
            {
                {"one".AsBracket(), EmptyFolder()},
                {"two".AsBracket(), EmptyFolder()}
            };
            var mainFolder = new Folder(files, follows, folders);
            folders["one".AsBracket()].Folders.Add("mainCircle".AsBracket(), mainFolder);
            return new Index(mainFolder);
        }
        public static Index Index4()
        {
            var file1 = new FileHash(Hash.Random(20, Global.Rand));
            var file2 = new FileHash(Hash.Random(20, Global.Rand));
            var file3 = new FileHash(Hash.Random(20, Global.Rand));

            var folderThree = EmptyFolder();
            folderThree.Files["three.txt".AsBracket()] = file3;

            var mainFolder = EmptyFolder();
            mainFolder.Files["one.txt".AsBracket()] = file1;
            mainFolder.Files["two.txt".AsBracket()] = file2;
            mainFolder.Folders["folderThree".AsBracket()] = folderThree;

            return new Index(mainFolder);
        }
    }
}
