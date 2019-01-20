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

namespace Entia.Unity
{
    static class Program
    {
        static void Main(string[] arguments)
        {
            var logger = new StringBuilder();
            var options = Options.Parse(arguments);

            Console.WriteLine($"-> Birth: {string.Join(", ", arguments)}");

            if (string.IsNullOrWhiteSpace(options.Watch.pipe)) Run(logger, options, arguments).Wait();
            else
            {
                var cancel = new CancellationTokenSource();
                try
                {
                    Task.Run(() => Run(logger, options), cancel.Token);
                    while (Process.GetProcessById(options.Watch.process) is Process) Thread.Sleep(100);
                }
                catch { }
                finally { cancel.Cancel(); }
            }

            Console.WriteLine($"-> Kill: {string.Join(", ", arguments)}");
        }

        static void Run(StringBuilder logger, Options options)
        {
            var buffer = new byte[4096];
            var response = "";

            Console.WriteLine($"-> Server: {options.Watch}");
            using (var server = new NamedPipeServerStream(options.Watch.pipe, PipeDirection.InOut, 1))
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine($"-> Wait For Connection");
                        server.WaitForConnection();
                        var count = server.Read(buffer, 0, buffer.Length);
                        var request = Encoding.UTF32.GetString(buffer, 0, count);

                        Console.WriteLine($"-> Request: {request}");

                        var arguments = request.Split('|');
                        options = Options.Parse(arguments);
                        logger.Clear();
                        Run(logger, options, arguments).Wait();
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
                        Console.WriteLine();
                    }
                }
            }
        }

        static async Task Run(StringBuilder logger, Options options, string[] arguments)
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
            return created;
        }

        static bool Move(string from, string to)
        {
            if (File.Exists(from))
            {
                Directory.CreateDirectory(to.Directory());
                File.Move(from, to);
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
