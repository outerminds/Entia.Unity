using Entia.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Entia.Unity
{
    static class Program
    {
        static readonly ConcurrentDictionary<string, DateTime> _writes = new ConcurrentDictionary<string, DateTime>();
        static readonly object _lock = new object();

        static void Main(string[] arguments)
        {
            var logger = new StringBuilder();
            var options = Options.Parse(arguments);

            Console.WriteLine($"-> Birth: {string.Join(", ", arguments)}");

            if (string.IsNullOrEmpty(options.Watch.pipe))
                RunAsync(logger, options, arguments).Wait();
            else
            {
                var cancel = new CancellationTokenSource();
                try
                {
                    Task.Run(() => Run(logger, options, arguments), cancel.Token);
                    while (Process.GetProcessById(options.Watch.process) is Process process &&
                        !process.HasExited &&
                        process.StartTime.Ticks == options.Watch.ticks)
                        Thread.Sleep(100);
                }
                catch { }
                finally { cancel.Cancel(); }
            }

            Console.WriteLine($"-> Kill: {string.Join(", ", arguments)}");
        }

        static bool Change(params string[] paths)
        {
            var changed = false;
            foreach (var path in paths) changed |= Change(path);
            return changed;
        }

        static bool Change(string path)
        {
            var previous = _writes.TryGetValue(path, out var value) ? value : default;
            var current = File.GetLastWriteTimeUtc(path);
            if (current - previous < TimeSpan.FromMilliseconds(100)) return false;
            _writes[path] = current;
            return true;
        }

        static Disposable Watch(StringBuilder logger, Options options, string[] arguments)
        {
            void OnChanged(WatcherChangeTypes type, bool force = false, params string[] paths)
            {
                if (Monitor.TryEnter(_lock))
                {
                    try
                    {
                        Console.WriteLine($"-> Detect {type}: {string.Join(", ", paths)}");
                        if (Change(paths) || force)
                        {
                            Console.WriteLine($"-> Arguments: {string.Join(", ", arguments)}");
                            Console.WriteLine($"-> Generate...");
                            logger.Clear();
                            RunAsync(logger, options, arguments).Wait();
                        }
                    }
                    catch (Exception exception) { Console.WriteLine($"-> {exception}"); }
                    finally
                    {
                        Console.WriteLine($"-> Done");
                        Console.WriteLine();
                        Monitor.Exit(_lock);
                    }
                }
            }

            var watchers = new FileSystemWatcher[options.Inputs.Length];
            for (int i = 0; i < options.Inputs.Length; i++)
            {
                var directory = options.Inputs[i].Directory();
                var watcher = watchers[i] = new FileSystemWatcher(directory, "*.cs")
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true,
                };
                watcher.Changed += (_, data) => Task.Run(() => OnChanged(data.ChangeType, false, data.FullPath));
                watcher.Deleted += (_, data) => Task.Run(() => OnChanged(data.ChangeType, true, data.FullPath));
                watcher.Renamed += (_, data) => Task.Run(() => OnChanged(data.ChangeType, true, data.FullPath, data.OldFullPath));
                watcher.Created += (_, data) => Task.Run(() => OnChanged(data.ChangeType, true, data.FullPath));
                Console.WriteLine($"-> Watch: {directory}");
            }

            return new Disposable(() => { foreach (var watcher in watchers) try { watcher.Dispose(); } catch { } });
        }

        static void Run(StringBuilder logger, Options options, string[] arguments)
        {
            var buffer = new byte[8192];
            var response = "";
            var watcher = Watch(logger, options, arguments);

            Console.WriteLine($"-> Server: {options.Watch}");
            using (var server = new NamedPipeServerStream(options.Watch.pipe, PipeDirection.InOut, 1))
            {
                while (true)
                {
                    Console.WriteLine($"-> Wait For Connection");
                    Console.WriteLine();
                    server.WaitForConnection();

                    lock (_lock)
                    {
                        try
                        {
                            watcher.Dispose();

                            var count = server.Read(buffer, 0, buffer.Length);
                            var request = Encoding.UTF32.GetString(buffer, 0, count);
                            Console.WriteLine($"-> Request: {request}");

                            arguments = request.Split('|');
                            options = Options.Parse(arguments);

                            if (options.Changes.Length == 0 || Change(options.Changes))
                            {
                                logger.Clear();
                                Console.WriteLine($"-> Generate...");
                                RunAsync(logger, options, arguments).Wait();
                            }

                            response = "Success";
                        }
                        catch (Exception exception) { response = exception.ToString(); }
                        finally
                        {
                            Console.WriteLine($"-> Response: {response}");

                            var count = Encoding.UTF32.GetBytes(response, 0, response.Length, buffer, 0);
                            server.Write(buffer, 0, count);

                            Console.WriteLine($"-> Wait For Pipe Drain");
                            server.WaitForPipeDrain();

                            Console.WriteLine($"-> Disconnect");
                            server.Disconnect();
                            watcher = Watch(logger, options, arguments);
                            Console.WriteLine($"-> Done");
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        static async Task RunAsync(StringBuilder logger, Options options, string[] arguments)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                logger.AppendLine(DateTime.Now.ToString());
                logger.AppendLine();
                logger.AppendLine("Arguments:");
                arguments.Iterate(argument => logger.AppendLine($"\t{argument}"));
                logger.AppendLine();

                var assembliesTask = Task.Run(() => options.Assemblies
                    .Where(assembly => !string.IsNullOrWhiteSpace(assembly))
                    .SelectMany(assembly => assembly.Files("*.dll", SearchOption.TopDirectoryOnly))
                    .Distinct()
                    .ToArray());
                var currentFilesTask = Task.Run(() => options.Output.Files("*.cs", SearchOption.AllDirectories).ToArray());
                var inputFilesTask = Task.Run(() => options.Inputs
                    .Where(input => !string.IsNullOrWhiteSpace(input))
                    .SelectMany(input => input.Files("*.cs", SearchOption.AllDirectories))
                    .Distinct()
                    .ToArray());

                var assemblies = await assembliesTask;
                var currentFiles = await currentFilesTask;
                var inputFiles = await inputFilesTask;
                inputFiles = inputFiles.Except(currentFiles).ToArray();

                var result = await Generator.Generate(options.Suffix, Directory.GetCurrentDirectory(), inputFiles, currentFiles, assemblies);
                var outputFiles = result.Generated
                    .Select(generated => (path: AsPath(generated.type), generated.code))
                    .ToArray();
                var renamedFiles = result.Renamed
                    .Select(pair => (from: AsPath(pair.from), to: AsPath(pair.to)))
                    .ToArray();

                logger.AppendLine("Assemblies:");
                assemblies.Iterate(assembly => logger.AppendLine($"\t{assembly}"));
                logger.AppendLine();

                logger.AppendLine("Current:");
                currentFiles.Iterate(current => logger.AppendLine($"\t{current}"));
                logger.AppendLine();

                logger.AppendLine("Inputs:");
                inputFiles.Iterate(input => logger.AppendLine($"\t{input}"));
                logger.AppendLine();

                logger.AppendLine("Outputs:");
                outputFiles.Iterate(pair => logger.AppendLine($"\t{pair.path}"));
                logger.AppendLine();

                AppendLines(Rename(renamedFiles).ToArray());

                var createTask = Task.Run(() => Create(outputFiles).ToArray());
                var deleteTask = Task.Run(() => Delete(currentFiles, outputFiles).ToArray());

                AppendLines(await deleteTask);
                AppendLines(await createTask);
                AppendLines(Clean(options.Output).ToArray());
            }
            catch (Exception exception) { Except(exception); }

            Elapsed();
            Log();

            void AppendLines(params string[] lines)
            {
                foreach (var line in lines) logger.AppendLine(line);
                logger.AppendLine();
            }

            string AsPath(string[] type) => Path.Combine(type.Prepend(options.Output).ToArray()).ChangeExtension(".cs").Absolute();

            void Except(Exception exception)
            {
                logger.AppendLine();
                logger.AppendLine("Exception:");
                logger.AppendLine(exception.ToString());
                logger.AppendLine();
            }

            void Elapsed()
            {
                logger.AppendLine();
                logger.Append($"Elapsed: {watch.Elapsed}");
                logger.AppendLine();
            }

            void Log()
            {
                if (string.IsNullOrWhiteSpace(options.Log)) return;
                Write(Path.Combine(options.Output, options.Log), logger.ToString());
            }
        }

        static bool Write(string path, string text)
        {
            var created = !File.Exists(path);
            Directory.CreateDirectory(path.Directory());
            File.WriteAllText(path, text);
            _writes[path] = File.GetLastWriteTimeUtc(path);
            return created;
        }

        static bool Move(string from, string to)
        {
            if (File.Exists(from))
            {
                Directory.CreateDirectory(to.Directory());
                File.Move(from, to);
                _writes[to] = File.GetLastWriteTimeUtc(to);
                return true;
            }

            return false;
        }

        static IEnumerable<string> Clean(string path)
        {
            yield return "Cleaned:";

            var directories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories)
                .Where(directory => Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).None())
                .ToArray();

            foreach (var directory in directories)
            {
                Directory.Delete(directory);
                yield return $"\t{directory}";
            }

            yield return "";
        }

        static IEnumerable<string> Rename((string from, string to)[] renamed)
        {
            yield return "Renamed:";
            foreach (var (from, to) in renamed)
            {
                if (Move(from, to)) yield return $"\t{from} -> {to}";

                var fromMeta = from + ".meta";
                var toMeta = to + ".meta";
                if (Move(fromMeta, toMeta)) yield return $"\t{fromMeta} -> {toMeta}";
            }
            yield return "";
        }

        static IEnumerable<string> Create((string path, string code)[] outputs)
        {
            yield return "Created:";
            foreach (var output in outputs) if (Write(output.path, output.code)) yield return $"\t{output.path}";
            yield return "";
        }

        static IEnumerable<string> Delete(string[] files, (string path, string code)[] outputs)
        {
            yield return "Deleted:";
            foreach (var file in files.Except(outputs.Select(pair => pair.path)))
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    yield return $"\t{file}";
                }

                var meta = file + ".meta";
                if (File.Exists(meta))
                {
                    File.Delete(meta);
                    yield return $"\t{meta}";
                }

            }
            yield return "";
        }
    }
}
