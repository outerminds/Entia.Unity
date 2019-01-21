using System;
using Entia.Core;

namespace Entia.Unity.Generation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class GeneratedAttribute : Attribute
    {
        public string Link;
        public string[] Path;
        public Type Type;
    }
}