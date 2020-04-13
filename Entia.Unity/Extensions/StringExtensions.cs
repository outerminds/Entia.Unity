using System;
using System.Collections.Generic;
using System.IO;

namespace Entia.Unity
{
    public static class StringExtensions
    {
        public static IEnumerable<string> Files(this string path, string filter, SearchOption option = SearchOption.AllDirectories)
        {
            switch (path.Information())
            {
                case FileInfo file: yield return file.FullName; break;
                case DirectoryInfo directory:
                    foreach (var info in directory.EnumerateFiles(filter, option)) yield return info.FullName;
                    break;
            }
        }

        public static bool Exists(this string path) => path.Information() is FileSystemInfo;

        public static string Absolute(this string path) => string.IsNullOrWhiteSpace(path) ? "" : Path.GetFullPath(path);

        public static string Quote(this string path)
        {
            const string quote = @"""";
            if (string.IsNullOrWhiteSpace(path)) return path;
            if (!path.StartsWith(quote)) path = quote + path;
            if (!path.EndsWith(quote)) path = path + quote;
            return path;
        }
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

        public static FileSystemInfo Information(this string path)
        {
            try
            {
                var absolute = path.Absolute();
                var file = new FileInfo(absolute);
                if (file.Exists) return file;
                var directory = new DirectoryInfo(absolute);
                if (directory.Exists) return directory;
            }
            catch { }
            return null;
        }

        public static string Extension(this string path) => Path.GetExtension(path);
        public static string Directory(this string path) => new DirectoryInfo(path).Exists ? path : Path.GetDirectoryName(path);
        public static IEnumerable<string> Directories(this string path)
        {
            var directory = new DirectoryInfo(path.Directory());
            while (directory.Exists)
            {
                yield return directory.Name;
                directory = directory.Parent;
            }
        }
        public static string File(this string path, bool extension = true) => extension ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        public static string ChangeExtension(this string path, string extension) => Path.ChangeExtension(path, extension);
        public static string ChangeFileName(this string path, string name) => Path.Combine(path.Directory(), name) + path.Extension();
        public static string Decapitalize(this string value) =>
            string.IsNullOrEmpty(value) || char.IsLower(value[0]) ? value :
            char.ToLower(value[0]) + value.Substring(1);
        public static string Capitalize(this string value) =>
            string.IsNullOrEmpty(value) || char.IsUpper(value[0]) ? value :
            char.ToUpper(value[0]) + value.Substring(1);

        public static string Justify(this string value, int length) => value + new string(' ', Math.Max(length - value.Length, 0));

        public static bool IsSubPath(this string path, string super) => path.Absolute().StartsWith(super.Absolute());
    }
}
