using Entia;
using Entia.Phases;

namespace Phases
{
    public struct Phase1 : IPhase { }

    namespace Inner
    {
        public struct Phase2 : IPhase { }
    }
}