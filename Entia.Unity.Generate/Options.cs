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
        public (int process, long ticks, string pipe) Watch { get; private set; } = (0, 0L, "");

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
                    case "--inputs": options.Inputs = arguments[index + 1].Split(';').ToArray(); break;
                    case "--output": options.Output = arguments[index + 1]; break;
                    case "--assemblies": options.Assemblies = arguments[index + 1].Split(';'); break;
                    case "--changes": options.Changes = arguments[index + 1].Split(";"); break;
                    case "--suffix": options.Suffix = arguments[index + 1]; break;
                    case "--log": options.Log = arguments[index + 1]; break;
                    case "--watch":
                        var splits = arguments[index + 1].Split(";");
                        if (splits.Length == 3 && int.TryParse(splits[0], out var process) && long.TryParse(splits[1], out var ticks))
                            options.Watch = (process, ticks, splits[2]);
                        break;
                }
            }

            Next(0);
        }
    }
}
