using System;
using Entia;
using Entia.Core;
using Entia.Queryables;
using Entia.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    public struct Ambiguous : IComponent { }

    public struct Component1 : IComponent
    {
        [Default]
        public static Component1 Default => new Component1 { X = 312f };

        [FormerlySerializedAs("Poulah")]
        [Require]
        public float X;
        [Require]
        public Entity Entity;
        [Require]
        public Entity World;
        [Require]
        public int Component;
        [Require]
        public BoxCollider2D Collider;
        [Require]
        public Generated.IsFrozen Reference1;
        [Require]
        public EntityReference Reference2;
        public (float a, float b) Tuple;
    }

    namespace Inner
    {
        public struct Ambiguous : IComponent { }
        public struct Component7 : IComponent { public float X, A, B, C, D, E, F, G, H, I, J, K; }
        public struct Component2 : IComponent { public long X, A, B, C, D, E, F, G, H, I, J, K; }
        public struct Component3 : IComponent { public double X, A, B, C, D, E, F, G, H, I, J, K; }
        public struct Component4 : IComponent { public uint X, A, B, C, D, E, F, G, H, I, J, K; }
        public struct Component5 : IComponent { public byte X, A, B, C, D, E, F, G, H, I, J, K; }
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