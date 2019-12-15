using Entia;
using Entia.Core;
using Entia.Unity;
using UnityEngine;

namespace Components
{
    // Components are simple structs that implement the empty 'IComponent' interface.
    public struct Position : IComponent
    {
        public static implicit operator Vector2(Position position) => new Vector2(position.X, position.Y);
        public static implicit operator Vector3(Position position) => new Vector3(position.X, position.Y);
        public static implicit operator Position(Vector2 vector) => new Position { X = vector.x, Y = vector.y };
        public static implicit operator Position(Vector3 vector) => new Position { X = vector.x, Y = vector.y };

        public float X, Y;
    }
    public struct Velocity : IComponent
    {
        public static implicit operator Vector2(Velocity velocity) => new Vector2(velocity.X, velocity.Y);
        public static implicit operator Vector3(Velocity velocity) => new Vector3(velocity.X, velocity.Y);
        public static implicit operator Velocity(Vector2 vector) => new Velocity { X = vector.x, Y = vector.y };
        public static implicit operator Velocity(Vector3 vector) => new Velocity { X = vector.x, Y = vector.y };

        public float X, Y;
    }

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