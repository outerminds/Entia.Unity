using System;

namespace Entia.Unity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RequireAttribute : Attribute { }
}