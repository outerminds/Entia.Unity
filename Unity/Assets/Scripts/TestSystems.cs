using Entia;
using Entia.Core;
using Entia.Injectables;
using Entia.Messages;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Systems
{
    public struct Move : IRun
    {
        [None(typeof(Components.Inner.Component2))]
        public Group<Read<Components.Component1>> Group;

        public void Run()
        {
            foreach (ref readonly var item in Group)
            {
                ref readonly var component1 = ref item.Component1();
            }
        }
    }

    public struct Emitter : IRun
    {
        public Emitter<Messages.MessageA> MessageA;

        public void Run()
        {
            for (int i = 0; i < 10; i++) MessageA.Emit(default);
        }
    }

    public struct Reactive : IReact<Messages.MessageA>
    {
        public void React(in Messages.MessageA message)
        {

        }
    }

    public struct Queries : IRun
    {
        [All(typeof(IComponent))]
        public Group<Entity> NoEmpty;
        [None(typeof(IComponent))]
        public Group<Entity> OnlyEmpty;
        [None(typeof(Entia.Components.Unity<>))]
        public Group<Entity> NoUnity;
        [All(typeof(Entia.Components.Unity<>))]
        public Group<Entity> OnlyUnity;
        public Group<Read<Components.Inner.Ambiguous>, Read<Components.Ambiguous>> Ambiguous;

        public void Run()
        {
            foreach (ref readonly var item in OnlyUnity)
            {
                var entity = item.Entity();
            }

            foreach (ref readonly var item in Ambiguous)
            {
                var ambiguous = item.Ambiguous();

            }
        }
    }

    public unsafe struct Spawner : IRun,
        IReact<OnCreate>,
        IReact<OnPreDestroy>,
        IReact<OnAdopt>,
        IReact<OnReject>,
        IReact<OnAdd<Components.Component1>>,
        IReact<OnRemove<Entia.Components.Unity<Transform>>>,
        IOnAdd<Components.Component1>,
        IOnRemove<Entia.Components.Unity<Transform>>
    {
        [All(typeof(Entia.Components.Unity<>))]
        public struct Query : IQueryable
        {
            public Components.Inner.Component4* Pointer1;
            public Maybe<Read<Components.Inner.Component7>> Component2;
            public Write<Components.Component1> Component10;
            public Entity Entity;
            public Write<Components.Component1> Component11;
            public Components.Inner.Component5* Pointer2;
            public Maybe<Read<Components.Inner.Component2>> Component3;
            public Write<Components.Component1> Component12;
            public Any<Write<Components.Inner.Component3>, Read<Components.Inner.Component4>> Component4;
            public Write<Components.Component1> Component13;
            public Components.Inner.Component3* Pointer3;
            public Maybe<Read<Components.Inner.Component5>> Component5;
            public Write<Components.Inner.Component7> Component14;
            public Maybe<Read<Components.Inner.Component3>> Component31;
            public Maybe<Read<Components.Inner.Component4>> Component41;
            public Maybe<Unity<Transform>> Transform1;
            public Components.Inner.Component7* Pointer4;
            public Unity<Transform> Transform2;
            public Unity<GameObject> GameObject;
            public Maybe<Read<Entia.Components.Debug>> Debug1;
            public Components.Inner.Component2* Pointer5;
        }

        public Resource<Resources.Prefabs> Prefabs;
        public Group<Query> Group1;
        [All(typeof(Entia.Components.Unity<>))]
        public Group<Entity, Read<Components.Component1>, Maybe<Read<Components.Inner.Component2>>> Group2;
        public AllEntities Entities;
        public AllFamilies Families;
        public Group<Entity> Group3;

        public void Run()
        {
            if (Input.GetKey(KeyCode.K))
            {
                Debug.Log(nameof(KeyCode.K));
                Object.Instantiate(Prefabs.Value.Prefab);
            }

            if (Input.GetKey(KeyCode.L))
            {
                Debug.Log($"{nameof(KeyCode.L)} | {Group1.Count}");
                foreach (ref readonly var item in Group1)
                {
                    ref var c0 = ref item.Component1();
                    ref var c1 = ref item.Component7();
                    ref readonly var c2 = ref item.Component2(out var s1, out var st1);
                    ref var c3 = ref item.Component3(out var s2);
                    ref readonly var c4 = ref item.Component5(out var s3);
                    Debug.Log(c0.X + c1.I + c2.G + c3.A + c4.C);
                    Debug.Log($"{item.Pointer1->A++} | {item.Pointer2->A++} | {item.Pointer3->A++} | {item.Pointer4->A++} | {item.Pointer5->A++}");
                    Debug.Log($"{s1}, {s2}, {s3}");
                    // Entities.Destroy(item.Entity);
                    break;
                }
            }

            if (Input.GetKey(KeyCode.J))
            {
                Debug.Log($"{nameof(KeyCode.J)} | {Group2.Count}");
                foreach (ref readonly var item in Group2)
                {
                    Entities.Destroy(item.Entity());
                    break;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                var source = Random.Range(0, Entities.Count);
                var target = Random.Range(0, Entities.Count);
                if (Entities.TryAt(source, out var parent) &&
                    Entities.TryAt(target, out var child))
                    Families.Adopt(parent, child);
            }

            if (Input.GetKey(KeyCode.R))
            {
                var index = Random.Range(0, Entities.Count);
                if (Entities.TryAt(index, out var child)) Families.Reject(child);
            }
        }

        public void React(in OnAdd<Components.Component1> message)
        {
            Debug.Log($"React OnAdd: {message.Entity}");
        }

        public void React(in OnRemove<Entia.Components.Unity<Transform>> message)
        {
            Debug.Log($"React OnRemove: {message.Entity}");
        }

        public void React(in OnCreate message)
        {
            Debug.Log($"React OnCreate: {message.Entity}");
        }

        public void React(in OnPreDestroy message)
        {
            Debug.Log($"React OnPreDestroy: {message.Entity}");
        }

        public void React(in OnAdopt message)
        {
            Debug.Log($"React OnAdopt: {message.Parent} | {message.Child}");
        }

        public void React(in OnReject message)
        {
            Debug.Log($"React OnReject: {message.Parent} | {message.Child}");
        }

        public void OnAdd(Entity entity, ref Components.Component1 component)
        {
            Debug.Log($"OnAdd: {entity} | {component}");
        }

        public void OnRemove(Entity entity, ref Entia.Components.Unity<Transform> transform)
        {
            Debug.Log($"OnRemove: {entity} | {transform.Value}", transform.Value);
        }
    }

    public struct EmitterA : IRun
    {
        public AllEmitters Emitters;

        public void Run()
        {
            Debug.Log("A | " + Time.frameCount);
            Emitters.Emit(new Messages.MessageA());
            Debug.Log("B | " + Time.frameCount);
            Emitters.Emit(new Messages.MessageB());
            Debug.Log("C | " + Time.frameCount);
            Emitters.Emit(new Messages.MessageC());
        }
    }

    public struct Reactor1 : IReact<Messages.MessageA>, IReact<Messages.MessageB>, IReact<Messages.MessageC>
    {
        public void React(in Messages.MessageA message) => Debug.Log(GetType().Format() + " | " + message);
        public void React(in Messages.MessageB message) => Debug.Log(GetType().Format() + " | " + message);
        public void React(in Messages.MessageC message) => Debug.Log(GetType().Format() + " | " + message);
    }

    public struct Reactor2 : IReact<Messages.MessageB>
    {
        public void React(in Messages.MessageA message) => Debug.Log(GetType().Format() + " | " + message);
        public void React(in Messages.MessageB message) => Debug.Log(GetType().Format() + " | " + message);
        public void React(in Messages.MessageC message) => Debug.Log(GetType().Format() + " | " + message);
    }

    public struct Reactor3 : IReact<Messages.MessageA>, IReact<Messages.MessageB>
    {
        public void React(in Messages.MessageA message) => Debug.Log(GetType().Format() + " | " + message);
        public void React(in Messages.MessageB message) => Debug.Log(GetType().Format() + " | " + message);
        public void React(in Messages.MessageC message) => Debug.Log(GetType().Format() + " | " + message);
    }
}