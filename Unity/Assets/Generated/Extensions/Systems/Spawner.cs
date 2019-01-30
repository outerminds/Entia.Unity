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
		public static global::Entia.Entity Entity(in this global::Systems.Spawner.Queryz item) => item.Query1.Entity;
		public static ref readonly global::Components.Component1 Component1(in this global::Systems.Spawner.Queryz item) => ref item.Query1.Component1.Value;
		public static bool TryComponent2(in this global::Systems.Spawner.Queryz item, out global::Entia.Queryables.Read<global::Components.Inner.Component2> value)
		{
			if (item.Query1.Component2.Has)
			{
				value = item.Query1.Component2.Value;
				return true;
			}
			
			value = default;
			return false;
		}
	}
}