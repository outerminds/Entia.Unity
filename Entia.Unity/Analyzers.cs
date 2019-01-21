using Entia.Analyzers;
using Entia.Core;
using Entia.Dependencies;
using Entia.Modules;
using Entia.Nodes;
using System.Linq;

namespace Entia.Analyzers
{
    public sealed class Parallel : Analyzer<Entia.Nodes.Parallel>
    {
        Result<Unit> Unity(Node node, IDependency[] dependencies) =>
            dependencies.OfType<Dependencies.Unity>().Select(_ => Result.Failure($"'{node}' depends on Unity.")).All();

        public override Result<IDependency[]> Analyze(Entia.Nodes.Parallel data, Node node, Node root, World world) =>
            node.Children.Select(child => world.Analyzers().Analyze(child, root).Map(dependencies => (child, dependencies))).All()
                .Bind(children => children.Select(pair => Unity(pair.child, pair.dependencies)).All())
                .Right(world.Analyzers().Default<Entia.Nodes.Parallel>().Analyze(node, root, world));
    }
}
