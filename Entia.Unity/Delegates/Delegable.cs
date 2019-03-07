using Entia.Delegates;

namespace Entia.Delegables
{
    public interface IDelegable<T> where T : IDelegate, new() { }
}