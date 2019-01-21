using System;
using Entia.Core;

namespace Entia.Unity.Generation
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class GeneratorAttribute : PreserveAttribute
    {
        public string[] Path;
        public bool Ignore;
    }
}