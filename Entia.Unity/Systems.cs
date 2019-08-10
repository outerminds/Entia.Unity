using Entia.Core;

namespace Entia.Systems
{
    public interface IRunFixed : ISystem, IImplementation<Schedulers.RunFixed>
    {
        void RunFixed();
    }

    public interface IRunLate : ISystem, IImplementation<Schedulers.RunLate>
    {
        void RunLate();
    }
}
