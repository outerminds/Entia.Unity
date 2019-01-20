using System.Linq;

namespace Entia.Unity
{
    public sealed class Options
    {
        public string[] Inputs { get; private set; } = { };
        public string Output { get; private set; } = "";
        public string[] Assemblies { get; private set; } = { };
        public string Suffix { get; private set; } = "";
        public string Log { get; private set; } = "";
        public string Link { get; private set; } = "";
        public (int process, string pipe) Watch { get; private set; } = (0, "");

        public static Options Parse(params string[] arguments)
        {
            Options Next(int index)
            {
                if (index > arguments.Length - 1) return new Options();

                var next = Next(index + 1);
                switch (arguments[index])
                {
                    case "--inputs": next.Inputs = arguments[index + 1].Split(';').ToArray(); break;
                    case "--output": next.Output = arguments[index + 1]; break;
                    case "--assemblies": next.Assemblies = arguments[index + 1].Split(';'); break;
                    case "--suffix": next.Suffix = arguments[index + 1]; break;
                    case "--log": next.Log = arguments[index + 1]; break;
                    case "--link": next.Link = arguments[index + 1]; break;
                    case "--watch":
                        var splits = arguments[index + 1].Split(";");
                        if (splits.Length == 2 && int.TryParse(splits[0], out var process)) next.Watch = (process, splits[1]);
                        break;
                    default: return next;
                }
                return next;
            }

            return Next(0);
        }
    }
}
