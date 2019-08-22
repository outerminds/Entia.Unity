#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Linq;
using Entia.Build;
using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Modules.Schedule;
using Entia.Nodes;
using Entia.Phases;
using Entia.Unity;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace Entia.Runners
{
    public sealed class Parallel : IRunner
    {
        public object Instance => Children;
        public readonly IRunner[] Children;
        public Parallel(params IRunner[] children) { Children = children; }

        public IEnumerable<Type> Phases() => Children.SelectMany(child => child.Phases());
        public IEnumerable<Phase> Schedule(Controller controller) => Children.SelectMany(child => child.Schedule(controller));
        public Option<Run<T>> Specialize<T>(Controller controller) where T : struct, IPhase
        {
            var children = Children.Select(controller, (child, state) => child.Specialize<T>(state)).Choose().ToArray();
            switch (children.Length)
            {
                case 0: return Option.None();
                case 1: return children[0];
                default:
                    void Run(in T phase) =>
                        JobUtility.Parallel(
                            (phase, children),
                            (in (T phase, Run<T>[] runs) state, int index) => state.runs[index](state.phase))
                        .Schedule(children.Length, 1)
                        .Complete();
                    return new Run<T>(Run);
            }
        }
    }

    public sealed class Profile : IRunner
    {
        public object Instance => Child;
        public readonly TypeMap<IPhase, (CustomSampler sampler, Recorder recorder)> Map;
        public readonly IRunner Child;
        public Profile(TypeMap<IPhase, (CustomSampler, Recorder)> map, Node node, IRunner child) { Map = map; Child = child; }

        public IEnumerable<Type> Phases() => Child.Phases();
        public IEnumerable<Phase> Schedule(Controller controller) => Child.Schedule(controller);
        public Option<Run<T>> Specialize<T>(Controller controller) where T : struct, IPhase
        {
            if (Child.Specialize<T>(controller).TryValue(out var child))
            {
                if (Map.TryGet<T>(out var pair, false, false))
                {
                    var (sampler, recorder) = pair;
                    var messages = controller.World.Messages();
                    void Run(in T phase)
                    {
                        sampler.Begin();
                        child(phase);
                        sampler.End();
                        messages.Emit(new OnProfile { Runner = this, Phase = typeof(T), Elapsed = TimeSpan.FromTicks(recorder.elapsedNanoseconds / 100) });
                    }
                    return new Run<T>(Run);
                }
                return child;
            }
            return Option.None();
        }
    }
}