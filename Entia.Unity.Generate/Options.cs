using System;
using System.Linq;

namespace Entia.Unity
{
    public sealed class Options
    {
        public string[] Inputs { get; private set; } = { };
        public string Output { get; private set; } = "";
        public string[] Assemblies { get; private set; } = { };
        public string[] Changes { get; private set; } = { };
        public string Suffix { get; private set; } = "";
        public string Log { get; private set; } = "";
        public TimeSpan Timeout { get; private set; } = TimeSpan.MaxValue;
        public (int process, long ticks, bool files, string pipe) Watch { get; private set; } = (0, 0L, false, "");

        public static Options Parse(params string[] arguments)
        {
            var options = new Options();
            Parse(options, arguments);
            return options;
        }

        public static void Parse(Options options, params string[] arguments)
        {
            void Next(int index)
            {
                if (index > arguments.Length - 1) return;

                Next(index + 1);
                switch (arguments[index])
                {
                    case "--inputs": options.Inputs = arguments[index + 1].Split(';').Distinct().ToArray(); break;
                    case "--output": options.Output = arguments[index + 1]; break;
                    case "--assemblies": options.Assemblies = arguments[index + 1].Split(';').Distinct().ToArray(); break;
                    case "--changes": options.Changes = arguments[index + 1].Split(";").Distinct().ToArray(); break;
                    case "--suffix": options.Suffix = arguments[index + 1]; break;
                    case "--log": options.Log = arguments[index + 1]; break;
                    case "--timeout":
                        if (TimeSpan.TryParse(arguments[index + 1], out var span)) options.Timeout = span;
                        break;
                    case "--watch":
                        var splits = arguments[index + 1].Split(";");
                        if (splits.Length == 4 && int.TryParse(splits[0], out var process) && long.TryParse(splits[1], out var ticks) && bool.TryParse(splits[2], out var files))
                            options.Watch = (process, ticks, files, splits[3]);
                        break;
                }
            }

            Next(0);
        }
    }
}
