using System.Collections.Generic;
using System.Linq;

namespace Entia.Unity.Generation
{
    public static class GeneratorExtensions
    {
        public static T ToEntia<T>(this T value) => value;
        public static Entity ToEntia(this EntityReference reference) => reference?.Entity ?? Entity.Zero;
        public static Entity[] ToEntia(this EntityReference[] references) => references?.Select(ToEntia).ToArray();
        public static Entity[][] ToEntia(this EntityReference[][] references) => references?.Select(ToEntia).ToArray();
        public static List<Entity>[] ToEntia(this List<EntityReference>[] references) => references?.Select(ToEntia).ToArray();
        public static List<Entity> ToEntia(this List<EntityReference> references) => references?.Select(ToEntia).ToList();
        public static List<Entity[]> ToEntia(this List<EntityReference[]> references) => references?.Select(ToEntia).ToList();
        public static List<List<Entity>> ToEntia(this List<List<EntityReference>> references) => references?.Select(ToEntia).ToList();

        public static T FromEntia<T>(this T value, World _) => value;
        public static EntityReference FromEntia(this Entity entity, World world) => entity && EntityRegistry.TryGet(world, entity, out var reference) ? reference as EntityReference : default;
        public static EntityReference[] FromEntia(this Entity[] entities, World world) => entities?.Select(entity => FromEntia(entity, world)).ToArray();
        public static EntityReference[][] FromEntia(this Entity[][] entities, World world) => entities?.Select(entity => FromEntia(entity, world)).ToArray();
        public static List<EntityReference>[] FromEntia(this List<Entity>[] entities, World world) => entities?.Select(entity => FromEntia(entity, world)).ToArray();
        public static List<EntityReference> FromEntia(this List<Entity> entities, World world) => entities?.Select(entity => FromEntia(entity, world)).ToList();
        public static List<EntityReference[]> FromEntia(this List<Entity[]> entities, World world) => entities?.Select(entity => FromEntia(entity, world)).ToList();
        public static List<List<EntityReference>> FromEntia(this List<List<Entity>> entities, World world) => entities?.Select(entity => FromEntia(entity, world)).ToList();
    }
}
