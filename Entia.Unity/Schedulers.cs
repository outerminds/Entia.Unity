using System;
using Entia.Modules.Schedule;
using Entia.Scheduling;
using Entia.Systems;

namespace Entia.Schedulers
{
    public sealed class RunFixed : Scheduler<IRunFixed>
    {
        public override Type[] Phases => new[] { typeof(Phases.RunFixed) };
        public override Phase[] Schedule(IRunFixed instance, in Context context) =>
            new[] { Phase.From<Phases.RunFixed>(instance.RunFixed) };
    }

    public sealed class RunLate : Scheduler<IRunLate>
    {
        public override Type[] Phases => new[] { typeof(Phases.RunLate) };
        public override Phase[] Schedule(IRunLate instance, in Context context) =>
            new[] { Phase.From<Phases.RunLate>(instance.RunLate) };
    }
}
