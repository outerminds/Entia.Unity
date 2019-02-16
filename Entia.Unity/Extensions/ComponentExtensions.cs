using Entia.Injectables;
using Entia.Components;
using UnityEngine;

namespace Entia.Unity
{
    public static class ComponentExtensions
    {
        public static bool TryUnity<T>(this Modules.Components components, Entity entity, out T @object) where T : Object
        {
            if (components.TryGet<Unity<T>>(entity, out var unity))
            {
                @object = unity.Value;
                return true;
            }

            @object = default;
            return false;
        }

        public static bool TryUnity<T>(this AllComponents components, Entity entity, out T @object) where T : Object
        {
            if (components.TryGet<Unity<T>>(entity, out var unity))
            {
                @object = unity.Value;
                return true;
            }

            @object = default;
            return false;
        }

        public static T Unity<T>(this Modules.Components components, Entity entity) where T : Object => components.Get<Unity<T>>(entity).Value;
        public static T Unity<T>(this AllComponents components, Entity entity) where T : Object => components.Get<Unity<T>>(entity).Value;
    }
}
