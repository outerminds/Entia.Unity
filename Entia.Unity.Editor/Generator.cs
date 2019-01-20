using Entia.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public class Generator : AssetPostprocessor
    {
        [MenuItem("Entia/Generator/Birth")]
        public static bool Birth() => TrySettings(out var settings) && Tool(settings) is string tool && Birth(tool, settings.Debug, true);

        [MenuItem("Entia/Generator/Kill")]
        public static bool Kill() => TrySettings(out var settings) && Tool(settings) is string tool && Kill(tool, true);

        [MenuItem("Entia/Generator/Generate %#g")]
        public static void Generate() => Generate(true);

        static void Generate(bool log)
        {
            var settings = Settings();
            var tool = Tool(settings);
            if (string.IsNullOrWhiteSpace(tool))
            {
                UnityEngine.Debug.LogError($"Could not find a valid executable at path '{settings.Tool}'. Make sure a proper path is defined in the '{nameof(GeneratorSettings)}' asset.");
                return;
            }

            Birth(tool, settings.Debug, settings.Debug || log);

            var arguments = Arguments(tool, settings);
            var buffer = new byte[4096];
            var input = string.Join("|", arguments);
            var output = "";
            var timer = Stopwatch.StartNew();

            try
            {
                using (var client = new NamedPipeClientStream(".", tool, PipeDirection.InOut, PipeOptions.WriteThrough))
                {
                    while (!client.IsConnected && timer.Elapsed.TotalSeconds < settings.Timeout)
                    {
                        try { client.Connect(); }
                        catch { Thread.Sleep(100); }
                    }

                    var count = Encoding.UTF32.GetBytes(input, 0, input.Length, buffer, 0);
                    client.Write(buffer, 0, count);

                    count = client.Read(buffer, 0, buffer.Length);
                    output = Encoding.UTF32.GetString(buffer, 0, count);
                }

                if (log) UnityEngine.Debug.Log(
$@"Generation succeeded after '{timer.Elapsed}'.
-> Input: {string.Join(" ", arguments.Select(argument => $@"""{argument}"""))}
-> Output: {output}");

                EditorUtility.Delayed(AssetDatabase.Refresh, 1);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(
$@"Generation failed after '{timer.Elapsed}'.
-> Input: {string.Join(" ", arguments.Select(argument => $@"""{argument}"""))}
-> Output: {exception}");
            }
        }

        public static bool TryGenerate(string path)
        {
            if (TrySettings(out var settings) && IsScript(path) && settings.Inputs.Any(input => IsSubPath(path, input)) && !IsSubPath(path, settings.Output))
            {
                Generate(settings.Debug);
                return true;
            }

            return false;
        }

        static bool Birth(string tool, bool debug, bool log)
        {
            var process = Processes(tool).FirstOrDefault();
            if (process == null)
            {
                process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Application.dataPath,
                    FileName = $"dotnet",
                    Arguments = $"{tool} --watch {Process.GetCurrentProcess().Id};{tool}",
                    CreateNoWindow = !debug,
                    WindowStyle = debug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                });

                if (log) UnityEngine.Debug.Log($"Gave birth to generator in process '{process.Id}'.");

                return true;
            }

            if (log) UnityEngine.Debug.Log($"Generator is already alive in process '{process.Id}'.");
            return false;
        }

        static bool Kill(string tool, bool log)
        {
            var killed = false;
            foreach (var process in Processes(tool))
            {
                process.Kill();
                killed = true;
                if (log) UnityEngine.Debug.Log($"Killed generator process '{process.Id}'.");
            }

            if (log && !killed) UnityEngine.Debug.Log($"Generator processes was not found.");
            return killed;
        }

        static string[] Arguments(string tool, GeneratorSettings settings) =>
            new string[]
            {
                "--inputs",
                $@"{string.Join(";", settings.Inputs)}",
                "--output",
                $@"{settings.Output}",
                "--suffix",
                $@"{settings.Suffix}",
                "--assemblies",
                $@"{string.Join(";", settings.Assemblies)}",
                "--log",
                $@"{settings.Log}"
            };

        static Process[] Processes() => Process.GetProcessesByName("dotnet");
        static IEnumerable<Process> Processes(string tool) => Processes().Where(process => process.Modules.Cast<ProcessModule>().Any(module => module.FileName == tool));

        static string Tool(GeneratorSettings settings) => settings.Tool.Files("*.dll").FirstOrDefault();

        static bool TrySettings(out GeneratorSettings settings) => (settings = GeneratorSettings.Instance) != null;

        static GeneratorSettings Settings()
        {
            if (TrySettings(out var settings)) return settings;
            settings = ScriptableObject.CreateInstance<GeneratorSettings>();

            try
            {
                var directory = typeof(Entity).Assembly.Location.Directory();
                var path = Path.Combine(directory, "Settings.asset").Relative(Application.dataPath);
                Directory.CreateDirectory(directory);
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.Refresh();
                UnityEngine.Debug.LogWarning($"Could not find a '{nameof(GeneratorSettings)}' asset in project. A default instance was created at path '{path}'.");
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning($"Could not find a '{nameof(GeneratorSettings)}' asset in project. A default instance was used instead. It can be created from menu 'Assets/Create/Entia/Generator/Settings'.");
                UnityEngine.Debug.LogException(exception);
            }

            return settings;
        }

        static bool IsScript(string path) => Path.GetExtension(path) == ".cs";

        static bool IsSubPath(string path, string super)
        {
            var file = new FileInfo(path);
            var directory = new DirectoryInfo(Path.Combine(Application.dataPath, super));
            var current = file.Directory;

            while (current != null)
            {
                if (current.FullName.Absolute() == directory.FullName.Absolute()) return true;
                else current = current.Parent;
            }

            return false;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssets)
        {
            if (TrySettings(out var settings) && settings.Automatic)
            {
                var (moved, renamed) = movedAssets.Zip(movedFromAssets, (to, from) => (to, from)).Split(pair => Path.GetFileName(pair.to) == Path.GetFileName(pair.from));
                if (settings.Debug)
                {
                    UnityEngine.Debug.Log(
$@"OnPostprocessAllAssets:
-> Imported: {string.Join(" | ", importedAssets)}
-> Deleted: {string.Join(" | ", deletedAssets)}
-> Renamed: {string.Join(" | ", renamed)}
-> Moved: {string.Join(" | ", moved)}");
                }

                foreach (var path in moved.SelectMany(pair => new[] { pair.from, pair.to }).Concat(importedAssets, deletedAssets))
                    if (TryGenerate(path)) break;
            }
        }
    }
}