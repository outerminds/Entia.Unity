namespace Entia.Nodes
{
    public static class NodeExtensions
    {
        public static Node Editor(this Node node, bool only = false) => node.Wrap(new Editor(only));
    }
}