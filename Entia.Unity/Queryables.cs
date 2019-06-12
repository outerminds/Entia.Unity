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
    public readonly struct Unity<T> : IQueryable where T : Object
    {
        sealed class Querier : Querier<Unity<T>>
        {
            public override bool TryQuery(in Context context, out Query<Unity<T>> query)
            {
                if (context.World.Queriers().TryQuery<Read<Components.Unity<T>>>(context, out var read))
                {
                    query = new Query<Unity<T>>(index => new Unity<T>(read.Get(index)), read.Types);
                    return true;
                }

                query = default;
                return false;
            }
        }

        [Querier]
        static readonly Querier _querier = new Querier();

        public T Value => _value.Value.Value;
        readonly Read<Components.Unity<T>> _value;
        public Unity(in Read<Components.Unity<T>> value) { _value = value; }
    }
}