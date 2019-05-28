namespace Systems
{
	using Entia.Queryables;
	using Entia.Unity;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Systems.Spawner), Link = "Assets/Scripts/TestSystems.cs", Path = new string[] { "Systems", "Spawner" })]
	public static class SpawnerExtensions
	{
		public static bool TryComponent7(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component7> value) => item.Component2.TryGet(out value);
		public static ref readonly global::Components.Inner.Component7 Component7(in this global::Systems.Spawner.Query item, out bool success) => ref item.Component2.Get(out success);
		public static ref readonly global::Components.Inner.Component7 Component7(in this global::Systems.Spawner.Query item, out bool success, out global::Entia.States state) => ref item.Component2.Get(out success, out state);
		public static ref global::Components.Component1 Component1(in this global::Systems.Spawner.Query item) => ref item.Component10.Value;
		public static ref global::Components.Component1 Component1(in this global::Systems.Spawner.Query item, out global::Entia.States state) => ref item.Component10.Get(out state);
		public static void Component1(in this global::Systems.Spawner.Query item, in global::Components.Component1 value) => item.Component10.Value = value;
		public static global::Entia.Entity Entity(in this global::Systems.Spawner.Query item) => item.Entity;
		public static bool TryComponent2(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component2> value) => item.Component3.TryGet(out value);
		public static ref readonly global::Components.Inner.Component2 Component2(in this global::Systems.Spawner.Query item, out bool success) => ref item.Component3.Get(out success);
		public static ref readonly global::Components.Inner.Component2 Component2(in this global::Systems.Spawner.Query item, out bool success, out global::Entia.States state) => ref item.Component3.Get(out success, out state);
		public static bool TryComponent3(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Write<global::Components.Inner.Component3> value) => item.Component4.Value1.TryGet(out value);
		public static ref global::Components.Inner.Component3 Component3(in this global::Systems.Spawner.Query item, out bool success) => ref item.Component4.Value1.Get(out success);
		public static ref global::Components.Inner.Component3 Component3(in this global::Systems.Spawner.Query item, out bool success, out global::Entia.States state) => ref item.Component4.Value1.Get(out success, out state);
		public static bool TryComponent4(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component4> value) => item.Component4.Value2.TryGet(out value);
		public static ref readonly global::Components.Inner.Component4 Component4(in this global::Systems.Spawner.Query item, out bool success) => ref item.Component4.Value2.Get(out success);
		public static ref readonly global::Components.Inner.Component4 Component4(in this global::Systems.Spawner.Query item, out bool success, out global::Entia.States state) => ref item.Component4.Value2.Get(out success, out state);
		public static bool TryComponent5(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component5> value) => item.Component5.TryGet(out value);
		public static ref readonly global::Components.Inner.Component5 Component5(in this global::Systems.Spawner.Query item, out bool success) => ref item.Component5.Get(out success);
		public static ref readonly global::Components.Inner.Component5 Component5(in this global::Systems.Spawner.Query item, out bool success, out global::Entia.States state) => ref item.Component5.Get(out success, out state);
		public static ref global::Components.Inner.Component7 Component7(in this global::Systems.Spawner.Query item) => ref item.Component14.Value;
		public static ref global::Components.Inner.Component7 Component7(in this global::Systems.Spawner.Query item, out global::Entia.States state) => ref item.Component14.Get(out state);
		public static void Component7(in this global::Systems.Spawner.Query item, in global::Components.Inner.Component7 value) => item.Component14.Value = value;
		public static bool TryComponent3(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Components.Inner.Component3> value) => item.Component31.TryGet(out value);
		public static bool TryTransform(in this global::Systems.Spawner.Query item, out global::UnityEngine.Transform value) => item.Transform1.TryGet(out value);
		public static global::UnityEngine.Transform Transform(in this global::Systems.Spawner.Query item, out bool success) => item.Transform1.Get(out success);
		public static global::UnityEngine.Transform Transform(in this global::Systems.Spawner.Query item) => item.Transform2.Value;
		public static global::UnityEngine.GameObject GameObject(in this global::Systems.Spawner.Query item) => item.GameObject.Value;
		public static bool TryDebug(in this global::Systems.Spawner.Query item, out global::Entia.Queryables.Read<global::Entia.Components.Debug> value) => item.Debug1.TryGet(out value);
		public static ref readonly global::Entia.Components.Debug Debug(in this global::Systems.Spawner.Query item, out bool success) => ref item.Debug1.Get(out success);
		public static global::Entia.Entity Entity(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item) => item.Value1;
		public static ref readonly global::Components.Component1 Component1(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item) => ref item.Value2.Value;
		public static ref readonly global::Components.Component1 Component1(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item, out global::Entia.States state) => ref item.Value2.Get(out state);
		public static bool TryComponent2(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item, out global::Entia.Queryables.Read<global::Components.Inner.Component2> value) => item.Value3.TryGet(out value);
		public static ref readonly global::Components.Inner.Component2 Component2(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item, out bool success) => ref item.Value3.Get(out success);
		public static ref readonly global::Components.Inner.Component2 Component2(in this global::Entia.Queryables.All<global::Entia.Entity, global::Entia.Queryables.Read<global::Components.Component1>, global::Entia.Queryables.Maybe<global::Entia.Queryables.Read<global::Components.Inner.Component2>>> item, out bool success, out global::Entia.States state) => ref item.Value3.Get(out success, out state);
	}
}