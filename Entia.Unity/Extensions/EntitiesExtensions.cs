using System.Collections.Generic;
using Entia.Core;
using Entia.Injectors;
using Entia.Modules;
using Entia.Unity;

namespace Entia.Unity
{
    public static class EntitiesExtensions
    {
        static readonly Pool<List<ComponentReference>> _references = new Pool<List<ComponentReference>>(
            () => new List<ComponentReference>(),
            instance => instance.Clear());

        public static Entity Create(this World world, EntityReference reference, Depth depth = Depth.Shallow)
        {
            var entity = world.Entities().Create();
            if (UnityEngine.Debug.isDebugBuild) world.Components().Set(entity, new Components.Debug { Name = reference.name });

            var delegates = world.Delegates();
            using (var list = _references.Use())
            {
                reference.GetComponents(list.Instance);
                foreach (var component in list.Instance) delegates.Get(component.GetType()).Clone(component, entity, depth, world);
            }
            return entity;
        }
    }
}