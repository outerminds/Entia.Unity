namespace Entia.Nodes
{
    public static class NodeExtensions
    {
        public static Node Editor(this Node node) => node.Wrap(new Editor());
    }
}