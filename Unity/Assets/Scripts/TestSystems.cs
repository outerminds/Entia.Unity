using Entia;
using Entia.Core;
using Entia.Injectables;
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
            foreach (var item in OnlyUnity)
            {
                var entity = item.Entity();
            }

            foreach (var item in Ambiguous)
            {
                var ambiguous = item.Ambiguous();

            }
        }
    }

    public struct Spawner : IRun
    {
        [All(typeof(Entia.Components.Unity<>))]
        public struct Query : IQueryable
        {
            public Maybe<Read<Components.Inner.Component7>> Component2;
            public Write<Components.Component1> Component10;
            public Entity Entity;
            public Write<Components.Component1> Component11;
            public Maybe<Read<Components.Inner.Component2>> Component3;
            public Write<Components.Component1> Component12;
            public Any<Write<Components.Inner.Component3>, Read<Components.Inner.Component4>> Component4;
            public Write<Components.Component1> Component13;
            public Maybe<Read<Components.Inner.Component5>> Component5;
            public Write<Components.Inner.Component7> Component14;
            public Maybe<Read<Components.Inner.Component3>> Component31;
            public Maybe<Read<Components.Inner.Component4>> Component41;
            public Maybe<Unity<Transform>> Transform1;
            public Unity<Transform> Transform2;
            public Unity<GameObject> GameObject;
            public Maybe<Read<Entia.Components.Debug>> Debug1;
            public Read<Entia.Components.Debug> Debug2;
        }

        public Resource<Resources.Prefabs> Prefabs;
        public Group<Query> Group1;
        [All(typeof(Entia.Components.Unity<>))]
        public Group<Entity, Read<Components.Component1>, Maybe<Read<Components.Inner.Component2>>> Group2;
        public AllEntities Entities;
        public Group<Entity> Group3;

        public void Run()
        {
            if (Input.GetKey(KeyCode.K)) Object.Instantiate(Prefabs.Value.Prefab);

            if (Input.GetKey(KeyCode.L))
            {
                foreach (var item in Group1)
                {
                    ref var c0 = ref item.Component1();
                    ref var c1 = ref item.Component7();
                    ref readonly var c2 = ref item.Component2(out var s1, out var st1);
                    ref var c3 = ref item.Component3(out var s2);
                    ref readonly var c4 = ref item.Component5(out var s3);
                    Debug.Log(c0.X + c1.I + c2.G + c3.A + c4.C);
                    Entities.Destroy(item.Entity);
                    break;
                }
            }

            if (Input.GetKey(KeyCode.J))
            {
                foreach (var item in Group2)
                {
                    Entities.Destroy(item.Entity());
                    break;
                }
            }
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