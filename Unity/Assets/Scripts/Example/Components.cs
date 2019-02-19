using Entia;
using Entia.Core;
using Entia.Unity;

namespace Components
{
    // Components are simple structs that implement the empty 'IComponent' interface.
    public struct Velocity : IComponent { public float X, Y; }

    // Components may be empty to act as a tag on an entity.
    public struct IsFrozen : IComponent { }

    public struct Physics : IComponent
    {
        [Default]
        public static Physics Default => new Physics { Mass = 1f, Drag = 3f, Gravity = -2f };

        // Since structs can not have default values, the 'Default' attribute will 
        // cause the generator to generate the default values on the 
        // 'ComponentReference'.
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
        public static Motion Default => new Motion { Acceleration = 2f, MaximumSpeed = 0.25f, JumpForce = 0.75f };

        public float Acceleration;
        public float MaximumSpeed;
        public float JumpForce;
    }
}