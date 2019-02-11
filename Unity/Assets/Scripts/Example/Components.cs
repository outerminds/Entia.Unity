using Entia;
using Entia.Unity;

namespace Components
{
    // Components are simple structs that implement the empty 'IComponent' interface.
    public struct Velocity : IComponent { public float X, Y; }

    // Components may be empty to act as a tag on an entity.
    public struct IsFrozen : IComponent { }

    public struct Physics : IComponent
    {
        // Since structs can not have default values, the 'Default' attribute will 
        // cause the generator to generate the default values on the 
        // 'ComponentReference'.
        [Default(1f)]
        public float Mass;
        [Default(3f)]
        public float Drag;
        [Default(-2f)]
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
        [Default(2f)]
        public float Acceleration;
        [Default(0.25f)]
        public float MaximumSpeed;
        [Default(0.75f)]
        public float JumpForce;
    }
}