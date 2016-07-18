﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dokany.Model.PathUtils;
using static System.StringSplitOptions;

namespace Dokany.Util
{
    public static class FileSystemUtil
    {
        public static Bracket AsBracket(this string @this)
        {
            return new Bracket(@this);
        }
        public static Brackets AsBrackets(this string @this)
        {
            return @this.Split('\\'.Singleton(), RemoveEmptyEntries).AsBrackets();
        }
        public static Brackets AsBrackets(this IEnumerable<string> @this)
        {
            return @this.Select(s => s.AsBracket()).AsBrackets();
        }
        public static Brackets AsBrackets(this IEnumerable<Bracket> @this)
        {
            return new Brackets(@this);
        }

        public static string GetPath(this Environment.SpecialFolder @this)
        {
            return Environment.GetFolderPath(@this);
        }
        public static DirectoryInfo GetDirectoryInfo(this string @this)
        {
            return new DirectoryInfo(@this);
        }
    }
}