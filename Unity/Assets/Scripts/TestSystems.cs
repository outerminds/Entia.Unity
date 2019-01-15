using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Systems
{
    public struct Move : IRun
    {
        public AllComponents Components;
        [None(typeof(Components.Inner.Component2))]
        public Group<Read<Components.Component1>> Group;

        public void Run()
        {
            foreach (var (entity, item) in Group)
            {
                ref readonly var component1 = ref item.Component1();
            }
        }
    }
}