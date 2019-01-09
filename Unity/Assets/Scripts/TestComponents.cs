using System;
using Entia;
using Entia.Core;
using Entia.Unity;
using UnityEngine;

namespace Components
{
    public struct Component1 : IComponent { }

    namespace Inner
    {
        public struct Component2 : IComponent { }
    }
}