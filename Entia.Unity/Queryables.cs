﻿using Entia.Core;
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
    public readonly struct Unity<T> : IQueryable<Unity<T>.Querier> where T : Object
    {
        sealed class Querier : Querier<Unity<T>>
        {
            public override bool TryQuery(Segment segment, World world, out Query<Unity<T>> query)
            {
                if (world.Queriers().TryQuery<Read<Components.Unity<T>>>(segment, out var read))
                {
                    query = new Query<Unity<T>>(index => new Unity<T>(read.Get(index)), read.Types);
                    return true;
                }

                query = default;
                return false;
            }
        }

        public T Value => _value.Value.Value;
        readonly Read<Components.Unity<T>> _value;
        public Unity(in Read<Components.Unity<T>> value) { _value = value; }
    }
}