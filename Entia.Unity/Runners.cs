#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Linq;
using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Modules.Build;
using Entia.Modules.Control;
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
        public IEnumerable<Phase> Phases(Controller controller) => Children.SelectMany(child => child.Phases(controller));
        public Option<Runner<T>> Specialize<T>(Controller controller) where T : struct, IPhase
        {
            var children = (items: new Runner<T>[Children.Length], count: 0);
            foreach (var child in Children)
                if (child.Specialize<T>(controller).TryValue(out var special)) children.Push(special);

            switch (children.count)
            {
                case 0: return Option.None();
                case 1: return children.items[0];
                default:
                    var runners = children.ToArray();
                    return new Runner<T>((in T phase) =>
                        JobUtility.Parallel(
                            (phase, runners),
                            (in (T phase, Runner<T>[] runners) state, int index) => state.runners[index].Run(state.phase))
                        .Schedule(runners.Length, 1)
                        .Complete());
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
        public IEnumerable<Phase> Phases(Controller controller) => Child.Phases(controller);
        public Option<Runner<T>> Specialize<T>(Controller controller) where T : struct, IPhase
        {
            if (Child.Specialize<T>(controller).TryValue(out var child))
            {
                if (Map.TryGet<T>(out var pair))
                {
                    var (sampler, recorder) = pair;
                    var messages = controller.World.Messages();
                    return new Runner<T>((in T phase) =>
                    {
                        sampler.Begin();
                        child.Run(phase);
                        sampler.End();
                        messages.Emit(new OnProfile { Runner = this, Phase = typeof(T), Elapsed = TimeSpan.FromTicks(recorder.elapsedNanoseconds / 100) });
                    });
                }
                return child;
            }
            return Option.None();
        }
    }
}