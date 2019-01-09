using System;
using UnityEngine;

namespace Entia.Unity
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class TypeEnumAttribute : PropertyAttribute
	{
		[Flags]
		public enum Filters
		{
			None,
			IsInterface = 1 << 0,
			IsNotInterface = 1 << 1,
			IsClass = 1 << 2,
			IsNotClass = 1 << 3,
			IsStruct = 1 << 4,
			IsNotStruct = 1 << 5,
			IsAbstract = 1 << 6,
			IsNotAbstract = 1 << 7,
			IsGeneric = 1 << 8,
			IsNotGeneric = 1 << 9
		}

		public Type Default;
		public Filters Filter;
		public Type[] Implementations;
		public string Regex;
	}
}