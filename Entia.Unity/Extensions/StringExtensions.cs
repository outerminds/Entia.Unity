using System;
using System.Collections.Generic;
using System.IO;

namespace Entia.Unity
{
    public static class StringExtensions
    {
        public static IEnumerable<string> Files(this string path, string filter, SearchOption option = SearchOption.AllDirectories)
        {
            if (string.IsNullOrWhiteSpace(path)) yield break;

            var file = new FileInfo(path);
            if (file.Exists) yield return file.FullName;

            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                foreach (var info in directory.EnumerateFiles(filter, option))
                    yield return info.FullName;
            }
        }

        public static string Absolute(this string path) => string.IsNullOrWhiteSpace(path) ? "" : Path.GetFullPath(path);
        public static string Relative(this string path) => path.Relative(System.IO.Directory.GetCurrentDirectory());
        public static string Relative(this string path, string to) =>
            string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(to) ? "" :
            new Uri(to.Absolute(), UriKind.Absolute).MakeRelativeUri(new Uri(path.Absolute(), UriKind.Absolute)).ToString();

        public static string Replace(this string @string, int index, int count, string value)
        {
            var start = @string.Substring(0, index);
            var end = @string.Substring(index + count);
            return start + value + end;
        }

        public static string Extension(this string path) => Path.GetExtension(path);
        public static string Directory(this string path) => Path.GetDirectoryName(path);
        public static string File(this string path, bool extension = true) => extension ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        public static string ChangeExtension(this string path, string extension) => Path.ChangeExtension(path, extension);
        public static string ChangeFileName(this string path, string name) => Path.Combine(path.Directory(), name) + path.Extension();
        public static string Decapitalize(this string value) =>
            string.IsNullOrEmpty(value) ? value :
            value[0].ToString().ToLower() + value.Substring(1);
        public static string Capitalize(this string value) =>
            string.IsNullOrEmpty(value) ? value :
            value[0].ToString().ToUpper() + value.Substring(1);
    }
}
