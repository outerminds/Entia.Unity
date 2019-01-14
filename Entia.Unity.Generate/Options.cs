using System.Linq;

namespace Entia.Unity
{
    public class Options
    {
        public string[] Inputs { get; private set; } = { };
        public string Output { get; private set; } = "";
        public string[] Assemblies { get; private set; } = { };
        public string Suffix { get; private set; } = "";
        public string Log { get; private set; } = "";
        public string Link { get; private set; } = "";
        public string Watch { get; private set; } = "";

        public static Options Parse(params string[] arguments)
        {
            Options Next(int index)
            {
                if (index > arguments.Length - 1) return new Options();

                var next = Next(index + 1);
                switch (arguments[index])
                {
                    case "-i":
                    case "--inputs": next.Inputs = arguments[index + 1].Split(';').ToArray(); break;
                    case "-o":
                    case "--output": next.Output = arguments[index + 1]; break;
                    case "-a":
                    case "--assemblies": next.Assemblies = arguments[index + 1].Split(';'); break;
                    case "-s":
                    case "--suffix": next.Suffix = arguments[index + 1]; break;
                    case "-p":
                    case "--log": next.Log = arguments[index + 1]; break;
                    case "--link": next.Link = arguments[index + 1]; break;
                    case "-w":
                    case "--watch": next.Watch = arguments[index + 1]; break;
                    default: return next;
                }
                return next;
            }

            return Next(0);
        }
    }
}
