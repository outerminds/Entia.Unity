using Entia;
using Entia.Core;
using Entia.Unity;

namespace Components
{
    // Components are simple structs that implement the empty 'IComponent' interface.
    public struct Position : IComponent { public float X, Y; }
    // Components are simple structs that implement the empty 'IComponent' interface.
    public struct Velocity : IComponent { public float X, Y; }

    // Components may be empty to act as a tag on an entity.
    public struct IsFrozen : IComponent { }

    public struct Physics : IComponent
    {
        // Since structs can not have default values, the 'Default' attribute is
        // used by the framework to create default initialized instances.
        [Default]
        public static Physics Default => new Physics { Mass = 1f, Drag = 3f, Gravity = -2f };

        public float Mass;
        public float Drag;
        public float Gravity;
    }

    public struct Input : IComponent
    {
        // The 'Disable' attribute will make the field read-only in the Unity editor.
        [Disable]
        public float Direction;
        [Disable]
        public bool Jump;
    }

    public struct Motion : IComponent
    {
        [Default]
        public static Motion Default => new Motion { Acceleration = 2f, Speed = 0.25f, Jump = 0.75f };

        public float Acceleration;
        public float Speed;
        public float Jump;
    }
}