using Entia;
using Entia.Components;
using Entia.Core;
using Entia.Delegates;
using Entia.Injectables;
using Entia.Messages;
using Entia.Modules;
using Entia.Systems;
using Entia.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems
{
    public struct SynchronizePooled : IReact<OnAdd<Pooled>>, IReact<OnRemove<Pooled>>
    {
        sealed class Instance
        {
            public readonly GameObject Object;
            public readonly (Component component, IDelegate @delegate)[] Components;

            public Instance(GameObject @object, params (Component component, IDelegate @delegate)[] components)
            {
                Object = @object;
                Components = components;
            }
        }

        sealed class Data
        {
            public readonly EntityReference Reference;
            public readonly GameObject Template;
            public readonly Transform Root;
            public readonly Stack<Instance> Instances;

            public Data(EntityReference reference, GameObject template, Transform root, params Instance[] instances)
            {
                Reference = reference;
                Template = template;
                Root = root;
                Instances = new Stack<Instance>(instances);
            }
        }

        [Default]
        static SynchronizePooled Default => new SynchronizePooled
        {
            _root = new Lazy<Transform>(() => new GameObject("Pools").transform),
            _pools = new Dictionary<EntityReference, Data>(),
            _instances = new Dictionary<GameObject, (Data, Instance)>(),
        };

        public readonly World World;
        public readonly Components<Pooled> Links;

        Lazy<Transform> _root;
        Dictionary<EntityReference, Data> _pools;
        Dictionary<GameObject, (Data data, Instance instance)> _instances;

        void IReact<OnAdd<Pooled>>.React(in OnAdd<Pooled> message)
        {
            ref var link = ref Links.Get(message.Entity);
            if (link.Reference is EntityReference reference)
            {
                var data = GetData(reference);
                var instance = GetInstance(data);
                link.Instance = instance.Object;
                instance.Object.SetActive(true);
                foreach (var (component, @delegate) in instance.Components) @delegate.Set(component, message.Entity, World);
            }
        }

        void IReact<OnRemove<Pooled>>.React(in OnRemove<Pooled> message)
        {
            ref var link = ref Links.Get(message.Entity);
            if (link.Instance != null && _instances.TryGetValue(link.Instance, out var pair))
            {
                foreach (var (component, @delegate) in pair.instance.Components) @delegate.Remove(component, message.Entity, World);
                link.Instance.SetActive(false);
                pair.data.Instances.Push(pair.instance);
            }
        }

        Instance GetInstance(Data data)
        {
            if (data.Instances.Count > 0) return data.Instances.Pop();
            else
            {
                var delegates = World.Delegates();
                var @object = UnityEngine.Object.Instantiate(data.Template, data.Root);
                var components = @object.GetComponentsInChildren<Component>()
                    .DistinctBy(component => component.GetType())
                    .Select(component => (component, delegates.Get(component.GetType())))
                    .ToArray();
                var instance = new Instance(@object, components);
                _instances[@object] = (data, instance);
                return instance;
            }
        }

        Data GetData(EntityReference reference)
        {
            if (_pools.TryGetValue(reference, out var data)) return data;

            var @object = reference.gameObject;
            var active = @object.activeSelf;
            @object.SetActive(false);

            var root = new GameObject(@object.name).transform;
            root.parent = _root.Value;

            try
            {
                var template = UnityEngine.Object.Instantiate(@object, root);
                template.name = @object.name;
                foreach (var component in template.GetComponents<ComponentReference>()) UnityEngine.Object.DestroyImmediate(component);
                foreach (var entity in template.GetComponents<EntityReference>()) UnityEngine.Object.DestroyImmediate(entity);
                return _pools[reference] = new Data(reference, template, root);
            }
            finally { @object.SetActive(active); }
        }
    }
}