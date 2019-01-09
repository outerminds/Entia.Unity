using Entia;
using Entia.Phases;
using Entia.Segments;

namespace Phases
{
    public struct Phase1 : IPhase { }

    namespace Inner
    {
        public struct Phase2 : IPhase { }
    }
}