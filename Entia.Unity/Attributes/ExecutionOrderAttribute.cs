using System;

namespace Entia.Unity
{
    public sealed class ExecutionOrderAttribute : Attribute
    {
        public readonly int Order;
        public ExecutionOrderAttribute(int order) { Order = order; }
    }
}