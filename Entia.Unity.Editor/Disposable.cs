using System;

namespace Entia.Unity.Editor
{
	public readonly struct Disposable : IDisposable
	{
		public static readonly Disposable Empty = new Disposable(() => { });

		readonly Action _dispose;

		public Disposable(Action dispose) { _dispose = dispose; }

		public void Dispose() => _dispose();
	}
}
