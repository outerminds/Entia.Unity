using Entia.Core;
using Entia.Dependencies;

namespace Entia.Dependencies
{
    public readonly struct Unity : IDependency
    {
        public override string ToString() => GetType().Format();
    }
}
