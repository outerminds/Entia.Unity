namespace Systems
{
	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.Spawner), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Spawner" })]
	public static class SpawnerExtensions
	{
		public static bool TryComponent1(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component1> value)
		{
			if (item.Component2.Has)
			{
				value = item.Component2.Value;
				return true;
			}
		
			value = default;
			return false;
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
		public static bool TryComponent3(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Write<global::Components.Inner.Component3> value)
		{
			if (item.Component4.Value.Value1.Has)
			{
				value = item.Component4.Value.Value1.Value;
				return true;
			}
		
			value = default;
			return false;
		}
		public static bool TryComponent4(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component4> value)
		{
			if (item.Component4.Value.Value2.Has)
			{
				value = item.Component4.Value.Value2.Value;
				return true;
			}
		
			value = default;
			return false;
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
	}
}