using System;
using System.Collections.Generic;
using Entia.Modules.Control;
using Entia.Modules.Schedule;
using Entia.Schedulers;
using Entia.Systems;
using Entia.Unity.Systems;

namespace Entia.Schedulers
{
    public sealed class RunFixed : Scheduler<IRunFixed>
    {
        public override IEnumerable<Type> Phases => new[] { typeof(Phases.RunFixed) };

        public override IEnumerable<Phase> Schedule(IRunFixed instance, Controller controller) =>
            new[] { Phase.From<Phases.RunFixed>(instance.RunFixed) };
    }

    public sealed class RunLate : Scheduler<IRunLate>
    {
        public override IEnumerable<Type> Phases => new[] { typeof(Phases.RunLate) };

        public override IEnumerable<Phase> Schedule(IRunLate instance, Controller controller) =>
            new[] { Phase.From<Phases.RunLate>(instance.RunLate) };
    }
}
