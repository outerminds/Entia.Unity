using Entia.Core;
using Entia.Core.Documentation;
using Entia.Dependables;
using Entia.Modules;
using Entia.Modules.Component;
using Entia.Modules.Query;
using Entia.Queriers;
using Entia.Queryables;
using UnityEngine;

namespace Entia.Queryables
{
    [ThreadSafe]
    public readonly struct Unity<T> : IQueryable, IDependable<Dependers.Unity<T>> where T : Object
    {
        sealed class Querier : Querier<Unity<T>>
        {
            public override bool TryQuery(Segment segment, World world, out Query<Unity<T>> query)
            {
                if (world.Queriers().TryQuery<Write<Components.Unity<T>>>(segment, out var write))
                {
                    query = new Query<Unity<T>>(index => new Unity<T>(write.Get(index)), write.Types);
                    return true;
                }

                query = default;
                return false;
            }
        }

        [Querier]
        static readonly Querier _querier = new Querier();

        public ref readonly T Value => ref _value.Value.Value;

        readonly Write<Components.Unity<T>> _value;

        public Unity(Write<Components.Unity<T>> value) { _value = value; }
    }
}