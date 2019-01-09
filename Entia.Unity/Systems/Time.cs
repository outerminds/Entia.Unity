using Entia.Dependables;
using Entia.Injectables;
using Entia.Systems;

namespace Entia.Unity.Systems
{
	public struct Time : IRun, IDepend<Dependencies.Unity>
	{
		public Resource<Resources.Time> Resource;

		void IRun.Run()
		{
			ref var time = ref Resource.Value;
			time.Current = UnityEngine.Time.time;
			time.Delta = UnityEngine.Time.deltaTime;
		}
	}
}
