using Entia.Core;
using Entia.Dependables;
using Entia.Modules;
using Entia.Modules.Query;
using Entia.Queriers;
using Entia.Queryables;
using UnityEngine;

namespace Entia.Unity.Queryables
{
	public readonly struct Unity<T> : IQueryable, IDepend<Components.Unity<T>> where T : Object
	{
		sealed class Querier : Querier<Unity<T>>
		{
			public override Query<Unity<T>> Query(World world)
			{
				var components = world.Components();
				var mask = IndexUtility<IComponent>.Cache<Components.Unity<T>>.Mask;
				return new Query<Unity<T>>(
					new Query(new Filter(mask, null, typeof(Components.Unity<T>)), current => current.HasAll(mask)),
					(Entity entity, out Unity<T> unity) =>
					{
						if (components.TryRead<Components.Unity<T>>(entity, out var component) && component.Value.Value is T @object)
						{
							unity = new Unity<T>(@object);
							return true;
						}

						unity = default;
						return false;
					});
			}
		}

		[Querier]
		static readonly Querier _querier = new Querier();

		public readonly T Value;
		public Unity(T value) { Value = value; }
	}
}