using Entia.Schedulers;
using Entia.Systems;

namespace Entia.Unity.Systems
{
	public interface IRunFixed : ISystem, ISchedulable<Schedulers.RunFixed>
	{
		void RunFixed();
	}

	public interface IRunLate : ISystem, ISchedulable<Schedulers.RunLate>
	{
		void RunLate();
	}
}
