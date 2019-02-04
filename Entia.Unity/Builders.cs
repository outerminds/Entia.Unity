using Entia.Builders;
using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Modules.Build;
using Entia.Modules.Control;
using Entia.Nodes;
using Entia.Phases;
using Entia.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace Entia.Builders
{
    public sealed class Parallel : IBuilder
    {
        public Result<IRunner> Build(Node node, Node root, World world) => node.Children.Length == 1 ?
            Result.Cast<Nodes.Parallel>(node.Value).Bind(_ => world.Builders().Build(node.Children[0], root)) :
            Result.Cast<Nodes.Parallel>(node.Value)
                .Bind(_ => node.Children.Select(child => world.Builders().Build(child, root)).All())
                .Map(children => new Runners.Parallel(children))
                .Cast<IRunner>();
    }

    public sealed class Profile : Builder<Runners.Profile>
    {
        readonly Dictionary<string, int> _names = new Dictionary<string, int>();

        public override Result<Runners.Profile> Build(Node node, Node root, World world) => Result.Cast<Nodes.Profile>(node.Value)
            .Bind(_ => world.Builders().Build(Node.Sequence(node.Children), root))
            .Map(child =>
            {
                var map = new TypeMap<IPhase, (CustomSampler, Recorder)>();
                foreach (var phase in child.Phases())
                {
                    var name = $"{phase.Format()}.{node.Name}";
                    var index = _names[name] = _names.TryGetValue(name, out var value) ? ++value : 0;
                    var sampler = CustomSampler.Create($"{name}[{index}]");
                    var recorder = sampler.GetRecorder();
                    map[phase] = (sampler, recorder);
                }
                return new Runners.Profile(map, node, child);
            });
    }
}