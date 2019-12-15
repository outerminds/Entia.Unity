using Entia.Build;
using Entia.Builders;
using Entia.Core;

namespace Entia.Nodes
{
    public readonly struct Editor : IWrapper, IImplementation<Editor.Builder>
    {
        sealed class Builder : Builder<Editor>
        {
            public override Result<IRunner> Build(in Editor data, in Context context) =>
                context.Build(Node.Sequence(context.Node.Name, context.Node.Children));
        }

        public readonly bool Only;
        public Editor(bool only) { Only = only; }
    }
}