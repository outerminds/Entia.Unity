using System;
using System.IO;
using System.Linq;
using Entia.Unity;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class DLLInstaller
    {
        [MenuItem("Entia/Install/DLLs")]
        public static void Install()
        {
            var managed = PathUtility.Replace("{Editor.Managed}");
            var extensions = PathUtility.Replace("{Editor.Extensions}");
            var target = PathUtility.Replace("{Project}/DLLs");

            Directory.CreateDirectory(target);
            var files = Enumerable.Concat(
                Directory.EnumerateFiles(managed, "*.dll", SearchOption.AllDirectories),
                Directory.EnumerateFiles(extensions, "*.dll", SearchOption.AllDirectories));

            foreach (var dll in files)
            {
                var path = Path.Combine(target, dll.File());
                try { File.Copy(dll, path, true); }
                catch
                {
                    Debug.LogWarning($"Failed to install DLL '{dll}' at path '{path}'. This may be happening because Unity is not running in administrator mode.");
                    throw;
                }
            }

            Debug.Log($"DLLs installed from '{managed}' and '{extensions}' to '{target}'.");
        }
    }
}