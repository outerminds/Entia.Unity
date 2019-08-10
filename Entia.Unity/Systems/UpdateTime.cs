using Entia.Core;
using Entia.Injectables;
using Entia.Systems;

namespace Entia.Unity.Systems
{
    public readonly struct UpdateTime : IRun, IImplementation<Dependers.Unity>
    {
        public readonly Resource<Resources.Time> Resource;

        void IRun.Run()
        {
            ref var time = ref Resource.Value;
            time.Current = UnityEngine.Time.time;
            time.Delta = UnityEngine.Time.deltaTime;
        }
    }
}
