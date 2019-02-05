namespace Systems
{
	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.Spawner), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Spawner" })]
	public static class SpawnerExtensions
	{
		public static bool TryComponent7(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component7> value)
		{
			if (item.Component2.Has)
			{
				value = item.Component2.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static ref readonly global::Components.Inner.Component7 Component7(in this global::Systems.Spawner.Query item, out bool success)
		{
			if (success = item.Component2.Has) return ref item.Component2.Value.Value;
			return ref global::Entia.Core.Dummy<global::Components.Inner.Component7>.Value;
		}
		public static ref global::Components.Component1 Component1(in this global::Systems.Spawner.Query item) => ref item.Component10.Value;
		public static void Component1(in this global::Systems.Spawner.Query item, in global::Components.Component1 value) => item.Component10.Value = value;
		public static global::Entia.Entity Entity(in this global::Systems.Spawner.Query item) => item.Entity;
		public static bool TryComponent2(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component2> value)
		{
			if (item.Component3.Has)
			{
				value = item.Component3.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static ref readonly global::Components.Inner.Component2 Component2(in this global::Systems.Spawner.Query item, out bool success)
		{
			if (success = item.Component3.Has) return ref item.Component3.Value.Value;
			return ref global::Entia.Core.Dummy<global::Components.Inner.Component2>.Value;
		}
		public static bool TryComponent3(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Write<global::Components.Inner.Component3> value)
		{
			if (item.Component4.Value1.Has)
			{
				value = item.Component4.Value1.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static ref global::Components.Inner.Component3 Component3(in this global::Systems.Spawner.Query item, out bool success)
		{
			if (success = item.Component4.Value1.Has) return ref item.Component4.Value1.Value.Value;
			return ref global::Entia.Core.Dummy<global::Components.Inner.Component3>.Value;
		}
		public static bool TryComponent4(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component4> value)
		{
			if (item.Component4.Value2.Has)
			{
				value = item.Component4.Value2.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static ref readonly global::Components.Inner.Component4 Component4(in this global::Systems.Spawner.Query item, out bool success)
		{
			if (success = item.Component4.Value2.Has) return ref item.Component4.Value2.Value.Value;
			return ref global::Entia.Core.Dummy<global::Components.Inner.Component4>.Value;
		}
		public static bool TryComponent5(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component5> value)
		{
			if (item.Component5.Has)
			{
				value = item.Component5.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static ref readonly global::Components.Inner.Component5 Component5(in this global::Systems.Spawner.Query item, out bool success)
		{
			if (success = item.Component5.Has) return ref item.Component5.Value.Value;
			return ref global::Entia.Core.Dummy<global::Components.Inner.Component5>.Value;
		}
		public static ref global::Components.Inner.Component7 Component7(in this global::Systems.Spawner.Query item) => ref item.Component14.Value;
		public static void Component7(in this global::Systems.Spawner.Query item, in global::Components.Inner.Component7 value) => item.Component14.Value = value;
		public static bool TryComponent3(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component3> value)
		{
			if (item.Component31.Has)
			{
				value = item.Component31.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static global::Entia.Entity Entity(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item) => item.Value1;
		public static ref readonly global::Components.Component1 Component1(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item) => ref item.Value2.Value;
		public static bool TryComponent2(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item, out global::Entia.Queryables.Read<global::Components.Inner.Component2> value)
		{
			if (item.Value3.Has)
			{
				value = item.Value3.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static ref readonly global::Components.Inner.Component2 Component2(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item, out bool success)
		{
			if (success = item.Value3.Has) return ref item.Value3.Value.Value;
			return ref global::Entia.Core.Dummy<global::Components.Inner.Component2>.Value;
		}
	}
}