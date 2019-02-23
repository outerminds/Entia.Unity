using Entia.Core;
using Entia.Initializers;
using Entia.Modules;

namespace Entia.Initializers
{
    sealed class EntityReference : Initializer<Entia.Entity>
    {
        public readonly int[] Components;
        public readonly World World;

        public EntityReference(int[] components, World world)
        {
            Components = components;
            World = world;
        }

        public override Result<Unit> Initialize(Entia.Entity instance, object[] instances)
        {
            var components = World.Components();
            components.Clear(instance);

            foreach (var component in Components)
            {
                var result = Result.Cast<IComponent>(instances[component]);
                if (result.TryValue(out var value)) components.Set(instance, value);
                else return result;
            }

            return Result.Success();
        }
    }

    public sealed class GameObject : Initializer<UnityEngine.GameObject>
    {
        public readonly string Name;
        public readonly string Tag;
        public readonly int Layer;
        public readonly bool Active;
        public readonly int[] Children;

        public GameObject(string name, string tag, int layer, bool active, params int[] children)
        {
            Name = name;
            Tag = tag;
            Layer = layer;
            Active = active;
            Children = children;
        }

        public override Result<Unit> Initialize(UnityEngine.GameObject instance, object[] instances)
        {
            var parent = instance.transform;
            foreach (var index in Children)
            {
                var result = Result.Cast<UnityEngine.GameObject>(index);
                if (result.TryValue(out var child)) child.transform.SetParent(parent);
                else return result;
            }

            instance.name = Name;
            instance.tag = Tag;
            instance.layer = Layer;
            instance.SetActive(Active);
            return Result.Success();
        }
    }
}
