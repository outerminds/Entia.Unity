using System;
using UnityEngine;

namespace Entia.Unity
{
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DisableAttribute : PropertyAttribute { }
}