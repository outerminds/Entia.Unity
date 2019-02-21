using System.Collections.Generic;
using UnityEngine;

namespace Entia.Unity
{
    public enum Reach { Immediate, Recursive }

    public static class EntityExtensions
    {
        public static IEnumerable<Entity> Entities(this GameObject gameObject, bool self = true, Reach? upwards = null, Reach? downwards = null)
        {
            if (self)
            {
                var reference = gameObject.GetComponent<IEntityReference>();
                if (reference != null && reference.Entity) yield return reference.Entity;
            }

            if (upwards.HasValue)
            {
                var transform = gameObject.transform;
                var parent = transform.parent;
                var reach = upwards.Value == Reach.Immediate ? Core.Nullable.Null<Reach>() : Reach.Recursive;
                foreach (var entity in parent.Entities(true, reach, null)) yield return entity;
            }

            if (downwards.HasValue)
            {
                var transform = gameObject.transform;
                var reach = downwards.Value == Reach.Immediate ? Core.Nullable.Null<Reach>() : Reach.Recursive;
                for (var i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    foreach (var entity in child.Entities(true, reach, null)) yield return entity;
                }
            }
        }

        public static IEnumerable<Entity> Entities(this Component component, bool self = true, Reach? upwards = null, Reach? downwards = null) =>
            component.gameObject.Entities(self, upwards, downwards);

        public static bool TryEntity(this GameObject gameObject, out Entity entity, bool self = true, Reach? upwards = null, Reach? downwards = null)
        {
            if (self)
            {
                var reference = gameObject.GetComponent<IEntityReference>();
                if (reference != null && reference.Entity)
                {
                    entity = reference.Entity;
                    return true;
                }
            }

            if (upwards.HasValue)
            {
                var transform = gameObject.transform;
                var parent = transform.parent;
                var reach = upwards.Value == Reach.Immediate ? Core.Nullable.Null<Reach>() : Reach.Recursive;
                if (parent != null && parent.TryEntity(out entity, true, reach, null)) return true;
            }

            if (downwards.HasValue)
            {
                var transform = gameObject.transform;
                var reach = downwards.Value == Reach.Immediate ? Core.Nullable.Null<Reach>() : Reach.Recursive;
                for (var i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    if (child.TryEntity(out entity, true, null, reach)) return true;
                }
            }

            entity = default;
            return false;
        }

        public static bool TryEntity(this Component component, out Entity entity, bool self = true, Reach? upwards = null, Reach? downwards = null) =>
            component.gameObject.TryEntity(out entity, self, upwards, downwards);
    }
}
