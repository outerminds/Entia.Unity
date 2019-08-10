using Entia.Build;
using Entia.Core;
using Entia.Nodes;
using Entia.Phases;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;

namespace Entia.Builders
{
    public sealed class Parallel : Builder<Nodes.Parallel>
    {
        public override Result<IRunner> Build(in Nodes.Parallel data, in Context context)
        {
            var children = context.Node.Children;
            if (children.Length == 1) return context.Build(children[0]);
            return children
                .Select(context, (child, state) => state.Build(child))
                .All()
                .Map(runners => new Runners.Parallel(runners))
                .Cast<IRunner>();
        }
    }

    public sealed class Profile : Builder<Nodes.Profile>
    {
        readonly Dictionary<string, int> _names = new Dictionary<string, int>();

        public override Result<IRunner> Build(in Nodes.Profile data, in Context context) =>
            context.Build(Node.Sequence(context.Node.Children)).Map(context, (child, state) =>
            {
                var node = state.Node;
                var map = new TypeMap<IPhase, (CustomSampler, Recorder)>();
                var index = _names[node.Name] = _names.TryGetValue(node.Name, out var value) ? ++value : 0;
                foreach (var phase in child.Phases().Distinct())
                {
                    var sampler = CustomSampler.Create($"{node.Name}.{phase.Format()}[{index}]");
                    var recorder = sampler.GetRecorder();
                    map[phase] = (sampler, recorder);
                }
                return new Runners.Profile(map, node, child);
            }).Cast<IRunner>();
    }
}