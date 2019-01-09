using Entia.Modules.Control;
using Entia.Modules.Schedule;
using Entia.Schedulers;
using Entia.Unity.Systems;

namespace Entia.Unity.Schedulers
{
    public sealed class RunFixed : Scheduler<IRunFixed>
    {
        public override Phase[] Schedule(IRunFixed instance, Controller controller, World world) =>
            new[] { Phase.From<Phases.RunFixed>(instance.RunFixed) };
    }

    public sealed class RunLate : Scheduler<IRunLate>
    {
        public override Phase[] Schedule(IRunLate instance, Controller controller, World world) =>
            new[] { Phase.From<Phases.RunLate>(instance.RunLate) };
    }
}
