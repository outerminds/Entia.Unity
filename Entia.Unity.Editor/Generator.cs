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
    [InitializeOnLoad]
    public class Generator : AssetPostprocessor
    {
        static Generator()
        {
            if (TrySettings(out var settings) && settings.Automatic && TryTool(settings, settings.Debug, out var tool))
                Birth(tool, settings, settings.Debug);
        }

        [MenuItem("Entia/Generator/Birth")]
        public static void Birth()
        {
            var settings = Settings();
            if (TryTool(settings, true, out var tool)) Birth(tool, settings, true);
        }

        [MenuItem("Entia/Generator/Kill")]
        public static void Kill()
        {
            var settings = Settings();
            if (TryTool(settings, true, out var tool)) Kill(tool, true);
        }

        [MenuItem("Entia/Generator/Generate %#g")]
        public static void Generate()
        {
            var settings = Settings();
            if (TryTool(settings, true, out var tool)) Generate(tool, settings, true);
        }

        public static void Generate(string tool, GeneratorSettings settings, bool log, params string[] changes)
        {
            var process = Birth(tool, settings, settings.Debug || log);
            var arguments = Arguments(tool, settings, false, changes);
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
                        catch
                        {
                            Thread.Sleep(100);
                            if (process.HasExited)
                            {
                                UnityEngine.Debug.LogError(
$@"Failed to connect to generator process.
This may happen because the .Net Core Runtime is not installed on this machine.
-> Go to 'https://dotnet.microsoft.com/download'.
-> Install the .Net Core Runtime version 2.1+.
-> Restart Unity.");
                                throw;
                            }
                        }
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

        public static bool IsInput(GeneratorSettings settings, string path) =>
            IsScript(path) && settings.Inputs.Any(input => IsSubPath(path, input)) && !IsSubPath(path, settings.Output);

        public static bool TryTool(GeneratorSettings settings, bool log, out string tool)
        {
            tool = settings.Tool.Files("*.dll").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(tool))
            {
                if (log) UnityEngine.Debug.LogError(
$@"Could not find a valid executable at path '{settings.Tool}'.
Make sure a proper path is defined in the '{nameof(GeneratorSettings)}' asset.");
                return false;
            }

            return true;
        }

        public static Process Birth(string tool, GeneratorSettings settings, bool log)
        {
            if (TryProcess(tool, out var process))
            {
                if (log) UnityEngine.Debug.Log($"Generator is already alive in process '{process.Id}'.");
                return process;
            }

            var input = string.Join(" ", Arguments(tool, settings, true));
            try
            {
                process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Application.dataPath,
                    FileName = $"dotnet",
                    Arguments = $"{tool} {input}",
                    CreateNoWindow = !settings.Debug,
                    WindowStyle = settings.Debug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                });
                SaveProcess(tool, process);
            }
            catch
            {
                UnityEngine.Debug.LogError(
$@"Failed to birth generator process.
This may happen because the .Net Core Runtime is not installed on this machine.
-> Go to 'https://dotnet.microsoft.com/download'.
-> Install the .Net Core Runtime version 2.1+.
-> Restart Unity.");
                throw;
            }

            if (log) UnityEngine.Debug.Log($"Gave birth to generator in process '{process.Id}' with input '{input}'.");
            return process;
        }

        public static bool Kill(string tool, bool log)
        {
            if (TryProcess(tool, out var process))
            {
                process.Kill();
                if (log) UnityEngine.Debug.Log($"Killed generator process '{process.Id}'.");
                return true;
            }

            if (log) UnityEngine.Debug.Log($"Generator process was not found.");
            return false;
        }

        public static bool TryProcess(string tool, out Process process)
        {
            if (TryLoadProcess(tool, out var identifier, out var time))
            {
                try
                {
                    process = Process.GetProcessById(identifier);
                    return !process.HasExited && process.StartTime.Ticks == time;
                }
                catch { }
            }

            process = default;
            return false;
        }

        static string[] Arguments(string tool, GeneratorSettings settings, bool watch, params string[] changes)
        {
            var arguments = new[]
            {
                "--inputs", string.Join(";", settings.Inputs),
                "--output", settings.Output,
                "--suffix", settings.Suffix,
                "--assemblies", string.Join(";", settings.Assemblies),
                "--log", settings.Log
            };
            if (changes.Length > 0) Core.ArrayUtility.Add(ref arguments, "--changes", string.Join(";", changes));
            if (watch) Core.ArrayUtility.Add(ref arguments, "--watch", $"{SerializeProcess(Process.GetCurrentProcess())};{tool}");
            return arguments;
        }

        static string SerializeProcess(Process process) => $"{process.Id};{process.StartTime.Ticks}";

        static void SaveProcess(string tool, Process process) =>
            EditorPrefs.SetString(ProcessKey(tool), SerializeProcess(process));

        static bool TryLoadProcess(string tool, out int identifier, out long ticks)
        {
            if (EditorPrefs.GetString(ProcessKey(tool), null) is string value)
            {
                var splits = value.Split(';');
                if (splits.Length == 2 && int.TryParse(splits[0], out identifier) && long.TryParse(splits[1], out ticks))
                    return true;
            }

            identifier = default;
            ticks = default;
            return false;
        }

        static string ProcessKey(string tool) => $"Entia_Generator:{tool}";

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
                UnityEngine.Debug.LogWarning(
$@"Could not find a '{nameof(GeneratorSettings)}' asset in project.
A default instance was created at path '{path}'.");
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(
$@"Could not find or create a '{nameof(GeneratorSettings)}' asset in project.
A default instance was used instead. It can be created from menu 'Assets/Create/Entia/Generator/Settings'.");
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
            if (TrySettings(out var settings) && settings.Automatic && TryTool(settings, false, out var tool))
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

                var changes = moved
                    .SelectMany(pair => new[] { pair.from, pair.to })
                    .Concat(importedAssets, deletedAssets)
                    .Where(asset => IsInput(settings, asset))
                    .Select(asset => asset.Absolute())
                    .ToArray();
                if (changes.Length > 0) Generate(tool, settings, settings.Debug, changes);
            }
        }
    }
}