using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Systems
{
    // An 'IRun' system will be called on every 'Update'.
    // A system may implement as many system interfaces as needed.
    // See the 'Entia.Systems' namespace for more system interfaces.
    public struct UpdateInput : IRun
    {
        // Groups allow to efficiently retrieve a subset of entities that correspond
        // to a given query represented by the generic types of the group.
        // This group will hold every entity that has a 'Components.Input'
        // component and will give write access to it.
        public readonly Group<Write<Components.Input>> Group;

        public void Run()
        {
            foreach (var item in Group)
            {
                // The 'item.Input()' extension method is generated by the generator.
                ref var input = ref item.Input();
                input.Direction = Input.GetAxis("Horizontal");
                input.Jump = Input.GetKeyDown(KeyCode.UpArrow);
            }
        }
    }

    public struct UpdateVelocity : IRun
    {
        // Queries can also be defined as a separate type which is convenient
        // for large queries.
        // The 'None' attribute means that all entities that have a
        // 'Components.IsFrozen' component will be excluded from the query results.
        [None(typeof(Components.IsFrozen))]
        public readonly struct Query : IQueryable
        {
            // A queryable can only hold queryable fields. They can not hold
            // components directly.
            public readonly Write<Components.Velocity> Velocity;
            public readonly Read<Components.Motion> Motion;
            public readonly Read<Components.Physics> Mass;
            public readonly Read<Components.Input> Input;
        }

        public readonly Group<Query> Group;

        // Resources hold world-global data.
        // 'Entia.Resources.Time' is a library resource that holds
        // Unity's 'Time.deltaTime'.
        public readonly Resource<Entia.Resources.Time>.Read Time;

        public void Run()
        {
            ref readonly var time = ref Time.Value;
            foreach (ref readonly var item in Group)
            {
                // Unpack the group item using generated extensions.
                ref var velocity = ref item.Velocity();
                ref readonly var motion = ref item.Motion();
                ref readonly var physics = ref item.Physics();
                ref readonly var input = ref item.Input();

                var drag = 1f - physics.Drag * time.Delta;
                velocity.X *= drag;
                velocity.Y *= drag;

                var move = (input.Direction * motion.Acceleration) / physics.Mass;
                velocity.X += move * time.Delta;

                if (input.Jump) velocity.Y += motion.Jump / physics.Mass;
                velocity.Y += physics.Gravity * time.Delta;

                // Clamp horizontal velocity.
                if (velocity.X < -motion.Speed)
                    velocity.X = -motion.Speed;
                if (velocity.X > motion.Speed)
                    velocity.X = motion.Speed;
            }
        }
    }

    public struct UpdatePosition : IRun
    {
        // 'Unity<Transform>' is a Unity-specific query that gives access
        // to core Unity components.
        // Note that it will not work for custom 'MonoBehaviour' types.
        public readonly Group<Unity<Transform>, Write<Components.Velocity>> Group;

        public void Run()
        {
            foreach (ref readonly var item in Group)
            {
                // Unpack the group item using generated extensions.
                var transform = item.Transform();
                ref var velocity = ref item.Velocity();

                var position = transform.position;
                position += new Vector3(velocity.X, velocity.Y);

                // Fake a floor.
                if (position.y < 0)
                {
                    position.y = 0;
                    velocity.Y = 0;
                }

                transform.position = position;
            }
        }
    }
}