using System;

namespace Entia.Unity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DefaultAttribute : Attribute
    {
        public readonly object Value;
        public DefaultAttribute(object value) { Value = value; }
    }
}
