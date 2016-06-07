using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                { "one.txt".AsBracket(), new FileHash(Hash.Random(20))},
                { "two.txt".AsBracket(), new FileHash(Hash.Random(20))}
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
            folders["one".AsBracket()].folders.Add("main".AsBracket(), mainFolder);
            return new Index(mainFolder);
        }

        public static Index Index3()
        {
            var files = new Dictionary<Bracket, FileHash>
            {
                { "first.txt".AsBracket(), new FileHash(Hash.Random(20))},
                { "second.txt".AsBracket(), new FileHash(Hash.Random(20))}
            };
            var follows = EmptyDict<Bracket, RemotePath>();
            var folders = new Dictionary<Bracket, Folder>
            {
                {"one".AsBracket(), EmptyFolder()},
                {"two".AsBracket(), EmptyFolder()}
            };
            var mainFolder = new Folder(files, follows, folders);
            folders["one".AsBracket()].folders.Add("mainCircle".AsBracket(), mainFolder);
            return new Index(mainFolder);
        }
    }
}
