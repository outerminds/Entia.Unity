using Unity.Jobs;

namespace Entia.Unity
{
    public delegate void ParallelExecute(int index);
    public delegate void ParallelExecute<T>(in T state, int index);

    public readonly struct ParallelJob<T> : IJobParallelFor
    {
        readonly T _state;
        readonly ParallelExecute<T> _execute;

        public ParallelJob(in T state, ParallelExecute<T> execute)
        {
            _state = state;
            _execute = execute;
        }

        public void Execute(int index) => _execute(_state, index);
    }

    public readonly struct ParallelJob : IJobParallelFor
    {
        readonly ParallelExecute _execute;
        public ParallelJob(ParallelExecute execute) { _execute = execute; }
        public void Execute(int index) => _execute(index);
    }

    public static class JobUtility
    {
        public static ParallelJob<T> Parallel<T>(T state, ParallelExecute<T> execute) => new ParallelJob<T>(state, execute);
        public static ParallelJob Parallel(ParallelExecute execute) => new ParallelJob(execute);
    }
}
