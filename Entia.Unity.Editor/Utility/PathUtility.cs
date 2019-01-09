using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Entia.Core;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class PathUtility
    {
        public static string Replace(string path) => path
            .Replace("{Editor.Managed}", Path.Combine("{Editor.Data}", "Managed"))
            .Replace("{Editor.Extensions}", Path.Combine("{Editor.Data}", "UnityExtensions", "Unity"))
            .Replace("{Editor.Templates}", Path.Combine("{Editor.Data}", "Resources", "ScriptTemplates"))
            .Replace("{Editor.Mono}", Path.Combine("{Editor.Data}", "MonoBleedingEdge", "lib", "mono", "unityjit"))
            .Replace("{Editor.Data}", EditorApplication.applicationContentsPath)
            .Replace("{Editor}", EditorApplication.applicationPath.Directory())
            .Replace("{Library.Assemblies}", Path.Combine("{Library}", "ScriptAssemblies"))
            .Replace("{Library}", Path.Combine("{Project}", "Library"))
            .Replace("{Project}", Path.Combine("{Assets}", ".."))
            .Replace("{Assets.Plugins}", Path.Combine("{Assets}", "Plugins"))
            .Replace("{Assets.Scripts}", Path.Combine("{Assets}", "Scripts"))
            .Replace("{Assets}", Application.dataPath)
            .Replace("{Entia}", typeof(Entity).Assembly.Location.Directory())
            .Replace("{DotNet}", typeof(object).Assembly.Location.Directory())
            .ReplacePatterns()
            .Absolute();

        static string ReplacePatterns(this string path)
        {
            var match = Regex.Match(path, @"\|.*?\|");
            if (match.Success)
            {
                var file = Directory.EnumerateFileSystemEntries("./", match.Value.Trim('|'), SearchOption.AllDirectories).FirstOrDefault() ?? "";
                return ReplacePatterns(path.Replace(match.Index, match.Length, file));
            }

            return path;
        }
    }
}