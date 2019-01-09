using System.Collections.Generic;

namespace Entia.Unity
{
    public static class EntityRegistry
    {
        public static IEnumerable<IEntityReference> References => _entities.Values;

        static readonly Dictionary<(World, Entity), IEntityReference> _entities = new Dictionary<(World, Entity), IEntityReference>();

        public static bool TryGet(World world, Entity entity, out IEntityReference reference) => _entities.TryGetValue((world, entity), out reference);

        public static bool Set(IEntityReference reference)
        {
            var key = (reference.World, reference.Entity);
            if (_entities.ContainsKey(key)) return false;

            _entities.Add(key, reference);
            return true;
        }

        public static bool Has(World world, Entity entity) => _entities.ContainsKey((world, entity));
        public static bool Has(IEntityReference reference) => _entities.ContainsValue(reference);

        public static bool Remove(IEntityReference reference) =>
            _entities.Remove((reference.World, reference.Entity));
    }
}