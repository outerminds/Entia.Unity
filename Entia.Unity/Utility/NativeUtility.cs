using Entia.Core;
using System;
using Unity.Collections;

namespace Entia.Unity
{
	public static class NativeUtility
	{
		public static bool Ensure<T>(ref NativeArray<T> array, int size, Allocator allocator = Allocator.Persistent, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct
		{
			if (size <= array.Length) return false;
			Resize(ref array, MathUtility.NextPowerOfTwo(size + 1), allocator, options);
			return true;
		}

		public static void Resize<T>(ref NativeArray<T> array, int size, Allocator allocator = Allocator.Persistent, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct
		{
			var @new = new NativeArray<T>(size, allocator, options);
			NativeArray<T>.Copy(array, 0, @new, 0, Math.Min(array.Length, @new.Length));
			array.Dispose();
			array = @new;
		}
	}
}
