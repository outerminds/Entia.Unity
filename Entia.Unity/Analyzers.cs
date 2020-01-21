using Entia.Analysis;
using Entia.Core;
using Entia.Dependencies;
using Entia.Nodes;
using System.Linq;

namespace Entia.Analyzers
{
    public sealed class Parallel : Analyzer<Entia.Nodes.Parallel>
    {
        Result<Unit> Unity(Node node, IDependency[] dependencies) => dependencies
            .OfType<Dependencies.Unity>()
            .Select(_ => Result.Failure($"'{node}' depends on Unity.").AsResult())
            .All();

        public override Result<IDependency[]> Analyze(in Nodes.Parallel data, in Context context) => context.Node.Children
            .Select(context, (child, state) => state.Analyze(child).Map(dependencies => (child, dependencies)))
            .All()
            .Bind(children => children
                .Select(pair => Unity(pair.child, pair.dependencies).Return(pair.dependencies))
                .All())
            .Map(dependencies => dependencies.SelectMany(_ => _).Distinct().ToArray());
    }
}
