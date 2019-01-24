using System;
using Entia;
using Entia.Core;
using Entia.Queryables;
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

    public struct Link : IComponent
    {
        [All(typeof(IComponent))]
        public Entity HasComponent;
        [None(typeof(IComponent))]
        public Entity NoComponent;
        [All(typeof(Entia.Components.Unity<>))]
        public Entity HasUnity;
        [None(typeof(Entia.Components.Unity<>))]
        public Entity NoUnity;
    }
}