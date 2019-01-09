using Entia.Core;
using Entia.Instantiators;
using System;

namespace Entia.Unity.Instantiators
{
	public sealed class GameObject : IInstantiator
	{
		public Result<object> Instantiate(object[] instances)
		{
			var instance = new UnityEngine.GameObject();
			instance.SetActive(false);
			return instance;
		}
	}

	public sealed class Component : IInstantiator
	{
		public readonly Type Type;
		public readonly int Index;

		public Component(Type type, int index)
		{
			Type = type;
			Index = index;
		}

		public Result<object> Instantiate(object[] instances)
		{
			var result = Result.Cast<UnityEngine.GameObject>(instances[Index]);
			if (result.TryFailure(out var failure)) return failure;
			if (result.TryValue(out var value)) return value.AddComponent(Type);
			return Result.Failure();
		}
	}

	public sealed class Transform : IInstantiator
	{
		public readonly int Index;
		public Transform(int index) { Index = index; }

		public Result<object> Instantiate(object[] instances)
		{
			var result = Result.Cast<UnityEngine.GameObject>(instances[Index]);
			if (result.TryFailure(out var failure)) return failure;
			if (result.TryValue(out var value)) return value.transform;
			return Result.Failure();
		}
	}
}
