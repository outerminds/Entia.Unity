#define ENABLE_PROFILER
using Entia.Builders;
using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Modules.Build;
using Entia.Modules.Control;
using Entia.Nodes;
using Entia.Phases;
using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace Entia.Unity.Builders
{
    public sealed class Parallel : IBuilder
    {
        public Option<Runner<T>> Build<T>(Node node, Controller controller, World world) where T : struct, IPhase
        {
            var children = new List<Runner<T>>(node.Children.Length);
            foreach (var child in node.Children)
            {
                if (world.Builders().Build<T>(child, controller).TryValue(out var current))
                    children.Add(current);
            }

            var runners = children.ToArray();
            switch (runners.Length)
            {
                case 0: return Option.None();
                case 1: return runners[0];
                default:
                    return new Runner<T>(runners, (in T phase) =>
                        JobUtility.Parallel(
                            (phase, runners),
                            (in (T phase, Runner<T>[] runners) state, int index) => state.runners[index].Run(state.phase))
                        .Schedule(runners.Length, 1)
                        .Complete());
            }
        }
    }

    public sealed class Profile : IBuilder
    {
        readonly Dictionary<string, int> _names = new Dictionary<string, int>();

        public Option<Runner<T>> Build<T>(Node node, Controller controller, World world) where T : struct, IPhase
        {
            if (world.Builders().Build<T>(Node.Sequence(node.Name, node.Children), controller).TryValue(out var runner))
            {
                var name = $"{typeof(T).Format()}.{node.Name}";
                var index = _names[name] = _names.TryGetValue(name, out var value) ? ++value : 0;
                var sampler = CustomSampler.Create($"{name}[{index}]");
                var recorder = sampler.GetRecorder();
                var messages = world.Messages();
                return new Runner<T>(
                    runner,
                    (in T phase) =>
                    {
                        sampler.Begin();
                        runner.Run(phase);
                        sampler.End();
                        messages.Emit(new OnProfile { Node = node, Phase = typeof(T), Elapsed = TimeSpan.FromTicks(recorder.elapsedNanoseconds / 100) });
                    });
            }

            return Option.None();
        }
    }
}