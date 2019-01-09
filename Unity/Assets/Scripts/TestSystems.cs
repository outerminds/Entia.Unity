using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Systems
{
    public struct Move : IRun
    {
        public AllComponents Components;
        public Group<Entity, Read<Components.Component1>> Group;

        public void Run()
        {
            foreach (var item in Group)
            {
                // var entity = item.Entity();
                // ref var poulah = ref item.Poulah();

                // Components.Add<Components.Viarge>();
                // 'poulah' pointer becomes invalid...

                // Components.Remove<Components.Viarge>();
                // 'poulah' pointer is still invalid...
            }
        }
    }
}