using Entia.Core;
using UnityEngine;

namespace Entia.Cloners
{
    public sealed class Object : Cloner<UnityEngine.Object>
    {
        public override Result<UnityEngine.Object> Clone(UnityEngine.Object instance, TypeData type, World world) => instance;
    }
}