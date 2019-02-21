using Entia.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [InitializeOnLoad]
    public sealed class Generator : AssetPostprocessor
    {
        sealed class ConnectionException : Exception { }

        static Generator()
        {
            if (TryFindSettings(out var settings) && settings.Automatic && TryTool(settings, settings.Debug, out var tool))
            {
                try { Birth(tool, settings, settings.Debug); }
                catch (Exception exception)
                {
                    UnityEngine.Debug.LogException(exception, settings);
                    UnityEngine.Debug.LogWarning($"Automatic birth of the generator may be disabled in the '{nameof(GeneratorSettings)}' asset by unchecking the '{nameof(GeneratorSettings.Automatic)}' option.", settings);
                }
            }
        }

        [MenuItem("Entia/Generator/Birth")]
        static void Birth()
        {
            var settings = FindOrCreateSettings();
            if (TryTool(settings, true, out var tool)) Birth(tool, settings, true);
        }

        [MenuItem("Entia/Generator/Kill")]
        static void Kill()
        {
            var settings = FindOrCreateSettings();
            if (TryTool(settings, true, out var tool)) Kill(tool, settings, true);
        }

        [MenuItem("Entia/Generator/Generate %#g")]
        static void Generate()
        {
            var settings = FindOrCreateSettings();
            if (TryTool(settings, true, out var tool)) Generate(tool, settings, true);
        }

        [MenuItem("Entia/Generator/Settings")]
        static void Settings() => Selection.activeObject = FindOrCreateSettings();

        public static void Generate(string tool, GeneratorSettings settings, bool log, params string[] changes)
        {
            var process = Birth(tool, settings, settings.Debug || log);
            var arguments = Arguments(tool, settings, true, changes).ToArray();
            var buffer = new byte[8192];
            var input = string.Join("|", arguments);
            var output = "";
            var timer = Stopwatch.StartNew();
            var task = Task
                .Run(() =>
                {
                    using (var client = new NamedPipeClientStream(".", tool, PipeDirection.InOut, PipeOptions.WriteThrough))
                    {
                        do
                        {
                            try { client.Connect(); }
                            catch
                            {
                                Thread.Sleep(100);
                                if (process.HasExited) throw new ConnectionException();
                            }
                        }
                        while (!client.IsConnected);

                        var count = Encoding.UTF32.GetBytes(input, 0, input.Length, buffer, 0);
                        client.Write(buffer, 0, count);

                        count = client.Read(buffer, 0, buffer.Length);
                        output = Encoding.UTF32.GetString(buffer, 0, count);
                    }
                })
                .Timeout(settings.Timeout)
                .Do(() =>
                {
                    if (log) UnityEngine.Debug.Log(
$@"Generation succeeded after '{timer.Elapsed}'.
-> Input: {string.Join(" ", arguments)}
-> Output: {output}", settings);
                })
                .Except<TimeoutException>(exception => UnityEngine.Debug.LogError(
$@"Generation timed out after '{timer.Elapsed}'.
This may be happening because the 'Timeout' value of '{settings.Timeout}' is too low.
-> Input: {string.Join(" ", arguments)}
-> Output: {exception}", settings))
                .Except<ConnectionException>(exception => UnityEngine.Debug.LogError(
$@"Failed to connect to generator process.
This may happen because the .Net Core Runtime is not installed on this machine.
-> Go to 'https://dotnet.microsoft.com/download'.
-> Install the .Net Core Runtime version 2.1+.
-> Restart Unity.", settings))
                .Except(exception => UnityEngine.Debug.LogError(
$@"Generation failed after '{timer.Elapsed}'.
-> Input: {string.Join(" ", arguments)}
-> Output: {exception}", settings));

            task.Wait();
            if (task.IsCompleted) EditorUtility.Delayed(AssetDatabase.Refresh);
        }

        public static bool IsInput(GeneratorSettings settings, string path) =>
            IsScript(path) && settings.Inputs.Any(input => path.IsSubPath(input)) && !path.IsSubPath(settings.Output);

        public static bool TryTool(GeneratorSettings settings, bool log, out string tool)
        {
            tool = settings.Tool.Files("*.dll").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(tool))
            {
                if (log) UnityEngine.Debug.LogError(
$@"Could not find a valid executable at path '{settings.Tool}'.
Make sure a proper path is defined in the '{nameof(GeneratorSettings)}' asset.", settings);
                return false;
            }

            return true;
        }

        public static Process Birth(string tool, GeneratorSettings settings, bool log)
        {
            if (TryProcess(tool, out var process))
            {
                if (log) UnityEngine.Debug.Log($"Generator is already alive in process '{process.Id}'.", settings);
                return process;
            }

            var input = string.Join(" ", Arguments(tool, settings, true));
            try
            {
                process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Application.dataPath,
                    FileName = "dotnet",
                    Arguments = $"{tool.Quote()} {input}",
                    CreateNoWindow = !settings.Debug,
                    WindowStyle = settings.Debug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                });
                SaveProcess(tool, process);
            }
            catch
            {
                UnityEngine.Debug.LogError(
$@"Failed to birth generator process from path '{tool}'.
This may happen because the .Net Core Runtime is not installed on this machine.
-> Go to 'https://dotnet.microsoft.com/download'.
-> Install the .Net Core Runtime version 2.1+.
-> Restart Unity.", settings);
                throw;
            }

            if (log) UnityEngine.Debug.Log($"Gave birth to generator at path '{tool}' in process '{process.Id}' with input '{input}'.", settings);
            return process;
        }

        public static bool Kill(string tool, GeneratorSettings settings, bool log)
        {
            if (TryProcess(tool, out var process))
            {
                process.Kill();
                if (log) UnityEngine.Debug.Log($"Killed generator process '{process.Id}'.", settings);
                return true;
            }

            if (log) UnityEngine.Debug.Log($"Generator process was not found.", settings);
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

        static IEnumerable<string> Arguments(string tool, GeneratorSettings settings, bool watch, params string[] changes)
        {
            yield return "--inputs";
            yield return string.Join(";", settings.Inputs.Select(input => input.Quote()));
            yield return "--output";
            yield return settings.Output.Quote();
            yield return "--suffix";
            yield return settings.Suffix.Quote();
            yield return "--assemblies";
            yield return string.Join(";", settings.Assemblies.Select(assembly => assembly.Quote()));
            yield return "--log";
            yield return settings.Log.Quote();
            yield return "--timeout";
            yield return settings.Timeout.ToString();

            if (changes.Length > 0)
            {
                yield return "--changes";
                yield return string.Join(";", changes.Select(change => change.Quote()));
            }

            if (watch)
            {
                yield return "--watch";
                yield return $@"{SerializeProcess(Process.GetCurrentProcess())};{settings.Watch};{tool.Quote()}";
            }
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

        static bool TryFindSettings(out GeneratorSettings settings) => (settings = GeneratorSettings.Instance) != null;

        static GeneratorSettings FindOrCreateSettings()
        {
            if (TryFindSettings(out var settings)) return settings;
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
A default instance was created at path '{path}'.", settings);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(
$@"Could not find or create a '{nameof(GeneratorSettings)}' asset in project.
A default instance was used instead. It can be created from menu 'Assets/Create/Entia/Generator/Settings'.", settings);
                UnityEngine.Debug.LogException(exception);
            }

            return settings;
        }

        static bool IsScript(string path) => Path.GetExtension(path) == ".cs";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssets)
        {
            if (TryFindSettings(out var settings) && settings.Automatic && TryTool(settings, false, out var tool))
            {
                var (moved, renamed) = movedAssets.Zip(movedFromAssets, (to, from) => (to, from)).Split(pair => Path.GetFileName(pair.to) == Path.GetFileName(pair.from));
                var changes = moved
                    .SelectMany(pair => new[] { pair.from, pair.to })
                    .Concat(importedAssets, deletedAssets)
                    .Where(asset => IsInput(settings, asset))
                    .Select(asset => asset.Absolute())
                    .ToArray();

                if (settings.Debug)
                {
                    UnityEngine.Debug.Log(
$@"OnPostprocessAllAssets:
-> Changed: {string.Join(" | ", changes)}
-> Imported: {string.Join(" | ", importedAssets)}
-> Deleted: {string.Join(" | ", deletedAssets)}
-> Renamed: {string.Join(" | ", renamed)}
-> Moved: {string.Join(" | ", moved)}");
                }

                if (changes.Length > 0)
                {
                    try { Generate(tool, settings, settings.Debug, changes); }
                    catch (Exception exception)
                    {
                        UnityEngine.Debug.LogException(exception, settings);
                        UnityEngine.Debug.LogWarning($"Automatic birth of the generator may be disabled in the '{nameof(GeneratorSettings)}' asset by unchecking the '{nameof(GeneratorSettings.Automatic)}' option.", settings);
                    }
                }
            }
        }
    }
}