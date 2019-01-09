using System;
using UnityEngine;

namespace Entia.Unity
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ProxyAttribute : Attribute { }
}