namespace Systems
{
	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.Spawner), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Spawner" })]
	public static class SpawnerExtensions
	{
		public static global::Entia.Entity Entity(in this global::Systems.Spawner.Query item) => item.Entity;
		public static ref readonly global::Components.Component1 Component1(in this global::Systems.Spawner.Query item) => ref item.Component1.Value;
		public static bool TryComponent2(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component2> value)
		{
			if (item.Component2.Has)
			{
				value = item.Component2.Value;
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