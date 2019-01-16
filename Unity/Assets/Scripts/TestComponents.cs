using System;
using Entia;
using Entia.Core;
using Entia.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    public struct Component1 : IComponent
    {
        [FormerlySerializedAs("Poulah")]
        public float X;
    }

    namespace Inner
    {
        public struct Component2 : IComponent { }
    }
}