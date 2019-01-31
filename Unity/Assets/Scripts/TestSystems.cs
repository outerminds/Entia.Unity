using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Systems
{
    public struct Move : IRun
    {
        public AllComponents Components;
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

    public struct Queries : ISystem
    {
        [All(typeof(IComponent))]
        public Group<Entity> NoEmpty;
        [None(typeof(IComponent))]
        public Group<Entity> OnlyEmpty;
        [None(typeof(Entia.Components.Unity<>))]
        public Group<Entity> NoUnity;
        [All(typeof(Entia.Components.Unity<>))]
        public Group<Entity> OnlyUnity;
    }

    public struct Spawner : IRun
    {
        [All(typeof(Entia.Components.Unity<>))]
        public struct Query : IQueryable
        {
            public Entity Entity;
            public Read<Components.Component1> Component1;
            public Maybe<Read<Components.Inner.Component2>> Component2;
        }

        public struct Queryz : IQueryable
        {
            public Query Query1;
            public Query Query2;
            public Query Query3;
        }

        public Resource<Resources.Prefabs> Prefabs;
        public Group<Query> Group;
        public Group<Queryz> Groupz;
        public AllEntities Entities;
        public AllComponents Components;

        public void Run()
        {
            if (Input.GetKey(KeyCode.K)) Object.Instantiate(Prefabs.Value.Prefab);

            if (Input.GetKey(KeyCode.L))
            {
                foreach (var item in Group)
                {
                    Entities.Destroy(item.Entity);
                    break;
                }

                foreach (var item in Groupz)
                {

                }
            }
        }
    }
}